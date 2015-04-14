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
    public class UserController : ApiController
    {
        private readonly IUserBusiness _UserBusiness;
        private readonly IUtility _Utility;

        public UserController()
        {
            _UserBusiness = Factory.GetUserBusiness();
            _Utility = Factory.GetUtility();
        }

        // GET api/user?username=<username>&password=<password>
        public HttpResponseMessage Get(string username, string password)
        {
            try
            {
                IUser user = _UserBusiness.Get(username, password);
                if (user.UserId < 1) { return null; }

                string code = _Utility.GetUserCodeFromId(user.UserId);
                if (code == "notfound") { return null; }

                HttpResponseMessage mess = new HttpResponseMessage()
                {
                    Content = new StringContent(code)
                };
                return mess;
            }
            catch 
            {
                return null;
            }
        }
        
        //// GET api/user?username=<username>&password=<password>
        //public string Get(string username, string password)
        //{
        //    try
        //    {
        //        IUser user = _UserBusiness.Get(username, password);
        //        if (user.UserId < 1) { return null; }

        //        string code = _Utility.GetUserCodeFromId(user.UserId);
        //        if (code == "notfound") { return null; }

        //        return code;
        //    }
        //    catch (Exception e)
        //    {
        //        return null;
        //    }
        //}
    }
}
