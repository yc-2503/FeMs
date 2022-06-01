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
            //TODO, ��¼������û�һֱ�ڲ�����Ҫ���ϸ���TOKEN������û��������ҳ�ʱ�ˣ�TOKENֱ�ӹ���
            //step1 ����û��Ƿ���һֱ��������һ�������࣬ÿ���û�����������������ö�ʱ��
            //step2 ��ʱ����ʱ���˾����»�ȡTOKEN
            //���Ծ�ֱ����ApiAuthenticationStateProvider �½�һ���̰߳ɣ�û�а취��
            //���»�ȡTOKEN���治�����û������룬��Ϊ�û������벻�ܴ汾��ѽ���û�������������´��û��������û����
            builder.Services.AddHttpClient(name: "Identity",  c =>
            {
                //var _tokenHelper = builder.Services.BuildServiceProvider().GetService<TokenHelper>();
                c.BaseAddress = new Uri(builder.Configuration.GetSection("IdentityURL").Value);
                //var token = _tokenHelper.GetAccessToke();
                //c.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            }).AddHttpMessageHandler<LoginHandler>();

            // builder.Services.AddScoped<LoginHandler>();
            //�����LoggingHandler����HttpClientHandler����Ϊ���ն���ͨ��HttpClientHandler��������������������쳣
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