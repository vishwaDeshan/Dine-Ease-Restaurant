using DineEase.Data;
using DineEase.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DineEase.Controllers
{
	public class ProductController : Controller
	{
		private Repository<Product> products {  get; set; }
		private Repository<Ingredient> ingredients {  get; set; }
		private Repository<Category> categories {  get; set; }

		public ProductController(ApplicationDbContext context)
		{
			products = new Repository<Product>(context);
			categories = new Repository<Category>(context);
			ingredients = new Repository<Ingredient>(context);
		}

		public async Task<IActionResult> Index()
		{
			return View(await products.GetAllAsync());
		}

		public async Task<IActionResult> AddEdit(int id)
		{
			ViewBag.Ingredient = ingredients.GetAllAsync();
			ViewBag.Category = categories.GetAllAsync();

			if (id == 0)
			{
				ViewBag.Operation = "Add";
				return View(new Product());
			}
			else
			{
				ViewBag.Operation = "Edit";
				var product = await products.GetByIdAsync(id, new QueryOptions<Product> { Includes = "ProductIngredients.Ingredient" });
				if (product == null)
				{
					return NotFound();
				}
				return View(product);
			}
		}
	}
}
