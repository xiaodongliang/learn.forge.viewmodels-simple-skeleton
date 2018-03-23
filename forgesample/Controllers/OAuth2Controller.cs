
using Autodesk.Forge;
using System;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using RestSharp;

namespace forgesample.Controllers
{
    public class OAuth2Controller : ApiController
    {
        private static dynamic PublicToken { get; set; }
        private static dynamic InternalToken { get; set; }


        private static String responseString;

        /// <summary>
        /// Get access token with public (viewables:read) scope
        /// </summary>
        [HttpGet]
        [Route("api/forge/oauth/token")]
        public async Task<dynamic> GetPublicAsync()
        {
            if (PublicToken == null )
            {
                TwoLeggedApi oauth = new TwoLeggedApi();

                string grantType = "client_credentials";

                string forgeId       = WebConfigurationManager.AppSettings["FORGE_CLIENT_ID"];
                string forgeClient   = WebConfigurationManager.AppSettings["FORGE_CLIENT_SECRET"];

                PublicToken = await oauth.AuthenticateAsync(
                  forgeId,
                  forgeClient,
                  grantType,
                  new Scope[] { Scope.ViewablesRead });
            }
            return PublicToken;
        }

        public static async Task<dynamic> GetInternalAsync()
        {
            if (InternalToken == null)
            {
                TwoLeggedApi oauth = new TwoLeggedApi();

                string grantType = "client_credentials";

                string forgeId = WebConfigurationManager.AppSettings["FORGE_CLIENT_ID"];
                string forgeClient = WebConfigurationManager.AppSettings["FORGE_CLIENT_SECRET"];

                InternalToken = await oauth.AuthenticateAsync(
                  forgeId,
                  forgeClient,
                  grantType,
                  new Scope[] { Scope.BucketCreate, Scope.BucketRead, Scope.DataRead, Scope.DataCreate,Scope.DataWrite });
            }
            return InternalToken;
        }


        [HttpGet]
        [Route("api/forge/oauth/token/old")]
        public String GetPublic()
        {
            TwoLeggedApi oauth = new TwoLeggedApi();

            string grantType = "client_credentials";

            string forgeId = WebConfigurationManager.AppSettings["FORGE_CLIENT_ID"];
            string forgeClient = WebConfigurationManager.AppSettings["FORGE_CLIENT_SECRET"];

            RestClient _client = new RestClient("https://developer.api.autodesk.com");

            RestRequest authReq = new RestRequest();
            authReq.Resource = "authentication/v1/authenticate";
            authReq.Method = Method.POST;
            authReq.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            authReq.AddParameter("client_id", forgeId);
            authReq.AddParameter("client_secret", forgeClient);
            authReq.AddParameter("grant_type", grantType);
            authReq.AddParameter("scope", "data:read data:write bucket:create bucket:read");

            IRestResponse result = _client.Execute(authReq);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                responseString = result.Content;
            }
            return responseString;
        }

    }
}



