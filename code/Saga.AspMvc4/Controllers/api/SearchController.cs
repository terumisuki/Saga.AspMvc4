using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Interfaces.Models;
using Saga.DI;
using Saga.Music.Interfaces.Models;
using Saga.Specification.Interfaces;
using Saga.Specification.Interfaces.Artists;
using Saga.Specification.Interfaces.Genres;
using Saga.Specification.Interfaces.Musical;
using Saga.Specification.Interfaces.Users;

namespace Saga.Music.AspMvc4.Controllers.api
{
    public class SearchController : ApiController
    {
        private readonly IGenreBusiness _GenreBusiness;
        private readonly IUserBusiness _UserBusiness;
        private readonly IArtistBusiness _ArtistBusiness;
        private readonly IPartBusiness _PartBusiness;
        private readonly IUtility _Utility;

        public SearchController()
        {
            _GenreBusiness = Factory.GetGenreBusiness();
            _UserBusiness = Factory.GetUserBusiness();
            _ArtistBusiness = Factory.GetArtistBusiness();
            _PartBusiness = Factory.GetPartsBusiness();
            _Utility = Factory.GetUtility();
        }
        
        // GET api/search/keywords?code={val}
        public List<SearchItemModel> Get(string id, string code)
        {
            string keywords = id;

            int userId = _Utility.GetUserIdFromCode(code);
            if (userId < 1) { return null; }

            var user = _UserBusiness.Get(userId);

            List<SearchItemModel> resultsGenres = SearchGenres(keywords, user);
            List<SearchItemModel> resultsArtists = SearchArtists(keywords, user);
            List<SearchItemModel> resultsParts = SearchParts(keywords, user);


            List<SearchItemModel> results = resultsArtists.Concat(resultsGenres).ToList();
            results = results.Concat(resultsParts).OrderBy(sm => sm.Label).ToList();
            return results;
        }

        private List<SearchItemModel> SearchParts(string keywords, IUser user)
        {
            var matchingParts = _PartBusiness.Search(keywords);
            List<SearchItemModel> results = new List<SearchItemModel>();

            foreach (var part in matchingParts)
            {
                var attachedParts = user.Settings.IncludedParts.ToList().Find(p => p.PartId == part.PartId);
                var unAttachedParts = user.Settings.ExcludedParts.ToList().Find(p => p.PartId == part.PartId);
                bool? toggleOnOf = null;
                if (attachedParts != null)
                {
                    toggleOnOf = true;
                }
                else if (unAttachedParts != null)
                {
                    toggleOnOf = false;
                }
                SearchItemModel result = new SearchItemModel
                {
                    Id = part.PartId,
                    Label = part.Name,
                    Type = Specification.Enumerations.SearchItemTypes.Part,
                    Setting = toggleOnOf
                };
                results.Add(result);
            }

            return results;
        }

        private List<SearchItemModel> SearchArtists(string keywords, IUser user)
        {
            var matchingArtists = _ArtistBusiness.Search(keywords);
            List<SearchItemModel> results = new List<SearchItemModel>();

            foreach (var artist in matchingArtists)
            {
                var attachedArtist = user.Settings.AttachedArtists.ToList().Find(a => a.ArtistId == artist.ArtistId);
                var unAttachedArtists = user.Settings.UnattachedArtists.ToList().Find(a => a.ArtistId == artist.ArtistId);
                bool? toggleOnOf = null;
                if (attachedArtist != null)
                {
                    toggleOnOf = true;
                }
                else if (unAttachedArtists != null)
                {
                    toggleOnOf = false;
                }
                SearchItemModel result = new SearchItemModel
                {
                    Id = artist.ArtistId,
                    Label = artist.Name,
                    Type = Specification.Enumerations.SearchItemTypes.Artist,
                    Setting = toggleOnOf
                };
                results.Add(result);
            }

            return results;
        }
        
        private List<SearchItemModel> SearchGenres(string keywords, IUser user)
        {
            var matchingGenres = _GenreBusiness.Search(keywords);
            List<SearchItemModel> results = new List<SearchItemModel>();

            foreach (var genre in matchingGenres)
            {
                var attachedGenre = user.Settings.AttachedGenres.ToList().Find(g => g.GenreId == genre.GenreId);
                var unAttachedGenre = user.Settings.UnattachedGenres.ToList().Find(g => g.GenreId == genre.GenreId);
                bool? toggleOnOf = null;
                if (attachedGenre != null)
                {
                    toggleOnOf = true;
                }
                else if (unAttachedGenre != null)
                {
                    toggleOnOf = false;
                }
                SearchItemModel result = new SearchItemModel
                {
                    Id = genre.GenreId,
                    Label = genre.Name,
                    Type = Specification.Enumerations.SearchItemTypes.Genre,
                    Setting = toggleOnOf
                };
                results.Add(result);
            }

            return results;
        }
    }
}
