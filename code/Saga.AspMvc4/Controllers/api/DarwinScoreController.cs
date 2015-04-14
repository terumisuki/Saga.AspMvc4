using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Saga.DI;
using Saga.Specification.Interfaces;
using Saga.Specification.Interfaces.Audio;
using Saga.Specification.Interfaces.Users;

namespace Saga.Music.AspMvc4.Controllers.api
{
    public class DarwinScoreController : ApiController
    {
        private readonly IUserBusiness _UserBusiness;
        private readonly IUtility _Utility;

        public DarwinScoreController()
        {
            _UserBusiness = Factory.GetUserBusiness();
            _Utility = Factory.GetUtility();
        }

        // GET api/darwinscore?mediaId=<mediaId>&secondsPlayed=<secondsPlayed>&blessed=<true / false>&code=<code>
        public string Get(int mediaId, int secondsPlayed, bool blessed, string code)
        {
            try
            {
                int userId = _Utility.GetUserIdFromCode(code);
                if (userId < 1) { return null; }
                
                ITrack track = Factory.GetTrack(mediaId);
                IUser user = Factory.GetUser(userId);
                double? darwinScore = _UserBusiness.AudioSkipped(track, blessed, secondsPlayed, user);
                return "track (" + mediaId + ") given darwin score of " + darwinScore;
            }
            catch (Exception e)
            {
                return "!!! " + e.Message + " ..... " + e.StackTrace + " .... " + e.InnerException;
            }
        }
    }
}
