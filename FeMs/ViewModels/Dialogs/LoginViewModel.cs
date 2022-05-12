using FeMs.Common;
using FeMs.Service;
using FeMs.Share;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeMs.ViewModels
{
    internal class LoginViewModel : BindableBase, IDialogAware
    {
        public string Title
        {
            get { return "登录"; }    
          
        } 
        public DelegateCommand<string> ExeCommand { get; init; }

        public LoginViewModel(IUserService userService)
        {
            this.userService = userService;
            ExeCommand = new DelegateCommand<string>(Execute);
        }

        public void Execute(string cmd)
        {
            switch(cmd)
            {
                case "Login":
                    Note = "";
                    this.Login();
                    break;
                default:
                    break;
            }
        }

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {

        }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        private string? _note;

        public string? Note
        {
            get { return _note; }
            set { _note = value; RaisePropertyChanged(); }
        }



        private readonly IUserService userService;
        private  async void Login()
        {
            if(UserName == null||Password==null)
            {
                Note = "登录失败";
                return;
            }
            string? token = string.Empty;
            ApiResponse<string?> response = await userService.Login(UserName, Password);
            if(response.Status == true)
            {
                token = response.Result;
            }

            if(token!=null && token.Length>5)
            {
                AppSession.Token = token;
                AppSession.UserName = UserName;
                Note = token;
                RequestClose.Invoke(new DialogResult(ButtonResult.Yes));
            }
            else
            {
                Note = response.Message;
            }
        }
    }
}
