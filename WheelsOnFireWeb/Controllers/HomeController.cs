using Microsoft.AspNetCore.Mvc;
using WheelsOnFireWeb.Messages;
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
            var registerOrderCommand = new RegisterOrderCommand(model);

            using (var rabbitMqManager = new RabbitMqManager())
            {
                rabbitMqManager.SendRegisterOrderCommand(registerOrderCommand);
            }

            return View("Thanks");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
