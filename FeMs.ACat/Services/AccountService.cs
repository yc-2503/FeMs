using FeMs.ACat.Models;
using FeMs.ACat.Utils;
using FeMs.Share;
using MediatR;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace FeMs.ACat.Services
{
    public interface IAccountService
    {
        Task<(bool status, string msg)> LoginAsync(LoginParamsType model);
        void Logout();
        Task<string> ReLoginAsync();
        Task<string> GetCaptchaAsync(string modile);
    }
    //这个类只用于身份识别及授权，
    /// <summary>
    /// PLAN A 自己管理自己的HTTPCLIENT,获取授权TOKEN或者TOKEN更新后，发布MediatR事件通知其他服务更新自己的TOKEN
    /// PLAN B 全部服务使用同一个httpclient
    /// </summary>
    public class AccountService : IAccountService
    {
         private readonly Random _random = new Random();
        private readonly AuthenticationStateProvider auProvider;

        private readonly HttpClient httpClient;
        private readonly string baseUrl;

        private readonly TokenHelper _tokenHelper;
        public AccountService(AuthenticationStateProvider auProvider, IConfiguration configuration, TokenHelper tokenHelper)
        {
            httpClient = new HttpClient();
            baseUrl = configuration.GetValue<string>("IdentityURL");
            this.auProvider = auProvider;
            this._tokenHelper = tokenHelper;
            //loginHandler.ReLoginAsync = this.ReLoginAsync;//LoginHandler loginHandler,
            //loginHandler.LoginOut = this.Logout;
            // this.baseUrl = configuration.GetValue<string>("IdentityURL");

        }

        public void Logout()
        {
            _tokenHelper.RemoveToken();
            ((ApiAuthenticationStateProvider)auProvider).MarkUserAsLoggedOut();
        }
        public async Task<string> ReLoginAsync()
        {
            string refreshToken =  _tokenHelper.GetRefreshToken();
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return string.Empty;
            }
            httpClient.DefaultRequestHeaders.Authorization = new  AuthenticationHeaderValue("Bearer", refreshToken);
            var response = await httpClient.GetAsync($"{baseUrl}Login/LoginByRefreshToken");
            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadFromJsonAsync<IdentityToken>();

                _tokenHelper.SetAccessToken(token.accessToken);
                _tokenHelper.SetRefreshToken(token.refreshToken);

                return token.accessToken;
            }
            else
            {
                return string.Empty;
            }
        }
        public async Task<(bool status,string msg)> LoginAsync(LoginParamsType model)
        {
            var response = await httpClient.PostAsJsonAsync($"{baseUrl}Login/LoginByUserNameAndPwd", new { UserName = model.UserName, Pwd = model.Password });

            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadFromJsonAsync<IdentityToken>();
                
                _tokenHelper.SetAccessToken(token.accessToken);
                _tokenHelper.SetRefreshToken(token.refreshToken);
                
                return (true,string.Empty);
            }
            else
            {
                var msg = await response.Content.ReadAsStringAsync();
                return (false,msg);
            }
        }

        public Task<string> GetCaptchaAsync(string modile)
        {
            var captcha = _random.Next(0, 9999).ToString().PadLeft(4, '0');
            return Task.FromResult(captcha);
        }
    }

}