using FeMs.Share;
using IdentityService.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace IdentityService.WebAPI.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly AuDomainService auDomainService;
        private readonly IAuRepository auRepository;
        public LoginController(AuDomainService auDomainService, IAuRepository auRepository)
        {
            this.auDomainService = auDomainService;
            this.auRepository = auRepository;
        }
        [HttpPost]
        public async Task<IActionResult> CreateWorld()
        {
            var r = await auDomainService.CreateWorld();
            if (r)
            {
                return Ok();
            }
            else
            {
                return StatusCode((int)HttpStatusCode.BadRequest, "We Are the world");
            }
        }
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<string>> GetTest()
        {
            var req = new LoginByUserNameAndPwdReq("adm", "fwe");
            return Ok("hello world");
        }
        //( Microsoft.AspNetCore.Identity.SignInResult Result, string Token)
        [HttpPost]
        public async Task<ActionResult<IdentityToken>> LoginByUserNameAndPwd(LoginByUserNameAndPwdReq req)
        {
            (var checkResult, string accessToken, string refreshToken) = await auDomainService.LoginAsync(req.UserName, req.Pwd);
            if (checkResult.Succeeded)
            {
                return Ok(new IdentityToken(accessToken,refreshToken));
            }
            else
            {
                string msg = "用户名或密码错误";
                if (checkResult.IsLockedOut)
                {
                    msg = "用户被锁定";
                }
                else if (checkResult.IsNotAllowed)
                {
                    msg = "用户名未激活";
                }
                return StatusCode((int)HttpStatusCode.BadRequest, msg);
            }
         }
        [HttpGet]
        [Authorize (Roles = "Refresh")]
        public async Task<ActionResult<IdentityToken>> LoginByRefreshToken()
        {
            string userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await auRepository.FindByIdAsync(userId);
            if (user == null)//可能用户注销了
            {
                return NotFound();
            }
            (var checkResult, string accessToken,string refreshToken) = await auDomainService.LoginAsync(user);
            if (checkResult.Succeeded)
            {
                return Ok(new IdentityToken(accessToken, refreshToken));
            }
            else
            {
                string msg = "用户名或密码错误";
                if (checkResult.IsLockedOut)
                {
                    msg = "用户被锁定";
                }
                else if (checkResult.IsNotAllowed)
                {
                    msg = "用户名未激活";
                }
                return StatusCode((int)HttpStatusCode.BadRequest, msg);
            }
        }
        [HttpPost]
        public async Task<ActionResult<string>> RegisterAsync(UserDTO user)
        {
            var r = await auDomainService.RegisterAsync(user);
            if(r.status)
            {
                return Ok();
            }else
            {
                return StatusCode((int)HttpStatusCode.BadRequest, r.msg);
            }
        }

        [HttpGet]
        [Authorize]
        //获取当前用户的信息
        public async Task<ActionResult<UserDTO>> GetCurrentUserInfo()
        {
            string userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await auRepository.FindByIdAsync(userId);
            if (user == null)//可能用户注销了
            {
                return NotFound();
            }
            //出于安全考虑，不要机密信息传递到客户端
            //除非确认没问题，否则尽量不要直接把实体类对象返回给前端
            return new UserDTO() { Id = user.Id.ToString(), UserName = user.UserName,PhoneNumber=user.PhoneNumber,Email=user.Email };
        }
    }
    public record LoginByUserNameAndPwdReq(string UserName,string Pwd);
}
