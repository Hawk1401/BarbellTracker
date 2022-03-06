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
    public class VelocityCSVTranslater : IService, ICSVTranslater<VectorCSVModel>
    {
        public string Name => "Velocity Calculator";
        public string Description => "This service will Calculat the Velocity of the Moving barbell";

        private ServiceCache<VelocityCSVModel> cache;
        private ICalculator<Velocity> velocityCalculator;

        public VelocityCSVTranslater(ICalculator<Velocity> velocityCalculator, ServiceCache<VelocityCSVModel> cache)
        {
            this.velocityCalculator = velocityCalculator;
            this.cache = cache;
        }

        public VectorCSVModel GetCSV(TrackedInformation trackedInfos)
        {
            if (cache.TryGetCachedItem(trackedInfos, out var CachedCSV))
            {
                return CachedCSV;
            }

            var Velocity= velocityCalculator.GetCalculatedValue(trackedInfos);
            var CSV = CreateCSV(Velocity);

            cache.AddItemToCache(trackedInfos, CSV);
            return CSV;
        }

        public VelocityCSVModel CreateCSV(Velocity velocity)
        {
            double TimeStep = 1d / velocity.FPS;
            VelocityCSVModel CSVVelocityModel = new VelocityCSVModel();

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
