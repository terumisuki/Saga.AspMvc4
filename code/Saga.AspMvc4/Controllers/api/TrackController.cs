using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Saga.DI;
using Saga.Specification.Interfaces;
using Saga.Specification.Interfaces.Audio;

namespace Saga.Music.AspMvc4.Controllers.api
{
    public class TrackController : ApiController
    {
        private readonly ITrackBusiness _TrackBusiness;
        private readonly IUtility _Utility;

        public TrackController()
        {
            _TrackBusiness = Factory.GetTrackBusiness();
            _Utility = Factory.GetUtility();
        }

        // GET api/track/5?code=<user code>
        public HttpResponseMessage Get(int id, string code)
        {
            int userId = _Utility.GetUserIdFromCode(code);
            if (userId < 1) { throw new Exception("access denied"); };

            ITrack track = _TrackBusiness.Get(id);

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(track.MediaFilePath, FileMode.Open, FileAccess.Read);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("audio/mpeg");
            return result;
        }
    }
}
