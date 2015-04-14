using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Saga.DI;
using Saga.Specification.Interfaces;
using Saga.Specification.Interfaces.Musical;
using Saga.Specification.Interfaces.Users;

namespace Saga.Music.AspMvc4.Controllers.api
{
    public class PartSettingController : ApiController
    {
        private readonly IUserBusiness _UserBusiness = null;
        private readonly IPartBusiness _PartBusiness = null;
        private readonly IUtility _Utility;

        public PartSettingController()
        {
            _UserBusiness = Factory.GetUserBusiness();
            _PartBusiness = Factory.GetPartsBusiness();
            _Utility = Factory.GetUtility();
        }


        // GET api/partsetting/{0}?code={1}&include={2}
        public string Get(int id, string code, bool? include)
        {
            IPartBase part = _PartBusiness.GetAll().FirstOrDefault(p => p.PartId == id);

            int userId = _Utility.GetUserIdFromCode(code);
            if (userId < 1) { return null; }

            ISettings settings = _UserBusiness.Get(userId).Settings;

            _UserBusiness.ExcludedPartSetting_Remove(settings, part);
            _UserBusiness.IncludedPartSetting_Remove(settings, part);

            if (!include.HasValue)
            {
                return "cleared " + part.Name + " from this station ";
            }

            if (include.Value)
            {
                _UserBusiness.IncludedPartSetting_Add(settings, part);
                return "included " + part.Name + " from this station ";
            }
            else if (!include.Value)
            {
                _UserBusiness.ExcludedPartSetting_Add(settings, part);
                return "excluded " + part.Name + " from this station ";
            }
            return "done";
        }
    }
}
