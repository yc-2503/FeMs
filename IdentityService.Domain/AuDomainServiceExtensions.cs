using FeMs.Share;
using IdentityService.Domain.Entitys;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace IdentityService.Domain
{
    public static class AuDomainServiceExtensions
    {
        public static AuthenticationBuilder AddJWTAuthentication(this IServiceCollection services, JWTOptions jwtOpt)
        {
            return services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOpt.Issuer,
                    ValidAudience = jwtOpt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOpt.Key))
                };
            });
        }
        //public static IdentityBuilder AddIdentityCoreService(this IServiceCollection services)
        //{
        //    IdentityBuilder identityBuilder = services.AddIdentityCore<User>(options =>
        //    {
        //       options.Password.RequireDigit = false;
        //       options.Password.RequireLowercase = false;
        //       options.Password.RequireNonAlphanumeric = false;
        //       options.Password.RequireUppercase = false;
        //       options.Password.RequiredLength = 6;

        //        //不能设定RequireUniqueEmail，否则不允许邮箱为空
        //        //options.User.RequireUniqueEmail = true;
        //        //以下两行，把GenerateEmailConfirmationTokenAsync验证码缩短
        //        options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
        //       options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
        //    });
        //    identityBuilder = new IdentityBuilder(identityBuilder.UserType, typeof(Role), services);

        //    return identityBuilder.AddEntityFrameworkStores<AuDbcontex>()
        //          .AddDefaultTokenProviders()
        //          .AddRoleManager<RoleManager<Role>>()
        //          .AddUserManager<UserManager<User>>();
        //}
    }
}
