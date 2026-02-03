using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GoldContest.Pages.Admin;

public class LoginModel : PageModel
{
    private readonly IConfiguration _config;

    public LoginModel(IConfiguration config)
    {
        _config = config;
    }

    [BindProperty]
    public string Password { get; set; } = "";

    public IActionResult OnPost()
    {
        // ✅ CORRECT KEY PATH
        var adminPassword = _config["AdminSettings:Password"];

        if (string.IsNullOrEmpty(adminPassword))
        {
            ModelState.AddModelError("", "Admin password not configured");
            return Page();
        }

        if (Password == adminPassword)
        {
            HttpContext.Session.SetString("IsAdmin", "true");
            return RedirectToPage("/Admin/Index");
        }

        ModelState.AddModelError("", "Invalid password");
        return Page();
    }
}