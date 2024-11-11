using Calculator.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Calculator.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace Calculator.Controllers
{
    public class RawMaterialController : Controller
    {
        private readonly ApplicationDbContext context;

        public RawMaterialController(ApplicationDbContext _context)
        {
            this.context = _context;
        }

        public string GetNameFromToken(string token)
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
                    ClockSkew = TimeSpan.Zero // Supprimez le délai d'expiration
                }, out SecurityToken validatedToken);

                var jwtToken = validatedToken as JwtSecurityToken;

                // Utilisez ClaimTypes.Name pour récupérer la claim "Name"
                var nameClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == "Id");
                string name = nameClaim?.Value; // Récupérez la valeur

                return name; // Retournez le nom
            }
            catch (Exception ex)
            {
                // Optionnel : loggez l'exception pour le débogage
                Console.WriteLine($"Token validation failed: {ex.Message}");
                return null; // Token n'est pas valide
            }
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
        public IActionResult RawMaterial()
        {
            var token = HttpContext.Request.Cookies["token"];
            var Id = GetNameFromToken(token);
            if (IsTokenValid(token) && int.TryParse(Id, out int userId))
            {
                

                var rawMaterials = context.Components
                    // .Include(a => a.ArticleComponents)
                     .Where(a => a.UserId == userId) // Filtrer par UserId
                    .ToList();

                var rawmat = new ListComponentDto
                {
                    Components = rawMaterials,
                    token = token
                };



                return View(rawmat);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        

        // GET: RawMaterial/Create
        public IActionResult AddRawMaterial()
        {
            var token = HttpContext.Request.Cookies["token"];
            var rawmaterial = new RawMaterialDto
            {

            };
            if (IsTokenValid(token))
            {

                rawmaterial.token = token;
                // Cette vue ne nécessite pas de modèle ou utilise ArticleDto pour l'affichage
            }
            return View(rawmaterial); // Cette vue ne nécessite pas de modèle ou utilise ComponentDto pour l'affichage
        }

        [Authorize]
        [HttpPost]
        public IActionResult AddRawMaterial([FromBody]  RawMaterialDto rawMaterialDto)
    {
            if (rawMaterialDto ==null)
            {
                throw new Exception("yes");
            }
           
            try
            {

                var token = HttpContext.Request.Cookies["token"];
               var Id = GetNameFromToken(token);

                // Récupérer l'ID de l'utilisateur à partir des claims
                // var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                //var user = context.Users.FirstOrDefault(u => u.Email == userEmail);

                if (int.TryParse(Id, out int userId) && IsTokenValid(token))
                {
                    var existingComponent = context.Components.FirstOrDefault(comp => comp.Name == rawMaterialDto.Name);
                    if (existingComponent == null)
                    {
                        var component = new Component
                        {
                            Name = rawMaterialDto.Name,
                            Unit = rawMaterialDto.Unit,
                            UserId = userId
                        };


                        context.Components.Add(component);
                        context.SaveChanges();

                    }





                }


                return RedirectToAction("RawMaterial", "RawMaterial"); // Retourne OK si tout se passe bien
            }
            catch (Exception ex)
            {
                // Log l'erreur pour le débogage
                Console.Error.WriteLine(ex);
                return StatusCode(500, "Erreur lors de la création de la matiere premiere.");
            }

    }

  
        public IActionResult EditRawMaterial(int Id)
        {
            
            var token = HttpContext.Request.Cookies["token"];

            //if (list != null)
            //{
            //    throw new Exception($"{list.Id}");
            //}

            if (IsTokenValid(token))
            {
                
                var RawMaterials = context.Components.FirstOrDefault(rawmaterial => rawmaterial.Id == Id);
               // throw new Exception($"{RawMaterials.Name}");
                var rawMaterialToEdit = new ListComponentDto
                {
                    Name= RawMaterials.Name,
                    Id = RawMaterials.Id,
                    Unit = RawMaterials.Unit,
                    token = token
                };
                
                //EditRawMaterials(list.Id, list.Name, list.Unit);
                // return RedirectToAction("EditRawMaterial", "RawMaterial");
                return View(rawMaterialToEdit);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult EditRawMaterials([FromBody]  ListComponentDto list )
        {
            var component = context.Components.FirstOrDefault(comp => comp.Id == list.Id);
            component.Name = list.Name;
            component.Unit = list.Unit;
            context.Components.Update(component);
            context.SaveChanges();
            return RedirectToAction("RawMaterial", "RawMaterial");
        }



        // GET: RawMaterial/Delete/5

        public IActionResult DeleteRawMaterial(int id)
        {
            var token = HttpContext.Request.Cookies["token"];



            if (IsTokenValid(token))
            {
                var rawMaterialToDelete = new ListComponentDto
                {
                    token = token
                };

                DeleteRawMaterials(id);
                return RedirectToAction("RawMaterial", "RawMaterial");
                // return View(rawMaterialToDelete);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
            
            
            
        }
        [Authorize]
        [HttpDelete]
        public IActionResult DeleteRawMaterials(int rawmaterialId)
        {
            var token = HttpContext.Request.Cookies["token"];



            if (IsTokenValid(token))
            {
                //throw new Exception($"{rawmaterialId}");
                var rawMaterial = context.Components.FirstOrDefault(o => o.Id == rawmaterialId);

                context.Components.Remove(rawMaterial);
                context.SaveChanges();
                return RedirectToAction("RawMaterial", "RawMaterial");
            }
            else
            {
                return RedirectToAction("Login", "User");
            }



        }


    }
}
