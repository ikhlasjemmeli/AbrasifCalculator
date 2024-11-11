using Calculator.Migrations;
using Calculator.Models;
using Calculator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

public class ArticleController : Controller
{
   
    private readonly ApplicationDbContext context;

    public ArticleController(ApplicationDbContext _context)
    {
        this.context = _context;
    }


    public User GetUserInformationFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("my_1_top_2_secret_3_key_4_1234567890");
        tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        }, out SecurityToken validatedToken);

        var jwtToken = (JwtSecurityToken)validatedToken;
        var userId = jwtToken.Claims.FirstOrDefault(x => x.Type == "Id")?.Value;
        var Name = jwtToken.Claims.FirstOrDefault(x => x.Type == "Name")?.Value;
        var email = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

        int.TryParse(userId, out int Id);
        return new User
        {

            Id = Id,
            Name = Name,
            Email = email
        };

    }

    private string GetNameFromToken(string token)
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

 
   
    public IActionResult Article()
    {
        var token = HttpContext.Request.Cookies["token"];
        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var Id = GetNameFromToken(token);
         

        var articles  = context.Articles
              .Include(a => a.ArticleComponents)
              .Where(a => (a.UserId).ToString() == Id) // Filtrer par UserId
              .ToList();

        var components = context.Components.ToList();

        // Gérer le cas où l'utilisateur n'est pas trouvé (peut-être rediriger vers la page de connexion)

        var comp = new ComponentDto
        {
            articles = articles,
            Name = "",
            components = components,
           //oken = token 

        };
        if (IsTokenValid(token))
        {
            comp.token= token;
        }
        
        return View(comp);
    }

    public IActionResult Create()
    {
        var token = HttpContext.Request.Cookies["token"];
       
        if (IsTokenValid(token))
        {
            var Article = new ArticleDto
            {
                token = token
            };
            return View(Article);
            // Cette vue ne nécessite pas de modèle ou utilise ArticleDto pour l'affichage
        }
       else
        {
            return RedirectToAction("Login", "User");
        }

    }
    public IActionResult EditArticle(int id)
    {
        var token = HttpContext.Request.Cookies["token"];
        var article = context.Articles.Include(a =>a.ArticleComponents).ToList().FirstOrDefault(article => article.Id == id);
        // var Components = context.ArticleComponents.Select(c=> new ComponentDto
        // {
        //     Id= c.ComponentId,

        // });

        var name = article.Name;
        var articleDto = new ArticleDto
        {
            Nameart = name,
            Id = id,
         // token = token



        };
         if (IsTokenValid(token))
        {
            articleDto.token= token;
        }
        return View(articleDto); // Cette vue ne nécessite pas de modèle ou utilise ArticleDto pour l'affichage
    }
    [Authorize]
    [HttpPost]
    public IActionResult EditArticle([FromBody] ArticleDto articleDto)
    {
        var article = context.Articles.Include(a => a.ArticleComponents).ToList().FirstOrDefault(article => article.Id == articleDto.Id);
        
        article.Name = articleDto.Nameart;
        context.Articles.Update(article);
        context.SaveChanges();

        
        return RedirectToAction("Article","Article"); 
    }
    [Authorize]
    [HttpPost]
    public IActionResult Create( string Name)
    {
        if (Name == null)
        {
            throw new Exception("null");
        }
        if (!ModelState.IsValid)
        {
            return View(); // Passez l'articleDto à la vue Create
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
            var article = new Article
            {
                Name = Name,
                UserId = userId,
                CreationDate = DateTime.Now,
                

            };
            context.Articles.Add(article);
            context.SaveChanges();
                return Ok(new { message = "Article créé avec succès." });
                // return RedirectToAction("Article", "Article");
            }

            return Unauthorized("Token invalide.");
            // return RedirectToAction("Article", "Article"); // Retourne OK si tout se passe bien
        }
    catch (Exception ex)
    {
        // Log l'erreur pour le débogage
        Console.Error.WriteLine(ex);
        return StatusCode(500, "Erreur lors de la création de l'article.");
}


    }
    [Authorize]
    [HttpPost]
    public IActionResult EditComponent([FromBody] ComponentToAddDto comp)
    {
        if (comp == null)
        {
            Console.WriteLine("Le modèle est null");
            return BadRequest("Le modèle est null");
            throw new Exception($"{comp.Id}");
        }
        var token = HttpContext.Request.Cookies["token"];
        if (IsTokenValid(token)) { 
            var component = context.ArticleComponents.Include(c=>c.Component).ToList().FirstOrDefault(artcomp => artcomp.ComponentId == comp.Id);
            //throw new Exception($"{comp.Price}");
            component.Quantity = comp.Quantity ;
            component.Price = comp.Price ;
           // throw new Exception($"{comp.Price}");
        context.ArticleComponents.Update(component);
            
            context.SaveChanges();
            //throw new Exception($"{component.Quantity}, {component.Price}");
            //var componentDto = new ArticleDetailsViewModel
            //{
            //    Id = id,
            // ComponentName = component.Component.Name,
            // Price = component.Price,
            // Quantity = component.Quantity,

            //};
            return RedirectToAction("Details", "Article", new { id = component.ArticleId }); // Cette vue ne nécessite pas de modèle ou utilise ArticleDto pour l'affichage
        }
        else
        {
            return RedirectToAction("Login", "User");
        }
    }


    [Authorize]
    [HttpPost]
    public IActionResult Addcomponent([FromBody] ComponentToAddDto componentDto)
    {
        //var compoentId = 0;
        var existingComponent = context.Components.FirstOrDefault(comp => comp.Name == componentDto.Name);
       

        // Récupérer l'article de la base de données
        var article = context.Articles
            .Include(a => a.ArticleComponents)
            .FirstOrDefault(a => a.Id == componentDto.Id);
        var ArtComp = new ArticleComponent
        {
            ArticleId = componentDto.Id,
            ComponentId = existingComponent.Id,
            Quantity = componentDto.Quantity,
            Price = componentDto.Price
        };
        article.ArticleComponents.Add(ArtComp);
        context.SaveChanges();

        return RedirectToAction("Article", "Article");
    }
    public IActionResult AddComponent()
    {
        var token = HttpContext.Request.Cookies["token"];
        var Id = GetNameFromToken(token);
        if (IsTokenValid(token) && int.TryParse(Id, out int userId)!=null)
        {
            throw new Exception($"{userId}");
            var rawMaterials = context.Components
                     // .Include(a => a.ArticleComponents)
                     .Where(a => a.UserId == userId) // Filtrer par UserId
                    .ToList();
            var componentDto = new ComponentDto
            {
                components = rawMaterials,
                token = token
            };
            return View(componentDto);
        }
        else
        {
            return RedirectToAction("Login", "User");
        }
           // Cette vue ne nécessite pas de modèle ou utilise ComponentDto pour l'affichage
    }

  

    public IActionResult Details(int id)
    {
        
        

            var token = HttpContext.Request.Cookies["token"];
            var articleComponents = context.ArticleComponents.Where(c => c.ArticleId == id).Include(c => c.Component).Include(c => c.Article).ToList();
            var articleName = context.Articles.FirstOrDefault(c => c.Id == id).Name;

            var components = articleComponents.Select(c => new ComponentDto
            {
                Id = c.Component.Id,
                Name = c.Component.Name,  // Accéder au nom du composant à travers l'inclusion
                Quantity = c.Quantity,              // Inclure d'autres propriétés si nécessaire
                Price = c.Price,
                Tva = c.tva,
                totalpriceTTC = c.totalpriceTTC,
            })
         .ToList();

            var articleDetailsViewModel = new ArticleDetailsViewModel
            {
                ArticleName = articleName,
                ArticleId = id,
                Components = components,


            };

            if (IsTokenValid(token))
            {
                var comp = new ComponentDto
                {
                    details = articleDetailsViewModel,
                    token = token
                };
                return View(comp);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        
        

             
    }
    public IActionResult DeleteArticle()
    {
        var token = HttpContext.Request.Cookies["token"];
       
        
        
        if (IsTokenValid(token))
        {
            var articleToDelete = new ComponentDto
            {
                token = token
            };
            return View(articleToDelete);
        }
        else
        {
            return RedirectToAction("Login", "User");
        }// Cette vue ne nécessite pas de modèle ou utilise ComponentDto pour l'affichage
    }
    
    [Authorize]
    [HttpPost]
    public IActionResult DeleteArticle([FromBody] DeleteDto articletodelete  )

    {
        
        if (articletodelete.Type == "article")
        {
            var article = context.Articles.FirstOrDefault(o => o.Id == articletodelete.Id);
            
            context.Articles.Remove(article);
            context.SaveChanges();
            return RedirectToAction("Article", "Article");
        }
        if (articletodelete.Type == "component")
        {
            var component = context.ArticleComponents.FirstOrDefault(o => o.ComponentId == articletodelete.Id);

             context.ArticleComponents.Remove(component);
             context.SaveChanges();
            return RedirectToAction("Details", "Article", new { id = component.ArticleId });
        }
        else {
            return View(); 
        }
        
            
        
        
    }
    [Authorize]
    [HttpPost]
    public IActionResult UpdatePieces([FromBody] PiecesUpdateModelDto model)
    {
        var token = HttpContext.Request.Cookies["token"];



        
            if (ModelState.IsValid && IsTokenValid(token))
        {
            var article = context.Articles
                .FirstOrDefault(c => c.Id == model.Id );

            if (article == null)
            {
                return NotFound(new { success = false, message = "article not found." });
            }

            try
            {
                article.numberOfPieces = model.pieces;
               
                context.Update(article);
                context.SaveChanges();
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        return BadRequest(new { success = false, errors = ModelState });
    }



    [Authorize]
    [HttpPost]
    public IActionResult UpdateTva([FromBody] TvaUpdateModelDto model)
    {
        var token = HttpContext.Request.Cookies["token"];




        if (ModelState.IsValid && IsTokenValid(token))
          
        {
            var component = context.ArticleComponents
                .FirstOrDefault(c => c.ArticleId == model.articleId && c.ComponentId == model.componentId);

            if (component == null)
            {
                Console.WriteLine($"Composant non trouvé pour ArticleId={model.articleId}, ComponentId={model.componentId}");
                return NotFound(new { success = false, message = "Component not found." });
            }

            try
            {
                component.tva = model.Tva;
                component.totalpriceTTC = model.totalpriceTTC;
                context.Update(component);
                context.SaveChanges();

                decimal totalTTC = context.ArticleComponents
              .Where(c => c.ArticleId == model.articleId)
              .Sum(c => c.totalpriceTTC);
               
                return Ok(new { success = true , totalTtc = totalTTC });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de la mise à jour de la TVA: " + ex.Message);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        Console.WriteLine("ModelState non valide : " + JsonSerializer.Serialize(ModelState));
        return BadRequest(new { success = false, errors = ModelState });
    }


/*
    public IActionResult Statistic() 
    { 
        return View(); 
    }*/
}
