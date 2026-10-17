using System;
using BarbellTracker.DomainCode;

namespace BarbellTracker.WPF_DesktopClient.ViewModel
{
    internal static class TrackedInformationExtensions
    {
        /// <summary>
        /// The services cache their results by the id of the tracked information.
        /// A copy with a new id is calculated again, e.g. after the analysis settings were changed.
        /// </summary>
        public static TrackedInformation WithNewId(this TrackedInformation info)
        {
            return new TrackedInformation()
            {
                Id = $"{info.Id}#{Guid.NewGuid():N}",
                Name = info.Name,
                Positions = info.Positions,
                FrameRate = info.FrameRate,
                PixelPerCm = info.PixelPerCm,
            };
        }
    }
}
