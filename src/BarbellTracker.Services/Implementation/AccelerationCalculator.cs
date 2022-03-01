using BarbellTracker.AbstractionCode;
using BarbellTracker.Adapter.Model;
using BarbellTracker.DomainCode;
using BarbellTracker.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbellTracker.Services.Implementation
{
    public class AccelerationCalculator : IService, ICalculator<Acceleration>
    {
        public string Name => "Acceleration Calculator";

        public string Description => throw new NotImplementedException();

        private ServiceCache<Acceleration> cache;
        private ICalculator<Velocity> velocityCalculator;
        private object locker = new object();
        public AccelerationCalculator(ServiceCache<Acceleration> cache, ICalculator<Velocity> velocityCalculator)
        {
            this.cache = cache;
            this.velocityCalculator = velocityCalculator;
        }

        public Acceleration GetCalculatedValue
            (TrackedInformation trackedInfos)
        {
            lock (locker)
            {
                if (cache.TryGetCachedItem(trackedInfos, out Acceleration CachedAcceleration))
                {
                    return CachedAcceleration;
                }

                return cache.AddItemToCache(trackedInfos, CalculatAcceleration(trackedInfos));
            }
        }

        public Acceleration CalculatAcceleration(TrackedInformation trackedInformation)
        {
            var Velocity = velocityCalculator.GetCalculatedValue(trackedInformation);

            var AccelerationList = new List<Vector2D>();
            for (int i = 1; i < Velocity.Vectors.Length; i++)
            {
                AccelerationList.Add(Vector2D.Sub(Velocity.Vectors[i], Velocity.Vectors[i - 1]));
            }

            return new Acceleration() { FPS = trackedInformation.FrameRate, Vectors = AccelerationList.ToArray() };
        }
    }
}
