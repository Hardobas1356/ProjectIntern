#nullable disable

using InternSolution.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ProjectIntern.Areas.Identity.Pages.Account.Manage;

public class TwoFactorAuthenticationModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<TwoFactorAuthenticationModel> _logger;

    public TwoFactorAuthenticationModel(
        UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<TwoFactorAuthenticationModel> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }
    public bool HasAuthenticator { get; set; }
    public int RecoveryCodesLeft { get; set; }

    [BindProperty]
    public bool Is2faEnabled { get; set; }
    public bool IsMachineRemembered { get; set; }
    [TempData]
    public string StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        // 1. Safety Check: If 2FA is somehow enabled for this user, disable it automatically
        if (await _userManager.GetTwoFactorEnabledAsync(user))
        {
            await _userManager.SetTwoFactorEnabledAsync(user, false);
            _logger.LogInformation($"Automatically disabled 2FA for user '{user.Id}' upon accessing settings.");
        }

        // 2. Hard Block: Set a status message and bounce them back to the main Profile page
        StatusMessage = "Two-Factor Authentication is disabled for this organization.";
        return RedirectToPage("./Index");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Block any malicious or direct POST form requests to this page
        return BadRequest("Two-factor authentication features are disabled.");
    }
}
