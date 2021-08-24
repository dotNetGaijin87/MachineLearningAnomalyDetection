using Microsoft.ML;
using Microsoft.ML.TimeSeries;
using System.Collections.Generic;

namespace AIModels.SrCNN
{
    public class SrCnnTrainer
    {
        private MLContext _mlContext;
        string _outputColumnName = nameof(SrCnnTrainerOutput.Prediction);
        string _inputColumnName = nameof(SrCnnTrainerInput.Value);


        public SrCnnTrainer(MLContext mlContext)
        {
            _mlContext = mlContext;
        }

        public IEnumerable<SrCnnTrainerOutput> Run(IEnumerable<SrCnnTrainerInput> input, SrCnnEntireAnomalyDetectorOptions options)
        {
            var dataView = _mlContext.Data.LoadFromEnumerable(input);
            var outputDataView = _mlContext.AnomalyDetection.DetectEntireAnomalyBySrCnn(dataView, _outputColumnName, _inputColumnName, options);

            return _mlContext.Data.CreateEnumerable<SrCnnTrainerOutput>(outputDataView, reuseRowObject: false);
        }
    }
}
