using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Saga.DI;
using Saga.Music.Interfaces.Models;
using Saga.Specification.Interfaces;
using Saga.Specification.Interfaces.Tags;
using Saga.Specification.Interfaces.Users;

namespace Saga.Music.AspMvc4.Controllers.api
{
    public class SearchTagsController : ApiController
    {
        private readonly IUserBusiness _UserBusiness;
        private readonly ITagBusiness _TagBusiness;
        private readonly IUtility _Utility;

        public SearchTagsController()
        {
            _UserBusiness = Factory.GetUserBusiness();
            _TagBusiness = Factory.GetTagBusiness();
            _Utility = Factory.GetUtility();
        }


        // GET api/searchtags/5?code=<user code>
        public List<SearchItemModel> Get(string id, string code)
        {
            int userId = _Utility.GetUserIdFromCode(code);
            if (userId < 1) { return null; }

            string keywords = id;
            var user = _UserBusiness.Get(userId);

            List<SearchItemModel> results = GetActiveTags(user);
            
            return results;
        }

        private List<SearchItemModel> GetActiveTags(IUser user)
        {
            var activeTags = _TagBusiness.GetActiveTags();
            List<SearchItemModel> results = new List<SearchItemModel>();

            foreach (var tag in activeTags)
            {
                var attachedTag = user.Settings.IncludedSlideShowTags.ToList().Find(t => t.TagId == tag.TagId);
                var unAttachedTag = user.Settings.ExcludedSlideShowTags.ToList().Find(t => t.TagId == tag.TagId);
                bool? toggleOnOf = null;
                if (attachedTag != null)
                {
                    toggleOnOf = true;
                }
                else if (unAttachedTag != null)
                {
                    toggleOnOf = false;
                }
                SearchItemModel result = new SearchItemModel
                {
                    Id = tag.TagId,
                    Label = tag.Label,
                    Type = Specification.Enumerations.SearchItemTypes.Tag,
                    Setting = toggleOnOf
                };
                results.Add(result);
            }

            return results;
        }

        private List<SearchItemModel> SearchTags(string keywords, IUser user)
        {
            var matchingTags = _TagBusiness.Search(keywords);
            List<SearchItemModel> results = new List<SearchItemModel>();

            foreach (var tag in matchingTags)
            {
                var attachedTag = user.Settings.IncludedSlideShowTags.ToList().Find(t => t.TagId == tag.TagId);
                var unAttachedTag = user.Settings.ExcludedSlideShowTags.ToList().Find(t => t.TagId == tag.TagId);
                bool? toggleOnOf = null;
                if (attachedTag != null)
                {
                    toggleOnOf = true;
                }
                else if (unAttachedTag != null)
                {
                    toggleOnOf = false;
                }
                SearchItemModel result = new SearchItemModel
                {
                    Id = tag.TagId,
                    Label = tag.Label,
                    Type = Specification.Enumerations.SearchItemTypes.Tag,
                    Setting = toggleOnOf
                };
                results.Add(result);
            }

            return results;
        }
    }
}
