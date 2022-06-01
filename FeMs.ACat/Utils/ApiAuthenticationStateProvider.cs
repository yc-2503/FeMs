using Blazored.LocalStorage;
using FeMs.Share;
using MediatR;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FeMs.ACat.Utils
{
    public class TokenHelper
    {
        private readonly ILocalStorageService _localStorage;
        private string _accessToken = string.Empty;
        private string _refreshToken = string.Empty;

        public TokenHelper(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
            InitTokens();
            //accessToken = _localStorage.GetItemAsStringAsync("accessToken").Result;
            //refreshToken = _localStorage.GetItemAsStringAsync("refreshToken").Result;
        }
        async void InitTokens()
        {
            if(_accessToken == string.Empty)
            {
                _accessToken = await _localStorage.GetItemAsStringAsync("accessToken");
            }
            if(_refreshToken == string.Empty)
            {
                _refreshToken = await _localStorage.GetItemAsStringAsync("refreshToken");
            }
        }
        public string GetAccessToke()
        {
            //return await _localStorage.GetItemAsStringAsync("accessToken");
            return _accessToken;
        }
        public async Task<string> GetAccessTokeAsync()
        {
            return await _localStorage.GetItemAsStringAsync("accessToken");
        }
        public async Task SetAccessToken(string accessToken)
        {
            _accessToken = accessToken;
            await _localStorage.SetItemAsStringAsync("accessToken", accessToken);
        }
        public string GetRefreshToken()
        {
            // return await _localStorage.GetItemAsStringAsync("refreshToken");
            return _refreshToken;
        }
        public async Task SetRefreshToken(string refreshToken)
        {
            _refreshToken = refreshToken;
            await _localStorage.SetItemAsStringAsync("refreshToken", refreshToken);
        }
        public void RemoveToken()
        {
            _accessToken = String.Empty;
            _refreshToken = String.Empty;

            _localStorage.RemoveItemAsync("accessToken");
            _localStorage.RemoveItemAsync("refreshToken");
        }

    }
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly TokenHelper _tokenHelper;

        public ApiAuthenticationStateProvider(TokenHelper tokenHelper)
        {
            _tokenHelper = tokenHelper;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var savedToken =  _tokenHelper.GetAccessToke();

            if (string.IsNullOrWhiteSpace(savedToken))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
            else
            {
             //   _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(savedToken), "jwt")));
            }
        }

        public void MarkUserAsAuthenticated()
        {
            string accessToken = _tokenHelper.GetAccessToke();
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(accessToken), "jwt"));

            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));

            //var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, account), new Claim(ClaimTypes.Role, account) }, "apiauth"));
            //var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }
        public void MarkUserAsLoggedOut()
        {
            
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }
        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            if (keyValuePairs != null)
            {
                keyValuePairs.TryGetValue(ClaimTypes.Role, out object? roles);
                if (roles != null)
                {
                    string rStr = roles.ToString();
                    if (rStr.Trim().StartsWith("["))
                    {
                        var parsedRoles = JsonSerializer.Deserialize<string[]>(rStr);
                        if (parsedRoles != null)
                        {
                            foreach (var parsedRole in parsedRoles)
                            {
                                claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                            }
                        }
                    }
                    else
                    {
                        claims.Add(new Claim(ClaimTypes.Role, rStr));
                    }

                    keyValuePairs.Remove(ClaimTypes.Role);
                }
                claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));
            }
            return claims;
        }
        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

        //public Task Handle(LoginEvent notification, CancellationToken cancellationToken)
        //{
        //    //  MarkUserAsAuthenticated(notification.tokens.accessToken, notification.tokens.refreshToken);
        //    Console.WriteLine(randm);
        //    return Task.CompletedTask;
        //}
    }
}
