using AntDesign.TableModels;
using FeMs.ACat.Models;
using FeMs.Share;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FeMs.ACat.Pages.Security
{
    partial class UserManage
    {
        private readonly BasicListFormModel _model = new BasicListFormModel();

        private List<UserDTO> _data;
        private List<string> userRoles;
        int _pageIndex = 1;
        int _pageSize = 2;
        int _total;
        bool _loading = false;
        [Inject] private HttpClient ProjectService { get; set; }

        private void ShowModal()
        {
        }
        async Task HandleTableChange(QueryModel<UserDTO> queryModel)
        {
            _loading = true;
            await base.OnInitializedAsync();

            userRoles = new List<string>();
            userRoles.Add("role1");
            userRoles.Add("role2");
            userRoles.Add("role3");

            var rs = await ProjectService.GetStringAsync("http://localhost:5152/UsersInfo/GetUsersCount");

            int.TryParse(rs, out _total);

            _data = await ProjectService.GetFromJsonAsync<List<UserDTO>>("http://localhost:5152/UsersInfo/GetUsers?from=0&to=3");
            _loading = false;
        }
        protected override async Task OnInitializedAsync()
        {

        }
    }
}
