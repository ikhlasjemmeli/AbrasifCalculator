using Calculator.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
namespace Calculator.Controllers
{
   // [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        private bool IsTokenValid(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("my_1_top_2_secret_3_key_4_1234567890"); // Utilisez la même clé que pour la génération du token

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero // Optionnel : supprimez le délai d'expiration
                }, out SecurityToken validatedToken);

                return true; // Token est valide
            }
            catch
            {
                return false; // Token n'est pas valide
            }
        }

   


        public IActionResult Index()
        {
            var token = HttpContext.Request.Cookies["token"];

            var name = HttpContext.Request.Cookies["Name"];
           
            

            if (!string.IsNullOrEmpty(token) && IsTokenValid(token)) // Vérifiez la validité du token
            {
                ViewBag.UserName = name;
                return View();   // Affichez la vue si le token est valide
            }
            return RedirectToAction("Login", "User"); // Redirigez vers la page de connexion si le token est invalide
           
        }



        public async Task<IActionResult> LogOut()
        {
            Response.Cookies.Delete("token");
            Response.Cookies.Delete("Name");
            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
           
            return RedirectToAction("Login","User");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
