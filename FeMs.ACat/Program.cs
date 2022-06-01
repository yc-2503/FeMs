using AntDesign.ProLayout;
using Blazored.LocalStorage;
using FeMs.ACat.Services;
using FeMs.ACat.Utils;
using FeMs.Share;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MediatR;
using System.Reflection;

namespace FeMs.ACat
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            //builder.Services.AddMediatR(typeof(Program));
            builder.Services.AddBlazoredLocalStorage(config =>
            {
                config.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                config.JsonSerializerOptions.IgnoreReadOnlyProperties = true;
                config.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                config.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                config.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
                config.JsonSerializerOptions.WriteIndented = false;
            });


            builder.Services.AddSingleton<TokenHelper>();
            builder.Services.AddAuthorizationCore().AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();

            builder.Services.AddScoped<IAccountService, AccountService>();

            builder.Services.AddScoped<LoginHandler>();
            //TODO, 登录后，如果用户一直在操作需要不断更新TOKEN，如果用户不操作且超时了，TOKEN直接过期
            //step1 检查用户是否在一直操作，找一个工具类，每次用户与服务器交互都重置定时器
            //step2 定时器到时间了就重新获取TOKEN
            //所以就直接在ApiAuthenticationStateProvider 新建一个线程吧，没有办法了
            //重新获取TOKEN还真不能用用户名密码，因为用户名密码不能存本地呀，用户关了浏览器重新打开用户名密码就没有了
            builder.Services.AddHttpClient(name: "Identity",  c =>
            {
                //var _tokenHelper = builder.Services.BuildServiceProvider().GetService<TokenHelper>();
                c.BaseAddress = new Uri(builder.Configuration.GetSection("IdentityURL").Value);
                //var token = _tokenHelper.GetAccessToke();
                //c.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            }).AddHttpMessageHandler<LoginHandler>();

            // builder.Services.AddScoped<LoginHandler>();
            //必须给LoggingHandler传入HttpClientHandler，因为最终都是通过HttpClientHandler发送请求，如果不传会抛异常
            //var loginHandler = builder.Services.BuildServiceProvider().GetService<LoginHandler>();
            //var loginHandler = new LoginHandler();
            builder.Services.AddScoped(sp =>
            {
                var _httpClient = new HttpClient() { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
                return _httpClient;
            });

            builder.Services.AddAntDesign();
            builder.Services.Configure<ProSettings>(builder.Configuration.GetSection("ProSettings"));
            builder.Services.AddScoped<IChartService, ChartService>();
            builder.Services.AddScoped<IProjectService, ProjectService>();
            builder.Services.AddScoped<IUserService, UserService>();
            
            builder.Services.AddScoped<IProfileService, ProfileService>();


            await builder.Build().RunAsync();
        }
    }
}