using AIModels.SrCNN;
using MachineLearningAnomalyDetection.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MachineLearningAnomalyDetection.Controllers
{
    [ApiController]
    public class SrCnnAnomalyDetectionController : ControllerBase
    {

        private readonly ILogger<SrCnnAnomalyDetectionController> _logger;
        private readonly SrCnnTrainer _srCnntrainer;

        public SrCnnAnomalyDetectionController(SrCnnTrainer srCnntrainer, ILogger<SrCnnAnomalyDetectionController> logger)
        {
            _logger = logger;
            _srCnntrainer = srCnntrainer;
        }


        [HttpPost("[controller]:Run")]
        public IEnumerable<SrCnnOutput> Get(SrCnnInput model)
        {
 
            var predictions = _srCnntrainer.Run(model.TrainingData, model.Options);


            return predictions
                    .Select(x => new SrCnnOutput 
                    {
                        IsAnomaly = x.Prediction[0],
                        RawScore = x.Prediction[1],
                        Mag = x.Prediction[2],
                    })
                    .ToList();
        }
    }
}
