using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Saga.DI;
using Saga.Specification.Interfaces;
using Saga.Specification.Interfaces.Tags;
using Saga.Specification.Interfaces.Users;

namespace Saga.Music.AspMvc4.Controllers.api
{
    public class TagSettingController : ApiController
    {
        private readonly IUserBusiness _UserBusiness;
        private readonly ITagBusiness _TagBusiness;
        private readonly IUtility _Utility;

        public TagSettingController()
        {
            _TagBusiness = Factory.GetTagBusiness();
            _UserBusiness = Factory.GetUserBusiness();
            _Utility = Factory.GetUtility();
        }

        // GET api/tagsetting/{0}?code={1}&include={2}
        public string Get(int id, string code, bool? include)
        {
            ITag tag = _TagBusiness.GetAll().FirstOrDefault(g => g.TagId == id);
            int userId = _Utility.GetUserIdFromCode(code);
            if (userId < 1) { return "-1"; }

            ISettings settings = _UserBusiness.Get(userId).Settings;

            _UserBusiness.ExcludedSlideShowTagSetting_Remove(settings, tag);
            _UserBusiness.IncludedSlideShowTagSetting_Remove(settings, tag);

            if (!include.HasValue)
            {
                return "cleared " + tag.Label + " from this station ";
            }

            if (include.Value)
            {
                _UserBusiness.IncludedSlideShowTagSetting_Add(settings, tag);
                return "included " + tag.Label + " from this station ";
            }
            else if (!include.Value)
            {
                _UserBusiness.ExcludedSlideShowTagSetting_Add(settings, tag);
                return "excluded " + tag.Label + " from this station ";
            }
            return "done";
        }
    }
}
