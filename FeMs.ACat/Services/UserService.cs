using FeMs.ACat.Models;
using FeMs.Share;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
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
    }

    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly string baseUrl;
        public UserService(HttpClient httpClient, AuthenticationStateProvider auProvider, IConfiguration configuration)
        {
            _httpClient = httpClient;
            this.baseUrl = configuration.GetValue<string>("IdentityURL");
        }

        public async Task<CurrentUser> GetCurrentUserAsync()
        {
            UserDTO response = await _httpClient.GetFromJsonAsync<UserDTO>($"{baseUrl}Login/GetUserInfo");
            CurrentUser cUser = new CurrentUser();
            cUser.Name = response.Name;
            cUser.Email = response.Email;
            cUser.Phone = response.Phone;
            return cUser;
        }
    }
}