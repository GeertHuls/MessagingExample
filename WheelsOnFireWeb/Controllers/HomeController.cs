using Microsoft.AspNetCore.Mvc;
using WheelsOnFireWeb.ViewModels;

namespace WheelsOnFireWeb.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult RegisterOrder()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RegisterOrder(OrderViewModel model)
        {
            //var registerOrderCommand = new RegisterOrderCommand(model);

            //Send RegisterOrderCommand

            return View("Thanks");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
