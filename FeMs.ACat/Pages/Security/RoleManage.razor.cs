using AntDesign.TableModels;
using FeMs.ACat.Models;
using FeMs.ACat.Services;
using FeMs.Share;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace FeMs.ACat.Pages.Security
{
    partial class RoleManage
    {
        private readonly BasicListFormModel _model = new BasicListFormModel();
        [Inject] private IUserService userService { get; set; }

        private List<RoleDTO> _data;
        int _pageIndex = 1;
        int _pageSize = 2;
        int _total;
        bool _loading = false;
        [Inject] private HttpClient ProjectService { get; set; }

        RoleDTO selectUser;
        private void UserListClick(RowData<RoleDTO> item)
        {
            selectUser = item.Data;
        }
        private void ShowModal()
        {
        }
        async Task HandleTableChange(QueryModel<RoleDTO> queryModel)
        {
            _loading = true;

            _total = await userService.GetUsersCount();
            if (queryModel.PageSize > 0 && queryModel.PageIndex > 0)
            {
                _data = await userService.GetRoles((queryModel.PageIndex - 1) * queryModel.PageSize, (queryModel.PageIndex - 1) * queryModel.PageSize + queryModel.PageSize);
            }
            _loading = false;
        }
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }
    }
}
