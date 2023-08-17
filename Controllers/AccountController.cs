using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SeeSay.Exceptions;
using SeeSay.Models.Contexts;
using SeeSay.Models.Dto;
using SeeSay.Models.Dto.Users;
using SeeSay.Models.Entities;
using SeeSay.Services.Abstractions;
using SeeSay.Utils.Extensions;
using Microsoft.EntityFrameworkCore;

namespace SeeSay.Controllers;

[Route(template: "api/[controller]/[action]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<User> userManager;
    private readonly SignInManager<User> signManager;
    private readonly IMapper mapper;
    private readonly ILogger<AccountController> logger;
    private readonly SqlServerDbContext context;

    public AccountController(UserManager<User> userManager,
        SignInManager<User> signManager,
        IMapper mapper,
        ILoggerFactory loggerFactory,
        SqlServerDbContext context)
    {
        this.userManager = userManager;
        this.signManager = signManager;
        this.mapper = mapper;
        this.context = context;
        logger = loggerFactory.CreateLogger<AccountController>();
    }

    [HttpGet("{username}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetUser(string username)
    {
        var user = await context.Users.Where(user => !string.IsNullOrWhiteSpace(user.UserName) && user.UserName.Equals(username)).Include(user => user.SocialMediaLinks).FirstOrDefaultAsync();
        if (user is null)
            throw new EntityNotFoundException();

        return Ok(user);
    }

    [HttpPost("{id}")]
    [Authorize]
    public async Task<IActionResult> EditUserProfile([FromRoute] string id,
        [FromForm] UserEditDto userEditDto,
        IFormFile? avatar,
        [FromServices] IFormFileManager fileManager)
    {
        var userEntry = await context.Users.Where(user => user.Id.Equals(id)).Include(user => user.SocialMediaLinks).FirstOrDefaultAsync();
        if (userEntry is null)
            throw new EntityNotFoundException();

        if (!string.IsNullOrWhiteSpace(userEditDto.FirstName))
        {
            userEntry.FirstName = userEditDto.FirstName;
        }

        if (!string.IsNullOrWhiteSpace(userEditDto.LastName))
        {
            userEntry.LastName = userEditDto.LastName;
        }

        if (!string.IsNullOrWhiteSpace(userEditDto.Bio))
        {
            userEntry.Bio = userEditDto.Bio;
        }

        if (!string.IsNullOrWhiteSpace(userEditDto.Location))
        {
            userEntry.Location = userEditDto.Location;
        }

        if (userEditDto.SocialMediaLinks is not null)
        {
            userEntry.SocialMediaLinks = mapper.Map<List<SocialMediaLink>>(userEditDto.SocialMediaLinks);
        }

        if (avatar is not null)
        {
            var imagePath = await fileManager.SaveFileAsync(avatar);
            userEntry.AvatarImagePath = imagePath.ToString();
        }


        await context.SaveChangesAsync();
        return Ok(userEntry);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register(UserRegisterDto registerDto)
    {
        if (!registerDto.Validate(out var errorMessage))
        {
            logger.LogWarning(
                "[POST] Register: entered registration data is invalid; returning 400 Bad Request");
            
            return BadRequest(errorMessage);
        }

        var user = mapper.Map<User>(registerDto);
        user.LockoutEnabled = false;
        var password = registerDto.Password;
        var registrationResult = await userManager.CreateAsync(user, password);

        if (registrationResult.Succeeded)
        {
            await signManager.SignInAsync(user, isPersistent: false);
            logger.LogInformation("[POST] Register: successfully registered user {Email}", user.Email);
            return Ok();
        }

        logger.LogWarning(
            "[POST] Register: registration result is not succeeded; returning 400 Bad Request");
        return BadRequest("An unexpected error occured during registration");
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(UserLoginDto loginDto)
    {

        if (!loginDto.Validate(out var errorMessage))
        {
            logger.LogWarning("[POST] Login: {ErrorMessage}; returning 400 Bad Request", errorMessage);
            return BadRequest(errorMessage);
        }

        var user = await userManager.FindByNameAsync(loginDto.UserName);
        if (user is null)
        {
            errorMessage = "User with such username not found.";
            logger.LogWarning("[POST] Login: {ErrorMessage}; returning 400 Bad Request", errorMessage);
            return BadRequest(errorMessage);
        }

        var result = await signManager.PasswordSignInAsync(
            user, loginDto.Password, isPersistent: true, lockoutOnFailure: true);
        if (result.Succeeded)
        {
            logger.LogInformation(
                "[POST] Login: successfully executed Login for user {Email}, returning 200 OK",
                user.Email);
            return Ok();
        }

        if (await userManager.IsLockedOutAsync(user))
        {
            errorMessage =
                "Your account has been blocked due to a high number of failed login attempts.";
        }
        else
        {
            const int MaxFailedAccessAttempts = 5;
            int remainingAccessTries =
                MaxFailedAccessAttempts - await userManager.GetAccessFailedCountAsync(user);

            errorMessage = remainingAccessTries > 3
                ? "Wrong password. Please try again."
                : $"Wrong password. Remaining tries: {remainingAccessTries}";
        }

        logger.LogWarning("[POST] Login: user {UserName} login result is not succeeded, returning view",
            user.UserName);
        return BadRequest(errorMessage);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ExternalLogin(string provider, string? returnUrl = null)
    {
        if (string.IsNullOrWhiteSpace(provider))
            throw new ArgumentNullException(nameof(provider));

        logger.LogInformation(
            "[GET] ExternalLogin: executing external login request (provider: {Provider})", provider);

        var redirectUrl = Url.Action(action: nameof(ExternalLoginCallback), controller: "Account",
            values: new { returnUrl });

        var properties = signManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ExternalLoginCallback()
    {
        var info = await signManager.GetExternalLoginInfoAsync();
        if (info is null)
        {
            logger.LogWarning(
                "[GET] ExternalLoginCallback: external login failed for a third-party reason");
            return Problem();
        }

        var email = (info.Principal.FindFirstValue(ClaimTypes.Email) ??
                     info.Principal.FindFirstValue(ClaimTypes.NameIdentifier))!;
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            var firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
            var lastName = info.Principal.FindFirstValue(ClaimTypes.Surname);

            user = new User
            {
                Email = email, UserName = email, FirstName = firstName, LastName = lastName,
                LockoutEnabled = false
            };

            await userManager.CreateAsync(user);
            logger.LogInformation(
                "[GET] ExternalLoginCallback: successfully registered new user {Email} through external login",
                user.Email);
        }

        var addingLoginResult = await userManager.AddLoginAsync(user, info);
        if (addingLoginResult.Succeeded)
        {
            logger.LogInformation(
                "[GET] ExternalLoginCallback: successfully added new login to user {Email} through external login",
                user.Email);
        }

        var result = await signManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey,
            isPersistent: false, bypassTwoFactor: true);

        if (result.Succeeded)
        {
            logger.LogInformation(
                "[GET] ExternalLoginCallback: successfully logged in user {Email} through {LoginProvider} login",
                user.Email, info.LoginProvider);


            var script = "window.opener.postMessage('authentication_successful', '*'); window.close();";
            var html = "<html><head><script type='text/javascript'>" +
                       script +
                       "</script></head><body></body></html>";
            return Content(html, "text/html");
        }

        logger.LogWarning(
            "[GET] ExternalLoginCallback: user {Email} external login result through {LoginProvider} is not succeeded, redirecting to login",
            user.Email, info.LoginProvider);
        return Problem();
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        logger.LogInformation("[GET] Logout: signing out user {Email}", User.Identity?.Name);
        await signManager.SignOutAsync();
        return Ok();
    }

    [HttpGet]
    [Authorize]
    public IActionResult GetCurrentUserId()
    {
        var currentUserId = User.GetCurrentUserId();
        return Ok(new { CurrentUserId = currentUserId });
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetCurrentUserRoles()
    {
        return Ok(new
        {
            Roles = await userManager.GetRolesAsync(
                await User.GetCurrentUserFromManagerAsync(userManager))
        });
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult IsAuthenticated()
    {
        return Ok(new { User.Identity?.IsAuthenticated });
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetUserById([FromRoute] string id)
    {
        var user = await context.Users.Where(user => user.Id.Equals(id)).Include(user => user.SocialMediaLinks).FirstOrDefaultAsync();
        if (user is null)
            throw new EntityNotFoundException();

        return Ok(user);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> BuyPremium(PaymentInfo payment)
    {
        if (!payment.Validate(out var errorMessage))
        {
            if (payment.Amount != 10)
                errorMessage = "Amount is not enough or is too big";
            
            return BadRequest(errorMessage);
        }
        
        var userId = User.GetCurrentUserId();
        var user = await context.Users.FindAsync(userId);
        if (user is null)
            throw new EntityNotFoundException();

        if (user.HasPremium)
            return BadRequest("You already have a premium");
        
        //TODO: call to payment API...
        user.HasPremium = true;
        await context.SaveChangesAsync();
        
        return Ok();
    }
    
}