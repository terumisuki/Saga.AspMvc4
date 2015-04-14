using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Saga.DI;
using Saga.Specification.Interfaces;
using Saga.Specification.Interfaces.Artists;
using Saga.Specification.Interfaces.Users;

namespace Saga.Music.AspMvc4.Controllers.api
{
    public class ArtistSettingController : ApiController
    {
        private readonly IUserBusiness _UserBusiness = null;
        private readonly IArtistBusiness _ArtistBusiness = null;
        private readonly IUtility _Utility;


        public ArtistSettingController()
        {
            _UserBusiness = Factory.GetUserBusiness();
            _ArtistBusiness = Factory.GetArtistBusiness();
            _Utility = Factory.GetUtility();
        }

        // GET api/artistsetting/{0}?code={1}&include={2}
        public string Get(int id, string code, bool? include)
        {
            int userId = _Utility.GetUserIdFromCode(code);
            if (userId < 1) { return null; }
            
            IArtist artist = _ArtistBusiness.GetAll().FirstOrDefault(a => a.ArtistId == id);
            ISettings settings = _UserBusiness.Get(userId).Settings;

            _UserBusiness.ExcludedArtistSetting_Remove(settings, artist);
            _UserBusiness.IncludedArtistSetting_Remove(settings, artist);

            if (!include.HasValue)
            {
                return "cleared " + artist.Name + " from this station ";
            }

            if (include.Value)
            {
                _UserBusiness.IncludedArtistSetting_Add(settings, artist);
                return "included " + artist.Name + " from this station ";
            }
            else if (!include.Value)
            {
                _UserBusiness.ExcludedArtistSetting_Add(settings, artist);
                return "excluded " + artist.Name + " from this station ";
            }
            return "done";
        }
    }
}
