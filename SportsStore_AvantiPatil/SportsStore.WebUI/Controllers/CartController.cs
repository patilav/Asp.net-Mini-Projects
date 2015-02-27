using System.Linq;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Models;

namespace SportsStore.WebUI.Controllers
{
    public class CartController : Controller
    {
        private IProductRepository repository;

        public CartController(IProductRepository repo)
        {
            repository = repo;
        }

        public ViewResult Index(Cart cart, string returnUrl)
        {
            return View(new CartIndexViewModel {
                Cart = cart,
                ReturnUrl = returnUrl
            });
        }

        public RedirectToRouteResult AddToCart(Cart cart, int productId, string returnUrl, int quantity = 1)
        {
            var product = repository.Products.FirstOrDefault(prod => prod.ProductId == productId);
            if (product != null)
            {
                cart.AddItem(product, quantity);
            }
            return RedirectToAction("Index", new { returnUrl });
        }

        public RedirectToRouteResult RemoveFromCart(Cart cart, int productId, string returnUrl)
        {
            var productInCart = cart.Lines.FirstOrDefault(prod => prod.Product.ProductId == productId);

            if (productInCart != null)
            {
                cart.RemoveLine(productInCart.Product);
            }
            return RedirectToAction("Index", new { returnUrl });
        }

		public RedirectToRouteResult UpdateQuantity(Cart cart, int productId, int quantity, string returnUrl)
		{
            return RedirectToAction("Index", new { returnUrl });
		}

        public PartialViewResult Summary(Cart cart)
        {
            return PartialView(cart);
        }

		[HttpGet]
        public ViewResult Checkout()
        {
            return View(new ShippingDetails());
        }

		[HttpPost]
		public ViewResult Checkout(Cart cart, ShippingDetails model)
		{
            if (cart.Lines.Count() == 0)
            {
                ModelState.AddModelError("Error", "Hey add some items! The cart is currently empty!");
            }
            if (ModelState.IsValid)
            {
                cart.Clear();
                return View("Completed");
            }
            return View(model);
		}

		public ViewResult Completed(Cart cart)
		{
			return View();
		}
	}
}