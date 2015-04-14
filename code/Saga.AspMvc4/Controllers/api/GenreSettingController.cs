using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Saga.DI;
using Saga.Specification.Interfaces;
using Saga.Specification.Interfaces.Genres;
using Saga.Specification.Interfaces.Users;

namespace Saga.Music.AspMvc4.Controllers.api
{
    public class GenreSettingController : ApiController
    {
        private readonly IUserBusiness _UserBusiness = null;
        private readonly IGenreBusiness _GenreBusiness = null;
        private readonly IUtility _Utility;


        public GenreSettingController()
        {
            _UserBusiness = Factory.GetUserBusiness();
            _GenreBusiness = Factory.GetGenreBusiness();
            _Utility = Factory.GetUtility();
        }


        // GET api/genresetting/5?code={usercode}&include={true | false}
        public string Get(int id, string code, bool? include)
        {
            int userId = _Utility.GetUserIdFromCode(code);
            if (userId < 1) { return null; }

            IGenre genre = _GenreBusiness.GetAll().FirstOrDefault(g => g.GenreId == id);
            ISettings settings = _UserBusiness.Get(userId).Settings;

            _UserBusiness.ExcludedGenresSetting_Remove(settings, genre);
            _UserBusiness.IncludedGenresSetting_Remove(settings, genre);

            if (!include.HasValue)
            {
                return "cleared " + genre.Name + " from this station ";
            }

            if (include.Value)
            {
                _UserBusiness.IncludedGenresSetting_Add(settings, genre);
                return "included " + genre.Name + " from this station ";
            }
            else if (!include.Value)
            {
                _UserBusiness.ExcludedGenresSetting_Add(settings, genre);
                return "excluded " + genre.Name + " from this station ";
            }
            return "done";
        }
    }
}
