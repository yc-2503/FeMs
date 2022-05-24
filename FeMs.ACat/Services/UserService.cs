using FeMs.ACat.Models;
using FeMs.Share;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
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
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private readonly string baseUrl;
        public UserService(IHttpClientFactory httpClientFactory, AuthenticationStateProvider authenticationStateProvider)
        {

            _httpClient = httpClientFactory.CreateClient(name: "Identity"); ;

            this.authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<CurrentUser> GetCurrentUserAsync()
        {

            CurrentUser cUser = new CurrentUser();
            var r = await _httpClient.GetAsync("Login/GetCurrentUserInfo");
            if (r != null)
            {
                //JWT过期了，客户端其实不知道，这里这么实现不太好，有人是专门创建了一个线程，不断的去访问server看JWT有没有过期。
                if(r.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    ((ApiAuthenticationStateProvider)authenticationStateProvider).MarkUserAsLoggedOut();
                }else if(r.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var user = await r.Content.ReadFromJsonAsync<UserDTO>();
                    cUser.Name = user.UserName;
                    cUser.Email = user.Email;
                    cUser.Phone = user.PhoneNumber;
                }
            }
            //UserDTO response = await _httpClient.GetFromJsonAsync<UserDTO>($"{baseUrl}Login/GetCurrentUserInfo");
            //cUser.Name = response.UserName;
            //cUser.Email = response.Email;
            //cUser.Phone = response.PhoneNumber;

            return cUser;
        }

        public async Task<int> GetUsersCount()
        {
            int _total = 0;
            var rs = await _httpClient.GetStringAsync($"{baseUrl}UsersInfo/GetUsersCount");
            int.TryParse(rs, out _total);
            return _total;
        }
        public async Task<List<UserDTO>> GetUsers(int from, int to)
        {
            List<UserDTO> _data = new List<UserDTO>();
            if (to >= from && from >= 0 & to >= 0)
            {
                _data = await _httpClient.GetFromJsonAsync<List<UserDTO>>($"{baseUrl}UsersInfo/GetUsers?from={from}&to={to}");
            }
            return _data;
        }

        public async Task<List<RoleDTO>> GetRoles(int from, int to)
        {
            List<RoleDTO> _data = new List<RoleDTO>();
            if (to >= from && from >= 0 & to >= 0)
            {
                _data = await _httpClient.GetFromJsonAsync<List<RoleDTO>>($"{baseUrl}UsersInfo/GetRoles?from={from}&to={to}");
            }
            return _data;
        }

        public async Task<int> GetRolesCount()
        {
            int _total = 0;
            var rs = await _httpClient.GetStringAsync($"{baseUrl}UsersInfo/GetRolesCount");
            int.TryParse(rs, out _total);
            return _total;
        }
    }
}