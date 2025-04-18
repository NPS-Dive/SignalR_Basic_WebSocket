using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SignalR.Pages;

[AllowAnonymous]
public class LoginModel : PageModel
{
    [BindProperty]
    [Required]
    public string Username { get; set; }

    [BindProperty]
    [Required]
    public string Password { get; set; }

    public string ReturnUrl { get; set; }

    public void OnGet ( string returnUrl = null )
    {
        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync ( string returnUrl = null )
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Hardcoded for demo; replace with database in production
        if (Username != "amir123" || Password != "Amir@123")
        {
            ModelState.AddModelError(string.Empty, "Invalid username or password");
            return Page();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, Username),
            new Claim(ClaimTypes.Role, "chatSupport")
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var properties = new AuthenticationProperties
        {
            RedirectUri = returnUrl ?? Url.Content("~/ChatSupportAgent")
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            properties);

        return LocalRedirect(properties.RedirectUri);
    }
}