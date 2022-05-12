using FeMs.Common;

using FeMs.Service;
using FeMs.ViewModels;
using FeMs.Views;
using FeMs.Views.Dialogs;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using Unity.Injection;

namespace FeMs
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }
        protected override void OnInitialized()
        {
            var dialog = Container.Resolve<IDialogService>();

            dialog.Show("LoginView", logResult =>
            {
                if (logResult.Result != ButtonResult.Yes)
                {
                    Environment.Exit(0);
                    return;
                }
                else
                {
                    IConfigureService? config = App.Current.MainWindow.DataContext as IConfigureService;
                    if(config != null)
                    {
                        config.Configure();
                    }
                    base.OnInitialized();
                }
            });
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // HttpClient is intended to be instantiated once per application, rather than per-use.
            // --微软官方文档 https://docs.microsoft.com/zh-cn/dotnet/api/system.net.http.httpclient?view=net-6.0
            containerRegistry.RegisterInstance(typeof(HttpClient),new HttpClient() { BaseAddress = new Uri(@"http://localhost:5152/") });
            containerRegistry.Register<IUserService,UserService>();

            containerRegistry.RegisterDialog<LoginView, LoginViewModel>();
        }
        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            ViewModelLocationProvider.Register<MainWindow, MainWindowViewModel>();
        }
    }
}
