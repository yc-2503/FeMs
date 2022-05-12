using FeMs.Common;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeMs.ViewModels
{
    internal class MainWindowViewModel: BindableBase,IConfigureService
    {
        private string? userName;

        public string? UserName
        {
            get { return userName; }
            set { userName = value;RaisePropertyChanged(); }
        }

        public void Configure()
        {
            UserName = AppSession.UserName;

        }
    }
}
