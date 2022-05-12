using IdentityService.Domain;
using IdentityService.Domain.Entitys;
using Microsoft.AspNetCore.Identity;
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

        public AuRepository(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        
        public Task<User> FindByNameAsync(string userName)
        {
            return userManager.FindByNameAsync(userName);
        }
        public Task<IdentityResult>CreateUserAsync(User user,string password)
        {
            return this.userManager.CreateAsync(user, password);
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
        public async Task<SignInResult> CheckForLoginAsync(User user, string password, bool lockoutOnFailure)
        {
            if (await userManager.IsLockedOutAsync(user))
            {
                return SignInResult.LockedOut;
            }
            else
            {
                if (await userManager.CheckPasswordAsync(user, password))
                {
                    return SignInResult.Success;
                }
                else
                {
                    if (lockoutOnFailure)
                    {
                        await AccessFailedAsync(user);//失败次数++,数据库操作失败就失败了，这里以后要加上日志
                    }
                    return SignInResult.Failed;
                }
            }
        }

        public async Task<User?> FindByIdAsync(string userId)
        {
            return await userManager.FindByIdAsync(userId);
        }
    }
}
