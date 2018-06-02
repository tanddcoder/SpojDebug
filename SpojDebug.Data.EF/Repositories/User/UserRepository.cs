using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SpojDebug.Core.User;
using SpojDebug.Data.Repositories.User;

namespace SpojDebug.Data.EF.Repositories.User
{
    public class UserRepository : IUserRepository
    {
        //private readonly SpojDebugDbContext _spojDebugDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public IQueryable<ApplicationUser> Get(Expression<Func<ApplicationUser, bool>> filter = null)
        {
            var query = _userManager.Users;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return _userManager.Users;
        }

        public string GetUserId(ClaimsPrincipal user)
        {
            var userId = _userManager.GetUserId(user);
            return userId;
        }

        public ApplicationUser GetById(object id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Insert(ApplicationUser entity)
        {
            var result =  await _userManager.CreateAsync(entity);

            return result.Succeeded;
        }

        public void Remove(ApplicationUser entityToDelete)
        {
            throw new NotImplementedException();
        }

        public void Update(ApplicationUser entity, params Expression<Func<ApplicationUser, object>>[] changedProperties)
        {
            throw new NotImplementedException();
        }

        public int SaveChanges()
        {
            throw new NotImplementedException();
        }

        public void InsertRange(IEnumerable<ApplicationUser> entities)
        {
            throw new NotImplementedException();
        }
    }
}
