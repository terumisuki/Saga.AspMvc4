using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Saga.DI;
using Saga.Specification.Interfaces;
using Saga.Specification.Interfaces.Users;

namespace Saga.Music.AspMvc4.Controllers.api
{
    public class TrackPlayedController : ApiController
    {
        private readonly IUserBusiness _UserBusiness;
        private readonly IUtility _Utility;

        public TrackPlayedController()
        {
            _UserBusiness = Factory.GetUserBusiness();
            _Utility = Factory.GetUtility();
        }

        // GET api/trackplayed?id=<track id>&code=<code>
        public string Get(int id, string code)
        {
            try
            {
                int userId = _Utility.GetUserIdFromCode(code);
                if (userId < 1) { throw new Exception("access denied"); }

                _UserBusiness.Played(userId, id);

                return "recorded that track " + id + " was played";
            }
            catch (Exception e)
            {
                return e.Message + " <<:...........:>> " + e.StackTrace;
            }
        }
    }
}
