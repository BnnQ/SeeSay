using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using SeeSay.Models.Entities;

namespace SeeSay.Utils.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetCurrentUserId(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)!;
    }

    public static Task<User> GetCurrentUserFromManagerAsync(this ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager)
    {
        var userId = claimsPrincipal.GetCurrentUserId();
        return userManager.FindByIdAsync(userId)!;
    }

}