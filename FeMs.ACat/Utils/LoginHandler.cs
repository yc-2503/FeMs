using FeMs.ACat.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FeMs.ACat.Utils
{
    public class LoginHandler : DelegatingHandler
    {
        private Func<Task<string>> _ReLoginAsync;
        private Action _LoginOut;
        IAccountService _AccountService;

        public LoginHandler(IAccountService accountService)
        {
            _AccountService = accountService;
            this._ReLoginAsync = accountService.ReLoginAsync;
            this._LoginOut = accountService.Logout;
        }

        public Func<Task<string>> ReLoginAsync { get => _ReLoginAsync; set => _ReLoginAsync = value; }
        public Action LoginOut { get => _LoginOut; set => _LoginOut = value; }


        //step 1 检测到返回401，调用relogin
        //step 2 relogin返回失败，通知授权过期
        protected override async Task<HttpResponseMessage> SendAsync( HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        { 
            var response = await base.SendAsync(request, cancellationToken);
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                Console.WriteLine("401!");
                if(ReLoginAsync!=null)
                {
                    string token = await ReLoginAsync();
                    if (token != string.Empty)
                    {
                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                        response = await base.SendAsync(request, cancellationToken);
                    }
                }

            }
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                if (LoginOut != null)
                {
                    LoginOut();
                }
            }
            return response;
        }
    }
}
