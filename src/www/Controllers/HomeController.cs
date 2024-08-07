using Microsoft.AspNetCore.Mvc;
using MawMvcApp.ViewModels.Navigation;

namespace MawMvcApp.Controllers;

[Route("")]
public class HomeController
    : MawBaseController<HomeController>
{
    public HomeController(ILogger<HomeController> log)
        : base(log)
    {

    }

    [HttpGet("")]
    public IActionResult Index()
    {
        ViewBag.NavigationZone = NavigationZone.Home;

        return View();
    }

    [HttpGet("privacy")]
    public IActionResult Privacy()
    {
        ViewBag.NavigationZone = NavigationZone.Home;

        return View();
    }
}
