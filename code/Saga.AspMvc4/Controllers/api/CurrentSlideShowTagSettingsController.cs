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
    public class CurrentSlideShowTagSettingsController : ApiController
    {
        private readonly IUserBusiness _UserBusiness;
        private readonly IUtility _Utility;


        public CurrentSlideShowTagSettingsController()
        {
            _UserBusiness = Factory.GetUserBusiness();
            _Utility = Factory.GetUtility();
        }

        // GET api/currentslideshowtagsettings/?code={0}
        public List<SearchItemModel> Get(string code)
        {
            int userId = _Utility.GetUserIdFromCode(code);
            if (userId < 1) { return null; }
            
            List<SearchItemModel> results = new List<SearchItemModel>();
            
            var user = _UserBusiness.Get(userId);

            #region genres
            // included tags
            var includedTags = user.Settings.IncludedSlideShowTags;
            foreach (var tag in includedTags)
            {
                SearchItemModel result = new SearchItemModel
                {
                    Id = tag.TagId,
                    Label = tag.Label,
                    Type = Specification.Enumerations.SearchItemTypes.Tag,
                    Setting = true
                };
                results.Add(result);
            }


            // excluded tags
            var excludedTags = user.Settings.ExcludedSlideShowTags;
            foreach (var tag in excludedTags)
            {
                SearchItemModel result = new SearchItemModel
                {
                    Id = tag.TagId,
                    Label = tag.Label,
                    Type = Specification.Enumerations.SearchItemTypes.Tag,
                    Setting = false
                };
                results.Add(result);
            }
            #endregion
            
            return results;
        }
    }
}
