using FeMs.ACat.Models;
using FeMs.Share;
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
        Task<(bool status, string? msg)> LoginAsync(LoginParamsType model);
        void Logout();
        Task<string> GetCaptchaAsync(string modile);
    }

    public class AccountService : IAccountService
    {
        private readonly Random _random = new Random();
        private readonly AuthenticationStateProvider auProvider;
        private readonly HttpClient httpClient;
        private readonly string baseUrl;
        public AccountService(HttpClient httpClient, AuthenticationStateProvider auProvider, IConfiguration configuration)
        {
            this.httpClient = httpClient;
            this.auProvider = auProvider;
            this.baseUrl = configuration.GetValue<string>("IdentityURL");
        }

        public void Logout()
        {
            httpClient.DefaultRequestHeaders.Authorization = null; 
            ((ApiAuthenticationStateProvider)auProvider).RemoveToken();
            ((ApiAuthenticationStateProvider)auProvider).MarkUserAsLoggedOut();
        }
        public async Task<(bool status,string?msg)> LoginAsync(LoginParamsType model)
        {
            // todo: login logic
            //new Uri(@"http://localhost:5152/") 
            var response = await httpClient.PostAsJsonAsync($"{baseUrl}Login/LoginByUserNameAndPwd", new { UserName = model.UserName, Pwd = model.Password });
            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();
                ((ApiAuthenticationStateProvider)auProvider).MarkUserAsAuthenticated(token);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
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