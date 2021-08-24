using AIModels.SrCNN;
using Microsoft.ML.TimeSeries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MachineLearningAnomalyDetection.Models
{
    public class SrCnnInput
    {
        public SrCnnEntireAnomalyDetectorOptions Options { get; set; }

        public List<SrCnnTrainerInput> TrainingData { get; set; } 
            = new List<SrCnnTrainerInput>()
            {
                    new SrCnnTrainerInput { Value = 1 },
                    new SrCnnTrainerInput { Value = 2 },
                    new SrCnnTrainerInput { Value = 3 },
                    new SrCnnTrainerInput { Value = 10 },
                    new SrCnnTrainerInput { Value = 1 },
                    new SrCnnTrainerInput { Value = 1 },
                    new SrCnnTrainerInput { Value = 1 },
                    new SrCnnTrainerInput { Value = 1 },
                    new SrCnnTrainerInput { Value = 1 },
                    new SrCnnTrainerInput { Value = 1 },
                    new SrCnnTrainerInput { Value = 1 },
                    new SrCnnTrainerInput { Value = 1 },
            };
    }
}
