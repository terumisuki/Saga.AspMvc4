using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Interfaces.Audio;
using Saga.Specification.Interfaces.Audio;

namespace Saga.Music.AspMvc4
{
    public class Helper
    {
        internal static TrackModel Convert_ITrack_To_TrackModel(ITrack inTrack)
        {
            TrackModel outTrack = new TrackModel()
            {
                MediaId = inTrack.MediaId,
                MediaFilePath = inTrack.MediaFilePath,
                Title = inTrack.Title
            };
            return outTrack;
        }

        internal static TrackModel GetTrackModel()
        {
            TrackModel outTrack = new TrackModel();
            return outTrack;
        }
    }
}