using BarbellTracker.Adapter.Model;
using BarbellTracker.DomainCode;
using BarbellTracker.Services.Interface;

namespace BarbellTracker.Services.Implementation
{
    public class SetCSVTranslater : IService, ICSVTranslater<SetCSVModel>
    {
        public string Name => "Set CSV Translater";
        public string Description => "This service translates the repetition metrics of a set into a CSV model";

        private readonly ServiceCache<SetCSVModel> cache;
        private readonly ICalculator<SetAnalysis> analyzer;

        public SetCSVTranslater(ICalculator<SetAnalysis> analyzer, ServiceCache<SetCSVModel> cache)
        {
            this.analyzer = analyzer;
            this.cache = cache;
        }

        public SetCSVModel GetCSV(TrackedInformation trackedInfos)
        {
            if (cache.TryGetCachedItem(trackedInfos, out var cached))
            {
                return cached;
            }

            return cache.AddItemToCache(trackedInfos, CreateCSV(analyzer.GetCalculatedValue(trackedInfos)));
        }

        public SetCSVModel CreateCSV(SetAnalysis analysis)
        {
            var model = new SetCSVModel();
            foreach (var rep in analysis.Repetitions)
            {
                model.AddRepetition(rep);
            }
            return model;
        }
    }
}
