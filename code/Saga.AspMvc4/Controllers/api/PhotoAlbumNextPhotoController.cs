using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Interfaces.Models;
using Saga.DI;
using Saga.Specification.Interfaces;
using Saga.Specification.Interfaces.Images;
using Saga.Specification.Interfaces.PhotoAlbums;
using Saga.Specification.Interfaces.Users;

namespace Saga.Music.AspMvc4.Controllers.api
{
    public class PhotoAlbumNextPhotoController : ApiController
    {
        private readonly IPhotoAlbumBusiness _PhotoAlbumBusiness;
        private readonly IUtility _Utility;
        private readonly IImageBusiness _ImageBusiness;
        private readonly IUserBusiness _UserBusiness;

        public PhotoAlbumNextPhotoController()
        {
            _PhotoAlbumBusiness = Factory.GetPhotoAlbumBusiness();
            _Utility = Factory.GetUtility();
            _ImageBusiness = Factory.GetImageBusiness();
            _UserBusiness = Factory.GetUserBusiness();
        }

        // GET api/photoalbumnextphoto?acode=<photo album code>&auth=<usercode>&timestamp=<timestamp>
        public PhotoModel Get(string acode, string auth, long timestamp)
        {
            int userId = _Utility.GetUserIdFromCode(auth);
            if (userId < 1) { return null; }

            IPhoto nextPhoto = _PhotoAlbumBusiness.GetNextPhoto(acode, userId);
            
            if (nextPhoto == null)
            {
                _ImageBusiness.ClearNoRepeatList(userId);
                nextPhoto = _PhotoAlbumBusiness.GetNextPhoto(acode, userId);
            }

            _UserBusiness.Played(userId, nextPhoto.MediaId);

            PhotoModel photo = new PhotoModel();
            photo.EnglishCaption = nextPhoto.EnglishCaption;
            photo.JapaneseCaption = nextPhoto.JapaneseCaption;
            photo.MediaId = nextPhoto.MediaId;
            return photo;
        }
    }
}
