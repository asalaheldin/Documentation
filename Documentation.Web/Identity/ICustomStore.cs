using Documentation.Web.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Documentation.Web.Identity
{
    public interface ICustomStore<TUser> : IUserStore<TUser>, IUserPasswordStore<TUser>
        where TUser : class, Microsoft.AspNet.Identity.IUser<string>
    {
        Task<IdentityResult> UpdatePassword(Guid userId, string currentPassword, string newPassword);
        Task<IdentityResult> AddPassword(Guid userId, string password);
        Task<ApplicationUser> FindByPasswordAsync(string userName, string passwordHash);
    }
}