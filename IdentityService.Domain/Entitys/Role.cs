using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.Entitys
{
    public class Role : IdentityRole<uint>
    {
        public Role()
        {
        }
    }
}

