using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbellTracker.AbstractionCode;
using BarbellTracker.Adapter.Interface;
using BarbellTracker.Adapter.Model;
using BarbellTracker.ApplicationCode;
using BarbellTracker.DomainCode;
using BarbellTracker.Services.Interface;

namespace BarbellTracker.Services.Implementation
{
    public class VelocityCalculator : IService, ICalculator<Velocity>
    {


        public string Name => "Velocity Calculator";
        public string Description => "This service will Calculat the Velocity of the Moving barbell";

        private ServiceCache<Velocity> cache;
        private object locker = new object();

        public VelocityCalculator(ServiceCache<Velocity> cache)
        {
            this.cache = cache;
        }

        public Velocity GetCalculatedValue(TrackedInformation trackedInfos)
        {
            lock (locker)
            {
                if (cache.TryGetCachedItem(trackedInfos, out Velocity CachedVelocity))
                {
                    return CachedVelocity;
                }

                return cache.AddItemToCache(trackedInfos, CalculatVelocity(trackedInfos));
            }
        }

        public Velocity CalculatVelocity(TrackedInformation trackedInformation)
        {
            List<Vector2D> velocityList = new List<Vector2D>();

            for (int i = 1; i < trackedInformation.Positions.Length; i++)
            {
                velocityList.Add(Vector2D.Sub(trackedInformation.Positions[i], trackedInformation.Positions[i - 1]));
            }

            return new Velocity() { FPS = trackedInformation.FrameRate, Vectors = velocityList.ToArray() };
        }
    }
}
