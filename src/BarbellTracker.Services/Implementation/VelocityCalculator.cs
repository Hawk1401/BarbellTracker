using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbellTracker.AbstractionCode;
using BarbellTracker.Adapter.Interface;
using BarbellTracker.ApplicationCode;
using BarbellTracker.DomainCode;
using BarbellTracker.Services.Interface;
using BarbellTracker.Services.Models;

namespace BarbellTracker.Services.Implementation
{
    public class VelocityCalculator : IService
    {


        public string Name => "Velocity Calculator";
        public string Description => "This service will Calculat the Velocity of the Moving barbell";

        public static VelocityCalculator Instance = new VelocityCalculator();

        private Dictionary<TrackedInformation, Velocity> cache;


        private object locker;

        private VelocityCalculator()
        {
            cache = new Dictionary<TrackedInformation, Velocity>();
        }

        public Velocity GetVelocity(TrackedInformation trackedInfos)
        {
            lock (locker)
            {
                if (!cache.ContainsKey(trackedInfos))
                {
                    cache.Add(trackedInfos, CalculatVelocity(trackedInfos));
                    keepCacheClean();
                }

                return cache[trackedInfos];
            }
        }

        private Velocity CalculatVelocity(TrackedInformation eventContext)
        {
            List<Vector2D> velocityList = new List<Vector2D>();

            for (int i = 1; i < eventContext.Positions.Length; i++)
            {
                velocityList.Add(Vector2D.Sub(eventContext.Positions[i],eventContext.Positions[i - 1]));
            }

            return new Velocity() { FPS = eventContext.FrameRate, Vectors = velocityList.ToArray() };
        }

        private void keepCacheClean()
        {
            if(cache.Count > 4)
            {
                cache.Remove(cache.First().Key);
            }
        }
    }
}
