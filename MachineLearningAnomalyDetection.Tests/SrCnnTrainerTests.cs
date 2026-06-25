using AIModels.SrCNN;
using Microsoft.ML;
using Xunit;

namespace MachineLearningAnomalyDetection.Tests;

public class SrCnnTrainerTests
{
    // index 3 is a clear spike against an otherwise flat series
    private static List<SrCnnTrainerInput> SpikeSeries() =>
        new double[] { 1, 2, 3, 10, 1, 1, 1, 1, 1, 1, 1, 1 }
            .Select(v => new SrCnnTrainerInput { Value = v })
            .ToList();

    [Fact]
    public void Flags_the_spike_and_nothing_else()
    {
        var trainer = new SrCnnTrainer(new MLContext());

        var result = trainer.Run(SpikeSeries(), new SrCnnOptions()).ToList();

        Assert.Equal(12, result.Count);
        Assert.Equal(1d, result[3].Prediction[0]);
        Assert.All(
            result.Where((_, i) => i != 3),
            r => Assert.Equal(0d, r.Prediction[0]));
    }

    [Fact]
    public void AnomalyAndMargin_mode_maps_through_to_a_seven_column_vector()
    {
        var trainer = new SrCnnTrainer(new MLContext());
        var options = new SrCnnOptions { DetectMode = AnomalyDetectionMode.AnomalyAndMargin };

        var result = trainer.Run(SpikeSeries(), options).ToList();

        // AnomalyOnly emits 3 columns; AnomalyAndMargin emits 7 — so a 7-wide
        // vector proves our owned enum was mapped onto ML.NET's SrCnnDetectMode.
        Assert.All(result, r => Assert.Equal(7, r.Prediction.Length));
    }
}
