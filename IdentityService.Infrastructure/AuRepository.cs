using IdentityService.Domain;
using IdentityService.Domain.Entitys;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Infrastructure
{
    public class AuRepository: IAuRepository
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;
        private readonly SignInManager<User> signInManager;

        public AuRepository(UserManager<User> userManager, RoleManager<Role> roleManager, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
        }

        public Task<User> FindByNameAsync(string userName)
        {
            return userManager.FindByNameAsync(userName);
        }
        public Task<IdentityResult> CreateUserAsync(User user,string password)
        {
            return this.userManager.CreateAsync(user, password);
        }
        public async Task<int> GetUsersCount()
        {
            return await this.userManager.Users.CountAsync();
        }
        public async Task<List<User>> GetUsers(int from,int to)
        {
            return await this.userManager.Users.OrderBy(u => u.Id).Skip(from).Take(to - from).ToListAsync();
        }
        //添加一个角色，不需要提前检查这个role是否存在，identity框架内部有处理
        public Task<IdentityResult>CreateRoleAsyn(string roleName)
        {
            Role role = new Role { Name = roleName };
            return roleManager.CreateAsync(role);
        }
        public async Task<IdentityResult> AddToRoleAsync(User user, string role)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                return IdentityResult.Failed(new IdentityError() { Description="角色不存在"});
            }else
            {
                return await userManager.AddToRoleAsync(user, role);
            }
        }
        public Task<IList<string>> GetRolesAsync(User user)
        {
            return userManager.GetRolesAsync(user);
        }
        public Task<Role> FindRoleAsync(string roleName)
        {
            return  roleManager.FindByNameAsync(roleName);
        }
        public Task<IdentityResult> AccessFailedAsync(User user)
        {
            return userManager.AccessFailedAsync(user);
        }
        public async Task<IdentityResult> ChangeUserPassword(User user,string newPasswd)
        {
            if(newPasswd.Length<6)
            {
                IdentityError err = new IdentityError();
                err.Description = "密码长度不能少于6";
                err.Code = "Password Invalid";
                return IdentityResult.Failed(err);
            }
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var r = await userManager.ResetPasswordAsync(user,token,newPasswd);
            return r;
        }
        public async Task<SignInResult> CheckForLoginAsync(User user, string password, bool lockoutOnFailure) =>
            await signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure);
        

        public async Task<User?> FindByIdAsync(string userId)
        {
            return await userManager.FindByIdAsync(userId);
        }

        public async Task<List<Role>> GetRoles(int from, int to)
        {
            return await this.roleManager.Roles.OrderBy(u => u.Id).Skip(from).Take(to - from).ToListAsync();
        }

        public async Task<int> GetRolesCount()
        {
            return await this.roleManager.Roles.CountAsync();
        }
    }
}
