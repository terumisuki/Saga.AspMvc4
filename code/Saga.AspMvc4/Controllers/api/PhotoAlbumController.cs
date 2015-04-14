using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using ImageResizer;
using Saga.DI;
using Saga.Specification;
using Saga.Specification.Interfaces;
using Saga.Specification.Interfaces.Images;
using Saga.Specification.Interfaces.Users;

namespace Saga.Music.AspMvc4.Controllers.api
{
    public class PhotoAlbumController : ApiController
    {
        private readonly IUtility _Utility;
        private readonly IImageBusiness _ImageBusiness;

        public PhotoAlbumController()
        {
            _Utility = Factory.GetUtility();
            _ImageBusiness = Factory.GetImageBusiness();
        }

        // GET api/photoalbum?auth=<user code>&photoid=<id>&width=<width>&height=<height>&timestamp=<timestamp>
        public HttpResponseMessage Get(string auth, int photoid, int width, int height, long timeStamp)
        {
            string traceString = string.Empty;
            try
            {
                int userId = _Utility.GetUserIdFromCode(auth);
                if (userId < 1) { return null; }

                IImage image = Factory.GetImageObject();
                image = _ImageBusiness.Get(photoid);

                string sagaImagePath = image.MediaFilePath.ToLowerInvariant();
                sagaImagePath = sagaImagePath.Replace(Constants.ImagesDirectoryPath.ToLowerInvariant(), Constants.ImagesDirectoryPath__InternetFacing);
                
                string resizedImageFilePath = sagaImagePath.ToLowerInvariant();
                resizedImageFilePath = resizedImageFilePath.Replace(@".jpg", @"_saga_resized.jpg");

                ResizeSettings resizeSettings = new ResizeSettings();
                resizeSettings.Width = width;
                resizeSettings.Height = height;
                //resizeSettings.Mode = FitMode.Crop;
                resizeSettings.Mode = FitMode.Pad;
                resizeSettings.BackgroundColor = System.Drawing.Color.Black;

                resizeSettings.Format = "jpg";
                //ImageResizer.ImageBuilder.Current.Build(sagaImagePath, resizedImageFilePath, new ResizeSettings("bgcolor=black&width=" + width + "&height=" + height + "&s.roundcorners=30"));
                //ImageResizer.ImageBuilder.Current.Build(sagaImagePath, resizedImageFilePath, new ResizeSettings("bgcolor=black&width=" + width + "&height=" + height + "&s.sepia=true"));
                traceString = "made it here   " + sagaImagePath + Environment.NewLine + resizedImageFilePath;
                ImageResizer.ImageBuilder.Current.Build(sagaImagePath, resizedImageFilePath, resizeSettings);
                var stream = new FileStream(resizedImageFilePath, FileMode.Open, FileAccess.Read);

                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
                return result;
            }
            catch (Exception ex)
            {
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                result.Content = new StringContent("ouch");

                // log error
                string error = "PhotoAlbumController errored." + Environment.NewLine + traceString;
                _ImageBusiness.LogError(error);
                _ImageBusiness.LogError(ex);
                return result;
            }
        }
    }
}
