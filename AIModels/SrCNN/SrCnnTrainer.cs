using Microsoft.ML;
using Microsoft.ML.TimeSeries;

namespace AIModels.SrCNN;

public class SrCnnTrainer
{
    private readonly MLContext _mlContext;
    private readonly string _outputColumnName = nameof(SrCnnTrainerOutput.Prediction);
    private readonly string _inputColumnName = nameof(SrCnnTrainerInput.Value);

    public SrCnnTrainer(MLContext mlContext)
    {
        _mlContext = mlContext;
    }

    public IEnumerable<SrCnnTrainerOutput> Run(IEnumerable<SrCnnTrainerInput> input, SrCnnOptions options)
    {
        var dataView = _mlContext.Data.LoadFromEnumerable(input);
        var outputDataView = _mlContext.AnomalyDetection.DetectEntireAnomalyBySrCnn(
            dataView, _outputColumnName, _inputColumnName, ToDetectorOptions(options));

        return _mlContext.Data.CreateEnumerable<SrCnnTrainerOutput>(outputDataView, reuseRowObject: false);
    }

    private static SrCnnEntireAnomalyDetectorOptions ToDetectorOptions(SrCnnOptions options) => new()
    {
        Threshold = options.Threshold,
        BatchSize = options.BatchSize,
        Sensitivity = options.Sensitivity,
        Period = options.Period,
        DetectMode = options.DetectMode switch
        {
            AnomalyDetectionMode.AnomalyAndMargin => SrCnnDetectMode.AnomalyAndMargin,
            AnomalyDetectionMode.AnomalyAndExpectedValue => SrCnnDetectMode.AnomalyAndExpectedValue,
            _ => SrCnnDetectMode.AnomalyOnly,
        },
        DeseasonalityMode = options.DeseasonalityMode switch
        {
            SeasonalityMode.Mean => SrCnnDeseasonalityMode.Mean,
            SeasonalityMode.Median => SrCnnDeseasonalityMode.Median,
            _ => SrCnnDeseasonalityMode.Stl,
        },
    };
}
