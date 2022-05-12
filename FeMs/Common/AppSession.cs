using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeMs.Common
{
    internal static class AppSession
    {
        public static string? UserName { get; set; }
        public static string? Token { get; set; }
        public static string? Role { get; set; }
    }
}
