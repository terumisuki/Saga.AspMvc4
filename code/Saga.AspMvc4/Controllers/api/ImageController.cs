using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
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
    public class ImageController : ApiController
    {
        private readonly IImageBusiness _ImageBusiness;
        private readonly IUtility _Utility;

        public ImageController()
        {
            _ImageBusiness = Factory.GetImageBusiness();
            _Utility = Factory.GetUtility();
        }

        // GET api/image/?code=<usercode>&width=<width>&height=<height>&timeStamp=<timestamp>
        public HttpResponseMessage Get(string code, int width, int height, long timeStamp)
        {
            try
            {
                int userid = _Utility.GetUserIdFromCode(code);
                if (userid < 1) { return null; }

                IUser user = Factory.GetUser(userid);
                IList<IImage> images = new List<IImage>();

                if (userid == Constants.User2.Id)
                {
                    images = _ImageBusiness.GetImagesForSlideShow(user.UserId, user.Settings.IncludedSlideShowTags, user.Settings.ExcludedSlideShowTags);
                    if (images.Count() == 0)
                    {
                        _ImageBusiness.ClearNoRepeatList(userid);
                        images = _ImageBusiness.GetImagesForSlideShow(user.UserId, user.Settings.IncludedSlideShowTags, user.Settings.ExcludedSlideShowTags);
                    }
                }
                // Todo: Move all the other users under the main "GetImagesForSlideShow" method
                else
                {
                    images = _ImageBusiness.GetImagesForSlideShow(userid);
                    if (images.Count() == 0)
                    {
                        _ImageBusiness.ClearNoRepeatList(userid);
                        images = _ImageBusiness.GetImagesForSlideShow(userid);
                    }
                }

                Random randomizer = new Random();
                IImage image = images[randomizer.Next(0, images.Count() - 1)];
                IUserBusiness userBusiness = Factory.GetUserBusiness();
                userBusiness.Played(user.UserId, image.MediaId);

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
                string error = "ImageControllerAlbumController errored.";
                _ImageBusiness.LogError(error);
                _ImageBusiness.LogError(ex);
                return result;
            }
        }
    }
}
