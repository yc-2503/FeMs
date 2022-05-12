﻿using FeMs.Share;
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
        public async Task<ActionResult<string>> LoginByUserNameAndPwd(LoginByUserNameAndPwdReq req)
        {
            (var checkResult, string? token) = await auDomainService.LoginByUserNameAndPwdAsync(req.UserName, req.Pwd);
            if (checkResult.Succeeded)
            {
                return Ok(token);
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
        [Authorize]
        public async Task<ActionResult<UserDTO>> GetUserInfo()
        {
            string userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await auRepository.FindByIdAsync(userId);
            if (user == null)//可能用户注销了
            {
                return NotFound();
            }
            //出于安全考虑，不要机密信息传递到客户端
            //除非确认没问题，否则尽量不要直接把实体类对象返回给前端
            return new UserDTO() { Name = user.UserName,Phone=user.PhoneNumber,Email=user.Email };
        }
    }
    public record LoginByUserNameAndPwdReq(string UserName,string Pwd);
}