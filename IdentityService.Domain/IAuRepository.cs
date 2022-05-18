using IdentityService.Domain.Entitys;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain
{
    //增删改查接口
    public interface IAuRepository
    {
        Task<IdentityResult> CreateUserAsync(User user, string password);
        Task<User> FindByNameAsync(string userName);
        Task<int> GetUsersCount();
        Task<List<User>> GetUsers(int from, int to);
        Task<User?> FindByIdAsync(string userId);
        Task<SignInResult> CheckForLoginAsync(User user, string password, bool lockoutOnFailure);
        Task<IdentityResult> ChangeUserPassword(User user, string newPasswd);
        Task<IdentityResult> AddToRoleAsync(User user, string role);
        Task<IList<string>> GetRolesAsync(User user);
        Task<IdentityResult> CreateRoleAsyn(string roleName);
        Task<Role> FindRoleAsync(string roleName);
        Task<int> GetRolesCount();
        Task<List<Role>> GetRoles(int from, int to);

    }
}
