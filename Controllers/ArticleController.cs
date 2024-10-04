﻿using Calculator.Migrations;
using Calculator.Models;
using Calculator.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class ArticleController : Controller
{
    private readonly ApplicationDbContext context;

    public ArticleController(ApplicationDbContext _context)
    {
        this.context = _context;
    }

    public IActionResult Article()
    {
        var articles = context.Articles.Include(a => a.ArticleComponents).ToList();
        var comp = new ComponentDto
        {
            articles = articles,
            Name = "",
            
        };
        
        return View(comp);
    }

    public IActionResult Create()
    {
        return View(); // Cette vue ne nécessite pas de modèle ou utilise ArticleDto pour l'affichage
    }
    public IActionResult EditArticle(int id)
    {
        var article = context.Articles.Include(a =>a.ArticleComponents).ToList().FirstOrDefault(article => article.Id == id);
        // var Components = context.ArticleComponents.Select(c=> new ComponentDto
        // {
        //     Id= c.ComponentId,

        // });

        var name = article.Name;
        var articleDto = new ArticleDto
        {
            Nameart = name,
            Id = id


        };
        return View(articleDto); // Cette vue ne nécessite pas de modèle ou utilise ArticleDto pour l'affichage
    }

    [HttpPost]
    public IActionResult EditArticle(int Id, string Nameart)
    {
        var article = context.Articles.Include(a => a.ArticleComponents).ToList().FirstOrDefault(article => article.Id == Id);
        
        article.Name = Nameart;
        context.Articles.Update(article);
        context.SaveChanges();

        
        return RedirectToAction("Article","Article"); 
    }
    [HttpPost]
    public IActionResult Create(string Nameart)
    {
        if (!ModelState.IsValid)
        {
            return View(); // Passez l'articleDto à la vue Create
        }

        var article = new Article
        {
            Name = Nameart,
        };

        context.Articles.Add(article);
        context.SaveChanges();

        return RedirectToAction("Article", "Article");
    }

    public IActionResult EditComponent(int id, int Quantity, decimal Price)
    {
        var component = context.ArticleComponents.Include(c=>c.Component).ToList().FirstOrDefault(artcomp => artcomp.ComponentId == id);

        component.Quantity = Quantity;
        component.Price = Price;
        context.ArticleComponents.Update(component);
        context.SaveChanges();
        //var componentDto = new ArticleDetailsViewModel
        //{
        //    Id = id,
        // ComponentName = component.Component.Name,
        // Price = component.Price,
        // Quantity = component.Quantity,

        //};
        return RedirectToAction("Details", "Article", new { id = component.ArticleId }); // Cette vue ne nécessite pas de modèle ou utilise ArticleDto pour l'affichage
    }

    public IActionResult AddComponent()
    {
        return View(); // Cette vue ne nécessite pas de modèle ou utilise ComponentDto pour l'affichage
    }

    [HttpPost]
    public IActionResult Addcomponent(ComponentDto componentDto, int articleId)
    {
        var compoentId = 0;
      var existingComponent = context.Components.FirstOrDefault(comp => comp.Name == componentDto.Name);
        if (existingComponent == null)
        {
            var component = new Component
            {
                Name = componentDto.Name,
            };
       

        context.Components.Add(component);
        context.SaveChanges();
            compoentId = component.Id;
        }
        else
        {
            compoentId = existingComponent.Id;
        }

        // Récupérer l'article de la base de données
        var article = context.Articles
            .Include(a => a.ArticleComponents)
            .FirstOrDefault(a => a.Id == articleId);
        var ArtComp = new ArticleComponent
        {
            ArticleId = articleId,
            ComponentId = compoentId,
            Quantity =componentDto.Quantity,
            Price = componentDto.Price
        };
        article.ArticleComponents.Add(ArtComp);
        context.SaveChanges();

        return RedirectToAction("Article", "Article");
    }

    public IActionResult Details(int id)
    {
        var articleComponents = context.ArticleComponents.Where(c => c.ArticleId == id).Include(c=>c.Component).Include(c => c.Article).ToList() ;
        var articleName = context.Articles.FirstOrDefault(c => c.Id == id).Name;
        var components = articleComponents.Select(c => new ComponentDto
     {
        Id=c.Component.Id,
         Name = c.Component.Name,  // Accéder au nom du composant à travers l'inclusion
         Quantity = c.Quantity,              // Inclure d'autres propriétés si nécessaire
         Price = c.Price
     })
     .ToList();

        var articleDetailsViewModel = new ArticleDetailsViewModel
        {
            ArticleName = articleName,
            Components = components

        };
        return View(articleDetailsViewModel); 
    }
    public IActionResult DeleteArticle()
    {
        return View(); // Cette vue ne nécessite pas de modèle ou utilise ComponentDto pour l'affichage
    }
    [HttpPost]
    public IActionResult DeleteArticle( int articleId, string type )
    {   if (type == "article")
        {
            var article = context.Articles.FirstOrDefault(o => o.Id == articleId);
            
            context.Articles.Remove(article);
            context.SaveChanges();
            return RedirectToAction("Article", "Article");
        }
        if (type == "component")
        {
            var component = context.ArticleComponents.FirstOrDefault(o => o.ComponentId == articleId);

             context.ArticleComponents.Remove(component);
             context.SaveChanges();
            return RedirectToAction("Details", "Article", new { id = component.ArticleId });
        }
        else {
            return View(); 
        }
        
            
        
        
    }
}