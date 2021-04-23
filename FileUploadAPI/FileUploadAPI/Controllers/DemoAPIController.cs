using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using FileUploadAPI.Models;

namespace FileUploadAPI.Controllers
{
    [RoutePrefix("API/Demo")]
    public class DemoAPIController : ApiController
    {
        [HttpPost]
        [Route("AddFileDetails")]
        public IHttpActionResult AddFile()
        {
            string result = "";
            try
            {
                AngularDBEntities objEntity = new AngularDBEntities();
                FileDetail objFile = new FileDetail();
                
                string fileName = null;
                string imageName = null;
                var httpRequest = HttpContext.Current.Request;
                var postedImage = httpRequest.Files["ImageUpload"];
                var postedFile = httpRequest.Files["FileUpload"];

                objFile.UserName = httpRequest.Form["UserName"];

                if (postedImage != null)
                {
                    imageName = new String(Path.GetFileNameWithoutExtension(postedImage.FileName).Take(10).ToArray()).Replace(" ", "-");
                    imageName = imageName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(postedImage.FileName);
                    var filePath = HttpContext.Current.Server.MapPath("~/Files/" + imageName);
                    postedImage.SaveAs(filePath);
                   
                }

                if (postedFile != null)
                {
                    fileName = new String(Path.GetFileNameWithoutExtension(postedFile.FileName).Take(10).ToArray()).Replace(" ", "-");
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(postedFile.FileName);
                    var filePath = HttpContext.Current.Server.MapPath("~/Files/" + fileName);
                    postedFile.SaveAs(filePath);
                }
                objFile.Image = imageName;
                objFile.DocFile = fileName;
                objEntity.FileDetails.Add(objFile);
                int i = objEntity.SaveChanges();
                if(i > 0)
                {
                    result ="File uploaded sucessfully";
                }
                else
                {
                    result = "File uploaded faild";
                }
               
            }
            catch (Exception)
            {
                throw;
            }
            return Ok(result);
        }


        [HttpGet]
        [Route("GetFile")]
        //download file api
        public HttpResponseMessage GetFile(string docFile)
        {

            //Create HTTP Response.
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

            //Set the File Path.
            string filePath = System.Web.HttpContext.Current.Server.MapPath("~/Files/") + docFile + ".docx";

            //Check whether File exists.
            if (!File.Exists(filePath))
            {
                //Throw 404 (Not Found) exception if File not found.
                response.StatusCode = HttpStatusCode.NotFound;
                response.ReasonPhrase = string.Format("File not found: {0} .", docFile);
                throw new HttpResponseException(response);
            }

            //Read the File into a Byte Array.
            byte[] bytes = File.ReadAllBytes(filePath);

            //Set the Response Content.
            response.Content = new ByteArrayContent(bytes);

            //Set the Response Content Length.
            response.Content.Headers.ContentLength = bytes.LongLength;

            //Set the Content Disposition Header Value and FileName.
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = docFile + ".docx";

            //Set the File Content Type.
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(docFile + ".docx"));
            return response;
        }

        [HttpGet]
        [Route("GetFileDetails")]
        public IHttpActionResult GetFile()
        {
           var url = HttpContext.Current.Request.Url;
            IEnumerable<FileDetailsVM> lstFile = new List<FileDetailsVM>();
            try
            {
                AngularDBEntities objEntity = new AngularDBEntities();

                lstFile = objEntity.FileDetails.Select(a=> new FileDetailsVM
                {
                FileId = a.FileId,
                UserName = a.UserName,
                Image =url.Scheme + "://" + url.Host + ":" + url.Port + "/Files/" + a.Image,
                DocFile = a.DocFile,
                ImageName = a.Image
                }).ToList();
            }
            catch (Exception)
            {
                throw;
            }
            return Ok(lstFile);
        }

        [HttpGet]
        [Route("GetImage")]
        //download Image file api
        public HttpResponseMessage GetImage(string image)
        {

            //Create HTTP Response.
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

            //Set the File Path.
            string filePath = System.Web.HttpContext.Current.Server.MapPath("~/Files/") + image + ".PNG";

            //Check whether File exists.
            if (!File.Exists(filePath))
            {
                //Throw 404 (Not Found) exception if File not found.
                response.StatusCode = HttpStatusCode.NotFound;
                response.ReasonPhrase = string.Format("File not found: {0} .", image);
                throw new HttpResponseException(response);
            }

            //Read the File into a Byte Array.
            byte[] bytes = File.ReadAllBytes(filePath);

            //Set the Response Content.
            response.Content = new ByteArrayContent(bytes);

            //Set the Response Content Length.
            response.Content.Headers.ContentLength = bytes.LongLength;

            //Set the Content Disposition Header Value and FileName.
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = image + ".PNG";

            //Set the File Content Type.
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(image + ".PNG"));
            return response;
        }
    }
}
