using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Interfaces.Audio;
using Saga.DI;
using Saga.Specification.Interfaces;
using Saga.Specification.Interfaces.Audio;
using Saga.Specification.Interfaces.Users;

namespace Saga.Music.AspMvc4.Controllers.api
{
    public class NextTrackController : ApiController
    {
        private readonly IUserBusiness _UserBusiness;
        private readonly IUtility _Utility;

        public NextTrackController()
        {
            _UserBusiness = Factory.GetUserBusiness();
            _Utility = Factory.GetUtility();
        }

        // GET api/nexttrack?code=<code>
        public TrackModel Get(string code)
        {
            try
            {
                int userId = _Utility.GetUserIdFromCode(code);
                if (userId < 1) { return null; }

                ITrack track = _UserBusiness.RandomAudioTrack_Get(userId);
                var trackModel = Helper.Convert_ITrack_To_TrackModel(track);
                return trackModel;
            }
            catch (Exception e)
            {
                _UserBusiness.LogError(e);
                TrackModel errorTrack = Helper.GetTrackModel();
                errorTrack.MediaId = -1;
                errorTrack.Title = e.Message;
                errorTrack.MediaFilePath = e.StackTrace;
                return errorTrack;
            }
        }
    }
}
