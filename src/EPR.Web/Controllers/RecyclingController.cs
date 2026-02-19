using Microsoft.AspNetCore.Mvc;
using EPR.Web.Attributes;

namespace EPR.Web.Controllers;

[Authorize]
public class RecyclingController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}









