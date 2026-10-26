using System.Security.Claims;
using System.Text.Json;
using EmployeeHub.Mvc.Models;
using EmployeeHub.Mvc.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeHub.Mvc.Controllers;

public class AccountController : Controller
{
    private readonly AuthApiClient _authApiClient;

    public AccountController(AuthApiClient authApiClient)
    {
        _authApiClient = authApiClient;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        return View(new LoginViewModel());
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        LoginViewModel model,
        string? returnUrl = null,
        CancellationToken cancellationToken = default)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        LoginApiResponse response;

        try
        {
            response = await _authApiClient.LoginAsync(
                model.Username,
                model.Password,
                cancellationToken);
        }
        catch (Exception ex) when (
            ex is HttpRequestException
            or JsonException
            or TaskCanceledException)
        {
            ModelState.AddModelError(
                string.Empty,
                "Unable to communicate with EmployeeHub API. " +
                "Please try again.");

            return View(model);
        }

        if (!response.Success ||
            string.IsNullOrWhiteSpace(response.Token))
        {
            ModelState.AddModelError(
                string.Empty,
                string.IsNullOrWhiteSpace(response.Message)
                    ? "Login failed. Please check your credentials."
                    : response.Message);

            return View(model);
        }

        var claims = new List<Claim>
        {
            new(
                ClaimTypes.Name,
                response.Username ?? model.Username),

            new(
                "EmployeeHubApiToken",
                response.Token)
        };

        if (!string.IsNullOrWhiteSpace(response.Role))
        {
            claims.Add(
                new Claim(ClaimTypes.Role, response.Role));
        }

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal);

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToAction("Index", "Employees");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}