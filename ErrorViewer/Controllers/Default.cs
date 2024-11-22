using Microsoft.AspNetCore.Mvc;

namespace ErrorViewer.Controllers;

public class Default : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}