using Microsoft.AspNetCore.Builder;
using SeeSay.Utils.Extensions;

var builder = WebApplication.CreateBuilder(args);
var app = builder.ConfigureServices().Build();

app.Configure();
app.Run();
