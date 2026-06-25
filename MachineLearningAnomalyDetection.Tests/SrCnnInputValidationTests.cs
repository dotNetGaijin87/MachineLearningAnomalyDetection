using System.ComponentModel.DataAnnotations;
using AIModels.SrCNN;
using MachineLearningAnomalyDetection.Models;
using Xunit;

namespace MachineLearningAnomalyDetection.Tests;

public class SrCnnInputValidationTests
{
    private static List<SrCnnTrainerInput> Series(int count)
    {
        var list = new List<SrCnnTrainerInput>(count);
        for (var i = 0; i < count; i++)
        {
            list.Add(new SrCnnTrainerInput { Value = 1 });
        }

        return list;
    }

    private static IReadOnlyList<string> Validate(SrCnnInput input)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(input, new ValidationContext(input), results, validateAllProperties: true);
        return results.Select(r => r.ErrorMessage ?? string.Empty).ToList();
    }

    [Fact]
    public void Valid_input_passes()
    {
        var input = new SrCnnInput { TrainingData = Series(12) };

        Assert.Empty(Validate(input));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public void Too_few_points_fails(int count)
    {
        var input = new SrCnnInput { TrainingData = Series(count) };

        Assert.Contains(Validate(input), m => m.Contains("at least 12"));
    }

    [Fact]
    public void Too_many_points_fails()
    {
        var input = new SrCnnInput { TrainingData = Series(SrCnnLimits.MaxPoints + 1) };

        Assert.Contains(Validate(input), m => m.Contains("must not exceed"));
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void Non_finite_value_fails(double bad)
    {
        var data = Series(12);
        data[5].Value = bad;
        var input = new SrCnnInput { TrainingData = data };

        Assert.Contains(Validate(input), m => m.Contains("finite"));
    }

    [Fact]
    public void Threshold_out_of_range_fails()
    {
        var input = new SrCnnInput
        {
            TrainingData = Series(12),
            Options = new SrCnnOptions { Threshold = 1.5 },
        };

        Assert.Contains(Validate(input), m => m.Contains("threshold"));
    }

    [Fact]
    public void Sensitivity_out_of_range_fails()
    {
        var input = new SrCnnInput
        {
            TrainingData = Series(12),
            Options = new SrCnnOptions { Sensitivity = 150 },
        };

        Assert.Contains(Validate(input), m => m.Contains("sensitivity"));
    }

    [Theory]
    [InlineData(5, true)]
    [InlineData(-1, false)]
    [InlineData(12, false)]
    public void BatchSize_rule(int batchSize, bool shouldFail)
    {
        var input = new SrCnnInput
        {
            TrainingData = Series(12),
            Options = new SrCnnOptions { BatchSize = batchSize },
        };

        var failed = Validate(input).Any(m => m.Contains("batchSize"));

        Assert.Equal(shouldFail, failed);
    }
}
