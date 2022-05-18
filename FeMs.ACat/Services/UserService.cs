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
        Task<List<UserDTO>> GetUsers(int from, int to);
        Task<List<RoleDTO>> GetRoles(int from, int to);
    }

    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private readonly string baseUrl;
        public UserService(HttpClient httpClient, IConfiguration configuration, AuthenticationStateProvider authenticationStateProvider)
        {

            _httpClient = httpClient;
            this.baseUrl = configuration.GetValue<string>("IdentityURL");
            this.authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<CurrentUser> GetCurrentUserAsync()
        {

            CurrentUser cUser = new CurrentUser();

            UserDTO response = await _httpClient.GetFromJsonAsync<UserDTO>($"{baseUrl}Login/GetCurrentUserInfo");
            cUser.Name = response.UserName;
            cUser.Email = response.Email;
            cUser.Phone = response.PhoneNumber;

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

        public Task<List<RoleDTO>> GetRoles(int from, int to)
        {
            throw new System.NotImplementedException();
        }
    }
}