﻿using BarbellTracker.Adapter.Model;
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

        private ServiceCache<VectorCSVModel> cache;
        private ICalculator<Velocity> velocityCalculator;

        public VelocityCSVTranslater(ICalculator<Velocity> velocityCalculator, ServiceCache<VectorCSVModel> cache)
        {
            this.velocityCalculator = velocityCalculator;
            this.cache = cache;
        }

        public VectorCSVModel GetCSV(TrackedInformation trackedInfos)
        {
            var Velocity= velocityCalculator.GetCalculatedValue(trackedInfos);
            var CSV = CreateCSV(Velocity);

            return CSV;
        }

        public VectorCSVModel CreateCSV(Velocity velocity)
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
