using System.Net.Http;

namespace FeMs.ACat.Utils
{
    public class HttpClientHelper
    {
        HttpClient httpClient;
        LoginHandler loginHandler;
        public HttpClientHelper()
        {
            httpClient = new HttpClient(loginHandler);
        }
        //public HttpClient GetClient()
        //{

        //}
    }
}
