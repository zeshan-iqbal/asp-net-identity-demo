using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Basic.Pages
{
    public class AuthenticateModel : PageModel
    {
        public async Task<IActionResult> OnGet()
        {
            var grandmasClaim = new List<Claim>()
            {
                new(ClaimTypes.Name, "Bob"),
                new(ClaimTypes.Country, "United States"),
                new("NickName", "Boby")
            };

            var drivingLicenseClaim = new List<Claim>()
            {
                new(ClaimTypes.Name, "Bob Kurt"),
                new(ClaimTypes.Country, "United States"),
                new("LicenseNumber", "4559910")
            };

            var grandmasIdentity = new ClaimsIdentity(grandmasClaim, CookieAuthenticationDefaults.AuthenticationScheme);
            var govtIdentity = new ClaimsIdentity(drivingLicenseClaim, CookieAuthenticationDefaults.AuthenticationScheme);

            var userPrinciple = new ClaimsPrincipal(new List<ClaimsIdentity>{ grandmasIdentity, govtIdentity });
            await HttpContext.SignInAsync(userPrinciple);

            return Redirect("Index");
        }
    }
}
