using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeMs.Share
{
    public class RoleDTO
    {
        [Required]
        public string RoleName { get; set; }
        public string RoleId { get; set; }
        public string Descript { get; set; }
    }
}
