using DineEase.Data;
using DineEase.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DineEase.Controllers
{
	public class IngredientController : Controller
	{
		private Repository<Ingredient> ingredients;

		public IngredientController(ApplicationDbContext context)
		{
			this.ingredients = new Repository<Ingredient>(context);
		}

		public async Task<IActionResult> Index()
		{
			return View(await ingredients.GetAllAsync());
		}

		public async Task<IActionResult> Details(int id)
		{
			var ingredient = await ingredients.GetByIdAsync(id, new QueryOptions<Ingredient>() { Includes = "ProductIngredients.Product" });
			if (ingredient == null)
			{
				return NotFound();
			}
			return View(ingredient);
		}

		//Ingredient/Create
		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("IngredientId, Name")] Ingredient ingredient)
		{
			if (ModelState.IsValid)
			{
				await ingredients.AddAsync(ingredient);
				return RedirectToAction("Index");
			}
			return View(ingredient);
		}
	}
}
