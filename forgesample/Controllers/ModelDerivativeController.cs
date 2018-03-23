using Autodesk.Forge;
using Autodesk.Forge.Model;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace forgesample.Controllers
{
  public class ModelDerivativeController : ApiController
  {
    /// <summary>
    /// Start the translation job for a give bucketKey/objectName
    /// </summary>
    /// <param name="objModel"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("api/forge/modelderivative/jobs")]
    public async Task<dynamic> TranslateObject([FromBody]TranslateObjectModel objModel)
    {
        dynamic oauth = await OAuth2Controller.GetInternalAsync();

      // prepare the payload
      List<JobPayloadItem> outputs = new List<JobPayloadItem>()
      {
       new JobPayloadItem(
         JobPayloadItem.TypeEnum.Svf,
         new List<JobPayloadItem.ViewsEnum>()
         {
           JobPayloadItem.ViewsEnum._2d,
           JobPayloadItem.ViewsEnum._3d
         })
      };
      JobPayload job;
      job = new JobPayload(new JobPayloadInput(Base64Encode(objModel.objectName)), new JobPayloadOutput(outputs));

      // start the translation
      DerivativesApi derivative = new DerivativesApi();
      derivative.Configuration.AccessToken = oauth.access_token;
      dynamic jobPosted = await derivative.TranslateAsync(job);
      return jobPosted;
    }

    /// <summary>
    /// Model for TranslateObject method
    /// </summary>
    public class TranslateObjectModel
    {
      public string objectName { get; set; }
    }

    [HttpPost]
    [Route("api/forge/modelderivative/status")]
    public async Task<dynamic> GetManifestStatus([FromBody]TranslateObjectModel objModel)
    {
        dynamic oauth = await OAuth2Controller.GetInternalAsync();

        // start the translation
        DerivativesApi derivative = new DerivativesApi();

        derivative.Configuration.AccessToken = oauth.access_token;
        dynamic manifest = await derivative.GetManifestAsync(objModel.objectName);
        return manifest;
    }




        /// <summary>
        /// Start the translation job for a give bucketKey/objectName
        /// </summary>
        /// <param name="objModel"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/forge/modelderivative/status")]
        public async Task<dynamic> GetManifestStatus([FromUri]string urn)
        {
            dynamic oauth = await OAuth2Controller.GetInternalAsync();

            // start the translation
            DerivativesApi derivative = new DerivativesApi();

            derivative.Configuration.AccessToken = oauth.access_token;
            dynamic manifest = await derivative.GetManifestAsync(Base64Encode(urn)); 
            return manifest;
        }


        /// <summary>
        /// Base64 enconde a string
        /// </summary>
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }



    }
}
