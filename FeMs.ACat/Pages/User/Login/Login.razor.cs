using AntDesign;
using FeMs.ACat.Models;
using FeMs.ACat.Services;
using FeMs.ACat.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Threading.Tasks;

namespace FeMs.ACat.Pages.User
{
    public partial class Login
    {
        private readonly LoginParamsType _model = new LoginParamsType();

        [Inject] public NavigationManager NavigationManager { get; set; }

        [Inject] public IAccountService AccountService { get; set; }
        //[Inject] public AuthenticationStateProvider auProvider { get; set; }
        [Inject] public MessageService Message { get; set; }
        int loginType = 1;
        public async void HandleSubmit()
        {
            if (loginType == 1)
            {
                var r = await AccountService.LoginAsync(_model);
                if (r.status)
                {
                   // ((ApiAuthenticationStateProvider)auProvider).MarkUserAsAuthenticated();
                    NavigationManager.NavigateTo("/");
                    return;
                }else
                {
                    await Message.Error(r.msg);
                }
            }else
            {
                if (_model.UserName == "user" && _model.Password == "ant.design")
                {
                    NavigationManager.NavigateTo("/");
                    return ;
                }else
                {
                    await Message.Error("用户名或密码错误");
                }
            }

        }

        public async Task GetCaptcha()
        {
            var captcha = await AccountService.GetCaptchaAsync(_model.Mobile);
            await Message.Success($"Verification code validated successfully! The verification code is: {captcha}");
        }
    }
}