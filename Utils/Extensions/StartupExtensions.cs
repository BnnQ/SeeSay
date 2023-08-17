using System.Text.Json.Serialization;
using Azure.Storage.Blobs;
using Google.Apis.PeopleService.v1;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using SeeSay.Authentication;
using SeeSay.Models.Contexts;
using SeeSay.Models.Entities;
using SeeSay.Services;
using SeeSay.Services.Abstractions;
using SeeSay.Services.MapperProfiles;

namespace SeeSay.Utils.Extensions;

public static class StartupExtensions
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers(options => options.AllowEmptyInputInBodyModelBinding = true)
            .AddJsonOptions(
                options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                });

        builder.Services.AddDbContext<SqlServerDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

        builder.Services.AddIdentity<User, IdentityRole>(
                options =>
                {
                    // Password settings
                    if (!builder.Environment.IsProduction())
                    {
                        options.Password.RequireDigit = true;
                        options.Password.RequiredLength = 3;
                        options.Password.RequireNonAlphanumeric = false;
                        options.Password.RequireUppercase = false;
                        options.Password.RequireLowercase = false;
                    }
                    else
                    {
                        options.Password.RequireDigit = true;
                        options.Password.RequiredLength = 8;
                        options.Password.RequireNonAlphanumeric = true;
                        options.Password.RequireUppercase = true;
                        options.Password.RequireLowercase = true;
                    }

                    // Lockout settings
                    if (builder.Environment.IsProduction())
                    {
                        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(value: 1);
                    }
                    else
                    {
                        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(value: 365);
                    }

                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.AllowedForNewUsers = true;
                })
            .AddEntityFrameworkStores<SqlServerDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddAuthentication()
            .AddCookie(options =>
            {
                options.LoginPath = "https://localhost:4200/account/login";
                options.Cookie.Expiration = TimeSpan.FromHours(1);
            })
            .AddGoogle(options =>
            {
                const string ClientIdPath = "Authentication:Google:ClientId";
                options.ClientId = builder.Configuration[ClientIdPath] ??
                                   throw new InvalidOperationException(
                                       $"'{ClientIdPath}' configuration value is not provided.");
                const string ClientSecretPath = "Authentication:Google:ClientSecret";
                options.ClientSecret = builder.Configuration[ClientSecretPath] ??
                                       throw new InvalidOperationException(
                                           $"'{ClientSecretPath}' configuration value is not provided.");

                options.SaveTokens = true;
                options.Scope.Add(PeopleServiceService.ScopeConstants.UserinfoEmail);
                options.Scope.Add(PeopleServiceService.ScopeConstants.UserinfoProfile);
                options.Events = new GoogleOAuthEvents();
            })
            .AddGitHub(options =>
            {
                const string ClientIdPath = "Authentication:GitHub:ClientId";
                options.ClientId = builder.Configuration[ClientIdPath] ??
                                   throw new InvalidOperationException(
                                       $"'{ClientIdPath}' configuration value is not provided.");
                const string ClientSecretPath = "Authentication:GitHub:ClientSecret";
                options.ClientSecret = builder.Configuration[ClientSecretPath] ??
                                       throw new InvalidOperationException(
                                           $"'{ClientSecretPath}' configuration value is not provided.");

                options.Scope.Add("user:email");
            });

        builder.Services.AddAuthorization(
            options =>
            {
                options.AddPolicy(name: "Authenticated",
                    policy => policy.RequireAuthenticatedUser());
            });

        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(
            options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromMinutes(value: 60);
            });

        builder.Services.AddHttpClient("AzureFunctionClient", httpClient =>
        {
            const string AzureFunctionBaseUrlPath = "Azure:FunctionApp:BaseAddress";
            var azureFunctionBaseUrl = builder.Configuration[AzureFunctionBaseUrlPath] ??
                                       throw new InvalidOperationException(
                                           $"'{AzureFunctionBaseUrlPath}' configuration value is not provided");

            httpClient.BaseAddress = new Uri(azureFunctionBaseUrl);
        });

        builder.Services.AddAutoMapper(profiles =>
        {
            profiles.AddProfile<CategoryToCategoryCreateDtoProfile>();
            profiles.AddProfile<CommentToCommentCreateDtoProfile>();
            profiles.AddProfile<CommentToCommentEditDtoProfile>();
            profiles.AddProfile<LikeToLikeAddDtoProfile>();
            profiles.AddProfile<PostToPostCreateDtoProfile>();
            profiles.AddProfile<PostToFullPostDtoProfile>();
            profiles.AddProfile<UserToUserLoginDtoProfile>();
            profiles.AddProfile<UserToUserRegisterDtoProfile>();
            profiles.AddProfile<LinkEditDtoToLinkProfile>();
        });

        builder.Services.AddSwaggerGen();

        builder.Services.AddScoped<ICategoryRepository, DatabaseCategoryRepository>();
        builder.Services.AddScoped<ICommentRepository, DatabaseCommentRepository>();
        builder.Services.AddScoped<ILikeRepository, DatabaseLikeRepository>();
        builder.Services.AddScoped<IPostRepository, DatabasePostRepository>();

        builder.Services.AddAzureClients(clients =>
        {
            clients.AddBlobServiceClient(builder.Configuration.GetConnectionString("AzureStorage"));
        });

        builder.Services.Configure<Configuration.Azure>(builder.Configuration.GetRequiredSection("Azure"));

        builder.Services.AddTransient<IFileNameGenerator, UniqueFileNameGenerator>();
        builder.Services.AddSingleton<IFormFileManager, AzureFormFileManager>(services =>
        {
            var azureOptions = services.GetRequiredService<IOptions<Configuration.Azure>>();
            var containerName = azureOptions.Value.AvatarImageContainerName;
            if (string.IsNullOrWhiteSpace(containerName))
            {
                throw new InvalidOperationException(
                    "The Azure:AvatarImageContainerName configuration value is missing.");
            }

            var blobServiceClient = services.GetRequiredService<BlobServiceClient>();
            var fileNameGenerator = services.GetRequiredService<IFileNameGenerator>();
            return new AzureFormFileManager(blobServiceClient, containerName, fileNameGenerator);
        });

        return builder;
    }

    public static void Configure(this WebApplication app)
    {
        app.UseCors(policyBuilder =>
        {
            policyBuilder.AllowAnyMethod()
                .AllowAnyHeader()
                .WithOrigins("https://localhost:4200")
                .SetIsOriginAllowedToAllowWildcardSubdomains()
                .AllowCredentials();
        });

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options => { options.SwaggerEndpoint("v1/swagger.json", "SeeSay API"); });
        }
        else
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseSession();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller}/{action=Index}/{id?}");

        app.MapFallbackToFile("index.html");
    }
}