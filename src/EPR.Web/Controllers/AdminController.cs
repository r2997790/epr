using Microsoft.AspNetCore.Mvc;
using EPR.Web.Attributes;

namespace EPR.Web.Controllers;

[Authorize]
public class AdminController : Controller
{
    public IActionResult Themes()
    {
        ViewData["Title"] = "Themes";
        return View();
    }

    public IActionResult License()
    {
        ViewData["Title"] = "License Management";
        return View();
    }

    public IActionResult Legal()
    {
        ViewData["Title"] = "Legal Notices";
        return View();
    }
}
