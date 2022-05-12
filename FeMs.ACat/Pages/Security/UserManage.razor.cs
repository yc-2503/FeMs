using FeMs.ACat.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace FeMs.ACat.Pages.Security
{
    public class UserInfo
    {
        public string UserName { get; set; }
        public List<string> Roles;
    }
    partial class UserManage
    {
        private readonly BasicListFormModel _model = new BasicListFormModel();

        private List<UserInfo> _data;

        [Inject] private HttpClient ProjectService { get; set; }

        private void ShowModal()
        {
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _data=new List<UserInfo>();
            UserInfo user1 = new UserInfo() { UserName = "abc"};
            user1.Roles = new List<string>();
            user1.Roles.Add("role1");
            user1.Roles.Add("role2");
            user1.Roles.Add("role3");

            _data.Add(user1);
            _data.Add(new UserInfo() { UserName = "def" });
            _data.Add(new UserInfo() { UserName = "mvp" });

            // _data = await ProjectService.GetFakeListAsync(5);
        }
    }
}
