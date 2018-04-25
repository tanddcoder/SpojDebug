using Microsoft.AspNetCore.Identity;
using SpojDebug.Core.Constant;
using SpojDebug.Core.User;
using SpojDebug.Data.EF.Contexts;
using SpojDebug.Ultil.Reflection;
using System.Threading.Tasks;

namespace SpojDebug.Business.Logic
{
    public class SeedDataBusiness : ISeedDataBusiness
    {
        private readonly SpojDebugDbContext _spojDebugDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SeedDataBusiness(
            SpojDebugDbContext spojDebugDbContext,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _spojDebugDbContext = spojDebugDbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public void InitData()
        {
            var userTask = EnsureUser("admin@spojdebug.com", "AdminPass123!@#");
            userTask.Wait();

            var roleTask =  EnsureRole(userTask.Result, Enums.UserRole.Admin.GetDisplayName());
            roleTask.Wait();
        }

        private async Task<string> EnsureUser(string UserName, string testUserPw)
        {

            var user = await _userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                user = new ApplicationUser { UserName = UserName };
                await _userManager.CreateAsync(user, testUserPw);
            }

            return user.Id;
        }

        private async Task<IdentityResult> EnsureRole(string uid, string role)
        {
            IdentityResult IR = null;
            if (!await _roleManager.RoleExistsAsync(role))
            {
                IR = await _roleManager.CreateAsync(new IdentityRole(role));
            }
            
            var user = await _userManager.FindByIdAsync(uid);

            IR = await _userManager.AddToRoleAsync(user, role);

            return IR;
        }
    }
}
