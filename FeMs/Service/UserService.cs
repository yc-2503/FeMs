using FeMs.Common;
using FeMs.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace FeMs.Service
{
    internal class UserService : IUserService
    {
        private readonly HttpClient httpClient;

        public UserService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<ApiResponse<string?>> Login(string username, string password)
        {
            var response = new ApiResponse<string?>() { Status = false, Result = String.Empty};
            try
            {
                var Result = await httpClient.PostAsJsonAsync("Login/LoginByUserNameAndPwd", new { UserName= username, Pwd = password });

                if (Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    response.Result = await Result.Content.ReadAsStringAsync();
                    response.Status = true;
                }
                else
                {
                    response.Message =   await Result.Content.ReadAsStringAsync();
                    response.Status = false;
                }
                return response;
            }
            catch(HttpRequestException e)
            {
                response.Message = "服务器不可达";
                return response;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public bool Logout(string username, string token)
        {
            throw new NotImplementedException();
        }
    }
}
