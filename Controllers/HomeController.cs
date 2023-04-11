using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CRUDelicious.Models;

namespace CRUDelicious.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    // Add a private variable of type MyContext (or whatever you named your context file)
    private DishContext _context;
    public HomeController(ILogger<HomeController> logger, DishContext context)
    {
        _logger = logger;
        // When our HomeController is instantiated, it will fill in _context with context
        // Remember that when context is initialized, it brings in everything we need from DbContext
        // which comes from Entity Framework Core
        _context = context;
    }

    // Create - view form
    [HttpGet("dishes/new")]
    public IActionResult NewDish()
    {
        return View();
    }

    // Create - action
    [HttpPost("dishes/create")]
    public IActionResult CreateDish(Dish newDish)
    {
        if (ModelState.IsValid)
        {
            // We can take the Monster object created from a form submission
            // and pass the object through the .Add() method  
            // Remember that _context is our database  
            _context.Add(newDish);
            // OR _context.Monsters.Add(newMon); if we want to specify the table
            // EF Core will be able to figure out which table you meant based on the model  
            // VERY IMPORTANT: save your changes at the end! 
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        else
        {
            // Handle unsuccessful validations
            return View("NewDish");
        }
    }

    // Read all
    [HttpGet("")]
    public IActionResult Index()
    {
        // Now any time we want to access our database we use _context
        List<Dish> AllDishes = _context.Dishes.ToList();
        return View(AllDishes);
    }

    // Read one
    [HttpGet("dishes/{id}")]
    public IActionResult ShowDish(int id)
    {
        Dish? OneDish = _context.Dishes.FirstOrDefault(a => a.DishId == id);
        return View(OneDish);
    }

    //Update one - view
    [HttpGet("dishes/{DishId}/edit")]
    public IActionResult EditDish(int DishId)
    {
        Dish? DishToEdit = _context.Dishes.FirstOrDefault(i => i.DishId == DishId);

        return View(DishToEdit);
    }

    // Update one - action
    [HttpPost("dishes/{DishId}/update")]
    public IActionResult UpdateDish(int DishId, Dish UpdatedDish)
    {
        Dish? DishToUpdate = _context.Dishes.FirstOrDefault(a => a.DishId == DishId);
        if (DishToUpdate == null)
        {
            return RedirectToAction("Index");
        }
        if (ModelState.IsValid)
        {
            DishToUpdate.Name = UpdatedDish.Name;
            DishToUpdate.Chef = UpdatedDish.Chef;
            DishToUpdate.Tastiness = UpdatedDish.Tastiness;
            DishToUpdate.Calories = UpdatedDish.Calories;
            DishToUpdate.UpdatedAt = DateTime.Now;

            _context.SaveChanges();

            return RedirectToAction("Index");
        } else {
            return View("EditDish", DishToUpdate);
        }
    }

    // Delete one
    [HttpPost("dishes/{DishId}/destroy")]
    public IActionResult DestroyDish(int DishId)
    {
        Dish? DishToDestroy = _context.Dishes.SingleOrDefault(a => a.DishId == DishId);
        _context.Dishes.Remove(DishToDestroy);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }
}
