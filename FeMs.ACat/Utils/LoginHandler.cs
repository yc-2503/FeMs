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
        private Func<Task<bool>> _ReLoginAsync;

        //public LoginHandler(IAccountService accountService)
        //{
        //    _accountService = accountService;
        //}
        public Func<Task<bool>> ReLoginAsync { set { this._ReLoginAsync = value; } }

        protected override async Task<HttpResponseMessage> SendAsync( HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                Console.WriteLine("401!");
                if(_ReLoginAsync!=null)
                {
                    if (await _ReLoginAsync())
                    {
                        response = await base.SendAsync(request, cancellationToken);
                    }
                }
                //if(await _accountService.ReLoginAsync())
                //{
                //    response = await base.SendAsync(request, cancellationToken);
                //}
            }
            return response;
        }
    }
}
