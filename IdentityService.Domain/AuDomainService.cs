using FeMs.Share;
using IdentityService.Domain.Entitys;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain
{
    public class AuDomainService
    {
        private readonly IAuRepository repository;
        private readonly IConfiguration configuration;
        private readonly JWTOptions jwtOption;
        public AuDomainService(IAuRepository repository, IConfiguration configuration)
        {
            this.repository = repository;
            this.configuration = configuration;
            this.jwtOption = configuration.GetSection("JWT").Get<JWTOptions>();
        }
        public async Task<bool> CreateWorld()
        {
            var r = await repository.CreateRoleAsyn("admin");
            if (r.Succeeded)
            {
                User user = new User("zhangsan");
                user.CreationTime = DateTime.Now;
                user.PhoneNumber = "12312341234";
                user.Email = "zhangsan@abc.com";
                await repository.CreateUserAsync(user, "123456");
                await repository.AddToRoleAsync(user, "admin");
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<(bool status,string msg)> RegisterAsync(UserDTO regUser)
        {
            bool status = false;
            string msg = string.Empty;
            User newUser = new User(regUser.UserName);
            newUser.CreationTime = DateTime.Now;
            newUser.PhoneNumber = regUser.PhoneNumber;
            newUser.Email = regUser.Email;
            
            var r = await repository.CreateUserAsync(newUser,regUser.Password);
            if(r.Succeeded)
            {
                status = true;
            }else
            {
                status = false;
                msg = "注册失败";
            }
            return  (status,msg);
        }
        private async Task<SignInResult> CheckUserNameAndPwdAsync(string userName, string password)
        {
            var user = await repository.FindByNameAsync(userName);
            if (user == null)
            {
                return SignInResult.Failed;
            }
            //CheckPasswordSignInAsync会对于多次重复失败进行账号禁用
            var result = await repository.CheckForLoginAsync(user, password, true);
            return result;
        }
        public async Task<(SignInResult Result, string accessToken,string refreshToken)> LoginAsync(string userName, string password)
        {
            
            var checkResult = await CheckUserNameAndPwdAsync(userName, password);
            if (checkResult.Succeeded)
            {
                var user = await repository.FindByNameAsync(userName);
                string accessToken = await BuildTokenAsync(user);
                string refreshToken = await BuildRefreshTokenAsync(user);
                return (SignInResult.Success, accessToken, refreshToken);
            }
            else
            {
                return (checkResult, String.Empty,string .Empty);
            }
        }
        public async Task<(SignInResult Result, string accessToken, string refreshToken)> LoginAsync(User user)
        {
            string accessToken = await BuildTokenAsync(user);
            string refreshToken = await BuildRefreshTokenAsync(user);
            return (SignInResult.Success, accessToken, refreshToken);
        }
        public async Task<string> BuildRefreshTokenAsync(User user)
        {
            var roles = await repository.GetRolesAsync(user);
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));

            claims.Add(new Claim(ClaimTypes.Role, "Refresh"));
            DateTime expires = DateTime.Now.AddSeconds(jwtOption.RefreshSeconds);
            return BuildToken(claims, expires);
        }
        private async Task<string> BuildTokenAsync(User user)
        {
            var roles = await repository.GetRolesAsync(user);
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            foreach (string role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            DateTime expires = DateTime.Now.AddSeconds(jwtOption.ExpireSeconds);
            return BuildToken(claims, expires);
        }
        string BuildToken(List<Claim> claims, DateTime expires)
        {
            
            byte[] secBytes = Encoding.UTF8.GetBytes(jwtOption.Key);
            var secKey = new SymmetricSecurityKey(secBytes);
            var credentials = new SigningCredentials(secKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(jwtOption.Issuer, jwtOption.Audience, claims: claims, expires: expires, signingCredentials: credentials);
            string jwt = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
            return jwt;
        }

    }
}
