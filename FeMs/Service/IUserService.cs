using FeMs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeMs.Service
{
    public  interface IUserService
    {
        //成功返回TOKEN，失败返回null
       Task<ApiResponse<string?>> Login(string username,string password);

        bool Logout(string username,string token);
    }
}
