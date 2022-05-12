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
            var r = repository.CreateRoleAsyn("admin");
            if (r.IsCompletedSuccessfully)
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
        public async Task<(SignInResult Result, string? Token)> LoginByUserNameAndPwdAsync(string userName, string password)
        {
            var checkResult = await CheckUserNameAndPwdAsync(userName, password);
            if (checkResult.Succeeded)
            {
                var user = await repository.FindByNameAsync(userName);
                string token = await BuildTokenAsync(user);
                return (SignInResult.Success, token);
            }
            else
            {
                return (checkResult, null);
            }
        }
        string BuildToken(List<Claim> claims)
        {
            DateTime expires = DateTime.Now.AddSeconds(jwtOption.ExpireSeconds);
            byte[] secBytes = Encoding.UTF8.GetBytes(jwtOption.Key);
            var secKey = new SymmetricSecurityKey(secBytes);
            var credentials = new SigningCredentials(secKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(jwtOption.Issuer, jwtOption.Audience, claims: claims, expires: expires, signingCredentials: credentials);
            string jwt = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
            return jwt;
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
            return BuildToken(claims);
        }
    }
}
