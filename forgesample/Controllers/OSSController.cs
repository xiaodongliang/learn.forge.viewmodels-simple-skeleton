
using Autodesk.Forge;
using Autodesk.Forge.Model;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace forgesample.Controllers
{
  public class OSSController : ApiController
  {
    /// <summary>
    /// Create a new bucket 
    /// </summary>
    [HttpPost]
    [Route("api/forge/oss/buckets")]
    public async Task<dynamic> CreateBucket([FromBody]CreateBucketModel bucket)
    {
      BucketsApi buckets = new BucketsApi();
      dynamic token = await OAuth2Controller.GetInternalAsync();
      buckets.Configuration.AccessToken = token.access_token;
      PostBucketsPayload bucketPayload = new PostBucketsPayload(bucket.bucketKey, null,
        PostBucketsPayload.PolicyKeyEnum.Transient);
      return await buckets.CreateBucketAsync(bucketPayload, "US");
    }

    /// <summary>
    /// Input model for CreateBucket method
    /// </summary>
    public class CreateBucketModel
    {
      public string bucketKey { get; set; }
    }

    /// <summary>
    /// Receive a file from the client and upload to the bucket
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Route("api/forge/oss/objects")]
    public async Task<dynamic> UploadObject()
    {
      // basic input validation
      HttpRequest req = HttpContext.Current.Request;
      if (string.IsNullOrWhiteSpace(req.Params["bucketKey"]))
        throw new System.Exception("BucketKey parameter was not provided.");

      if (req.Files.Count != 1)
        throw new System.Exception("Missing file to upload"); // for now, let's support just 1 file at a time

      string bucketKey = req.Params["bucketKey"];
      HttpPostedFile file = req.Files[0];

      // save the file on the server
      var fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), file.FileName);
      file.SaveAs(fileSavePath);

      // get the bucket...
      dynamic oauth = await OAuth2Controller.GetInternalAsync();
      ObjectsApi objects = new ObjectsApi();
      objects.Configuration.AccessToken = oauth.access_token;

      // upload the file/object, which will create a new object
      dynamic uploadedObj;
      using (StreamReader streamReader = new StreamReader(fileSavePath))
      {
        uploadedObj = await objects.UploadObjectAsync(bucketKey,
               file.FileName, (int)streamReader.BaseStream.Length, streamReader.BaseStream,
               "application/octet-stream");
      }

      // cleanup
      File.Delete(fileSavePath);

      return uploadedObj;
    }
  }
}
