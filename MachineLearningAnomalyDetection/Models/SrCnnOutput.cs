using AIModels.SrCNN;
using Microsoft.ML.TimeSeries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MachineLearningAnomalyDetection.Models
{
    public class SrCnnOutput
    {
        public double IsAnomaly { get; set; }
        public double RawScore { get; set; }
        public double Mag { get; set; }
    }
}
