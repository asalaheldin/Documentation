using Documentation.Data.DAL.Intefraces;
using Documentation.Data.Entities;
using Documentation.Web.Helper;
using Documentation.Web.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Documentation.Web.Identity
{
    public class DocumentationUserStore : ICustomStore<ApplicationUser>
    {
        private readonly IRepository<User> _userRepo;
        public DocumentationUserStore(IRepository<User> userRepo)
        {
            _userRepo = userRepo;
        }
        public async Task CreateAsync(ApplicationUser user)
        {
            try
            {
                _userRepo.Insert(new User() {
                    UserName = user.Email,
                    Password = user.PasswordHash,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                });
            }
            catch (Exception ex)
            { }
        }

        public async Task DeleteAsync(ApplicationUser user)
        {
            var dbUser = _userRepo.FindBy(x => x.UserName.Trim().ToLower() == user.Email.Trim().ToLower());
            if (dbUser != null)
                _userRepo.Delete(dbUser);

        }

        public async Task<ApplicationUser> FindByIdAsync(string userId)
        {
            if (Guid.TryParse(userId, out Guid Id))
            {
                var dbUser = _userRepo.GetById(Id);
                if (dbUser != null)
                    return new ApplicationUser()
                    {
                        Id = dbUser.Id.ToString(),
                        Email = dbUser.UserName,
                        UserName = dbUser.UserName,
                        FirstName = dbUser.FirstName,
                        LastName = dbUser.LastName,
                        PasswordHash = dbUser.Password
                        
                    };
            }
            return null;
        }

        public async Task<ApplicationUser> FindByNameAsync(string userName)
        {
            var dbUser = _userRepo.FindBy(x => x.UserName.Trim().ToLower() == userName.Trim().ToLower()).FirstOrDefault();
            if (dbUser != null)
            {
                return new ApplicationUser()
                {
                    Id = dbUser.Id.ToString(),
                    UserName = dbUser.UserName,
                    Email = dbUser.UserName,
                    FirstName = dbUser.FirstName,
                    LastName = dbUser.LastName,
                    PasswordHash = dbUser.Password
                };
            }
            return null;
        }
        public async Task<ApplicationUser> FindByPasswordAsync(string userName, string passwordHash)
        {
            var dbUser = _userRepo.FindBy(x => x.UserName.Trim().ToLower() == userName.Trim().ToLower() &&  x.Password == passwordHash).FirstOrDefault();
            if (dbUser != null)
            {
                return new ApplicationUser()
                {
                    Id = dbUser.Id.ToString(),
                    UserName = dbUser.UserName,
                    Email = dbUser.UserName,
                    FirstName = dbUser.FirstName,
                    LastName = dbUser.LastName,
                    PasswordHash = dbUser.Password
                };
            }
            return null;
        }

        public async Task UpdateAsync(ApplicationUser user)
        {
            try
            {
                var dbUser = _userRepo.FindBy(x => x.UserName.Trim().ToLower() == user.Email.Trim().ToLower()).FirstOrDefault();
                if(dbUser != null)
                {
                    dbUser.Password = user.PasswordHash;
                    dbUser.FirstName = user.FirstName;
                    dbUser.LastName = user.LastName;
                    _userRepo.Update(dbUser, dbUser.Id);
                }
            }
            catch (Exception ex)
            { }
        }

        public void Dispose()
        {
        }

        public async Task<string> GetPasswordHashAsync(ApplicationUser user)
        {
            var dbUser = _userRepo.FindBy(x => x.UserName.Trim().ToLower() == user.Email.Trim().ToLower()).FirstOrDefault();
            return dbUser?.Password;
        }

        public async Task<bool> HasPasswordAsync(ApplicationUser user)
        {
            var dbUser = _userRepo.FindBy(x => x.UserName.Trim().ToLower() == user.Email.Trim().ToLower()).FirstOrDefault();
            return !string.IsNullOrEmpty(dbUser?.Password);
        }

        public async Task SetPasswordHashAsync(ApplicationUser user, string passwordHash)
        {
            user.PasswordHash = passwordHash;
            await UpdateAsync(user);
        }
        public async Task<IdentityResult> UpdatePassword(Guid userId, string currentPassword, string newPassword)
        {
            var dbUser = _userRepo.FindBy(x => x.Id == userId && x.Password == currentPassword).FirstOrDefault();
            if (dbUser != null)
            {
                dbUser.Password = newPassword;
                var result = _userRepo.Update(dbUser, dbUser.Id);
                if (result > 0)
                    return IdentityResult.Success;
                else
                {
                    string[] errors = new string[] { "Something happened while saving the data please try again later." };
                    return IdentityResult.Failed(errors);
                }
            }
            else
            {
                string[] errors = new string[] { "The password is wrong or this account is not exist as individual." };
                return IdentityResult.Failed(errors);
            }
        }
        public async Task<IdentityResult> AddPassword(Guid userId, string password)
        {
            var dbUser = _userRepo.FindBy(x => x.Id == userId).FirstOrDefault();
            if (dbUser != null)
            {
                dbUser.Password = password;
                var result = _userRepo.Update(dbUser, dbUser.Id);
                if (result > 0)
                    return IdentityResult.Success;
                else
                {
                    string[] errors = new string[] { "Something happened while saving the data please try again later." };
                    return IdentityResult.Failed(errors);
                }
            }
            else
            {
                string[] errors = new string[] { "This account is not exist as individual." };
                return IdentityResult.Failed(errors);
            }
        }
    }
}