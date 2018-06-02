using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using SpojDebug.Core.User;

namespace SpojDebug.Data.Repositories.User
{
    public interface IUserRepository
    {
        IQueryable<ApplicationUser> Get(
            Expression<Func<ApplicationUser, bool>> filter = null);

        string GetUserId(ClaimsPrincipal user);

        ApplicationUser GetById(object id);

        Task<bool> Insert(ApplicationUser entity);

        void Remove(ApplicationUser entityToDelete);

        void Update(ApplicationUser entity, params Expression<Func<ApplicationUser, object>>[] changedProperties);

        int SaveChanges();

        void InsertRange(IEnumerable<ApplicationUser> entities);
    }
}
