using GITA.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

public class CustomSignInManager : SignInManager<ApplicationUser>
{
    public CustomSignInManager(UserManager<ApplicationUser> userManager,
                               IHttpContextAccessor contextAccessor,
                               IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
                               IOptions<IdentityOptions> optionsAccessor,
                               ILogger<SignInManager<ApplicationUser>> logger,
                               IAuthenticationSchemeProvider schemes,
                               IUserConfirmation<ApplicationUser> confirmation)
        : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
    {
    }

    public override async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
    {
        var user = await UserManager.FindByNameAsync(userName);
        if (user != null && user.IsBanned)
        {
            return SignInResult.NotAllowed;
        }

        return await base.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
    }
}
