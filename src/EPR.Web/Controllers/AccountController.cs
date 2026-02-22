using EPR.Application.Services;
using EPR.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace EPR.Web.Controllers;

public class AccountController : Controller
{
    private readonly IAuthenticationService _authService;
    private readonly IDatasetService _datasetService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IAuthenticationService authService, IDatasetService datasetService, ILogger<AccountController> logger)
    {
        _authService = authService;
        _datasetService = datasetService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
    {
        try
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Username and password are required.");
                return View();
            }

            var user = await _authService.AuthenticateAsync(username, password);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View();
            }

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("IsLoggedIn", "true");

            // Clear dataset so user is prompted to select one after login
            _datasetService.ClearCurrentDataset();

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed for user {Username}", username);
            ViewData["ReturnUrl"] = returnUrl;
            ModelState.AddModelError("", "An error occurred during login. Please try again.");
            return View();
        }
    }

    [HttpPost]
    public IActionResult Logout()
    {
        _datasetService.ClearCurrentDataset();
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}









