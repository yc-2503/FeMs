using FeMs.ACat.Models;
using FeMs.ACat.Utils;
using FeMs.Share;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FeMs.ACat.Services
{
    public interface IUserService
    {
        Task<CurrentUser> GetCurrentUserAsync();
        Task<int> GetUsersCount();
        Task<int> GetRolesCount();
        Task<List<UserDTO>> GetUsers(int from, int to);
        Task<List<RoleDTO>> GetRoles(int from, int to);
    }

    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly string baseUrl;
        TokenHelper tokenHelper;
        public UserService(IHttpClientFactory httpfactory,IConfiguration configuration, TokenHelper tokenHelper)
        {
            _httpClient = httpfactory.CreateClient("Identity");
            baseUrl = configuration.GetValue<string>("IdentityURL");
            this.tokenHelper = tokenHelper;
        }

        public async Task<CurrentUser> GetCurrentUserAsync()
        {
            CurrentUser cUser = new CurrentUser();
            cUser.Name = String.Empty;
            string token = tokenHelper.GetAccessToke();
            if(token == string.Empty)
            {
                token = await tokenHelper.GetAccessTokeAsync();
            }
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var r = await _httpClient.GetAsync($"{baseUrl}Login/GetCurrentUserInfo");
            if (r != null)
            {
                if(r.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var user = await r.Content.ReadFromJsonAsync<UserDTO>();
                    cUser.Name = user.UserName;
                    cUser.Email = user.Email;
                    cUser.Phone = user.PhoneNumber;
                }
            }

            return cUser;
        }

        public async Task<int> GetUsersCount()
        {
            int _total = 0;
            string token = tokenHelper.GetAccessToke();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var rs = await _httpClient.GetStringAsync($"{baseUrl}UsersInfo/GetUsersCount");
            int.TryParse(rs, out _total);
            return _total;
        }
        public async Task<List<UserDTO>> GetUsers(int from, int to)
        {
            List<UserDTO> _data = new List<UserDTO>();
            if (to >= from && from >= 0 & to >= 0)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenHelper.GetAccessToke());
                _data = await _httpClient.GetFromJsonAsync<List<UserDTO>>($"{baseUrl}UsersInfo/GetUsers?from={from}&to={to}");
            }
            return _data;
        }

        public async Task<List<RoleDTO>> GetRoles(int from, int to)
        {
            List<RoleDTO> _data = new List<RoleDTO>();
            if (to >= from && from >= 0 & to >= 0)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenHelper.GetAccessToke());
                _data = await _httpClient.GetFromJsonAsync<List<RoleDTO>>($"{baseUrl}UsersInfo/GetRoles?from={from}&to={to}");
            }
            return _data;
        }

        public async Task<int> GetRolesCount()
        {
            int _total = 0;
            string token = tokenHelper.GetAccessToke();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var rs = await _httpClient.GetStringAsync($"{baseUrl}UsersInfo/GetRolesCount");
            int.TryParse(rs, out _total);
            return _total;
        }
    }
}