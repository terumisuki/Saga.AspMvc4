using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Saga.DI;
using Saga.Music.Interfaces.Models;
using Saga.Specification.Interfaces;
using Saga.Specification.Interfaces.Users;

namespace Saga.Music.AspMvc4.Controllers.api
{
    public class CurrentSettingsController : ApiController
    {
        private readonly IUserBusiness _UserBusiness;
        private readonly IUtility _Utility;


        public CurrentSettingsController()
        {
            _UserBusiness = Factory.GetUserBusiness();
            _Utility = Factory.GetUtility();
        }




        // GET api/currentsettings/?code={val}
        public List<SearchItemModel> Get(string code)
        {
            int userId = _Utility.GetUserIdFromCode(code);
            if (userId < 1) { return null; }
            
            List<SearchItemModel> results = new List<SearchItemModel>();
            var user = _UserBusiness.Get(userId);

            #region genres
            // attached genres
            var attachedGenres = user.Settings.AttachedGenres;
            foreach (var genre in attachedGenres)
            {
                SearchItemModel result = new SearchItemModel
                {
                    Id = genre.GenreId,
                    Label = genre.Name,
                    Type = Specification.Enumerations.SearchItemTypes.Genre,
                    Setting = true
                };
                results.Add(result);
            }


            // excluded genres
            var excludedGenres = user.Settings.UnattachedGenres;
            foreach (var genre in excludedGenres)
            {
                SearchItemModel result = new SearchItemModel
                {
                    Id = genre.GenreId,
                    Label = genre.Name,
                    Type = Specification.Enumerations.SearchItemTypes.Genre,
                    Setting = false
                };
                results.Add(result);
            }
            #endregion


            #region artists
            // included artists
            var includedArtists = user.Settings.AttachedArtists;
            foreach (var artist in includedArtists)
            {
                SearchItemModel result = new SearchItemModel
                {
                    Id = artist.ArtistId,
                    Label = artist.Name,
                    Type = Specification.Enumerations.SearchItemTypes.Artist,
                    Setting = true
                };
                results.Add(result);
            }

            // excluded artists
            var excludedArtists = user.Settings.UnattachedArtists;
            foreach (var artist in excludedArtists)
            {
                SearchItemModel result = new SearchItemModel
                {
                    Id = artist.ArtistId,
                    Label = artist.Name,
                    Type = Specification.Enumerations.SearchItemTypes.Artist,
                    Setting = false
                };
                results.Add(result);
            }
            #endregion


            #region parts
            // included parts
            var includedParts = user.Settings.IncludedParts;
            foreach (var part in includedParts)
            {
                SearchItemModel result = new SearchItemModel
                {
                    Id = part.PartId,
                    Label = part.Name,
                    Type = Specification.Enumerations.SearchItemTypes.Part,
                    Setting = true
                };
                results.Add(result);
            }

            var excludedParts = user.Settings.ExcludedParts;
            foreach (var part in excludedParts)
            {
                SearchItemModel result = new SearchItemModel
                {
                    Id = part.PartId,
                    Label = part.Name,
                    Type = Specification.Enumerations.SearchItemTypes.Part,
                    Setting = false
                };
                results.Add(result);
            }
            #endregion
            
            
            return results;
        }
    }
}
