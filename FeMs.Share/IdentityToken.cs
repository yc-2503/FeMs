using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeMs.Share
{
    public class IdentityToken
    {
        public string accessToken { get; init; }
        public string refreshToken { get; init; }
        public IdentityToken(string accessToken, string refreshToken)
        {
            this.accessToken = accessToken;
            this.refreshToken = refreshToken;
        }
    }
}
