using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbellTracker.Adapter.Model;
using BarbellTracker.DomainCode;
using BarbellTracker.Services.Interface;

namespace BarbellTracker.Services.Implementation
{
    public class AccelerationCSVTranslater : IService, ICSVTranslater<VectorCSVModel>
    {
        public string Name => throw new NotImplementedException();

        public string Description => throw new NotImplementedException();

        private ServiceCache<VectorCSVModel> cache;
        private ICalculator<Acceleration> accelerationCalculator;

        public AccelerationCSVTranslater(ICalculator<Acceleration> accelerationCalculator, ServiceCache<VectorCSVModel> cache)
        {
            this.cache = cache;
            this.accelerationCalculator = accelerationCalculator;

        }

        public VectorCSVModel GetCSV(TrackedInformation trackedInfos)
        {
            if(cache.TryGetCachedItem(trackedInfos, out var cachedCSV))
            {
                return cachedCSV;
            }
            var Velocity = accelerationCalculator.GetCalculatedValue(trackedInfos);
            var CSV = CreateCSV(Velocity);

            cache.AddItemToCache(trackedInfos, CSV);
            return CSV;
        }

        public VectorCSVModel CreateCSV(Acceleration velocity)
        {
            double TimeStep = 1d / velocity.FPS;
            VectorCSVModel CSVVelocityModel = new VectorCSVModel();

            for (int i = 0; i < velocity.Vectors.Length; i++)
            {
                var time = TimeSpan.FromSeconds(i * TimeStep).ToString(@"mm\:ss\:FF");
                var length = velocity.Vectors[i].length();
                var vektor = velocity.Vectors[i].ToString();

                CSVVelocityModel.AddItem(time, length, vektor);
            }

            return CSVVelocityModel;
        }
    }
}
