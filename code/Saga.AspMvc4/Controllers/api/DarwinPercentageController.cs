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
    public class DarwinPercentageController : ApiController
    {
        private readonly IUtility _Utility;

        public DarwinPercentageController()
        {
            _Utility = Factory.GetUtility();
        }

        // GET api/darwinpercentage?percentage={0}&code=<code>
        public string Get(int percentage, string code)
        {
            try
            {
                int userId = _Utility.GetUserIdFromCode(code);
                if (userId < 1) { return null; }

                IUser user = Factory.GetUser(userId);
                if (user == null)
                {
                    return "";
                }

                IUserBusiness userBusiness = Factory.GetUserBusiness();
                userBusiness.DarwinPercentage_Set(user.Settings, percentage);

                return "set " + user.Username + " dariwn percentage to " + percentage;
            }
            catch (Exception e)
            {
                return "!!! " + e.Message + " ..... " + e.StackTrace + " .... " + e.InnerException;
            }
        }
    }
}
