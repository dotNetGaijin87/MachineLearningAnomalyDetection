using System.ComponentModel.DataAnnotations;
using AIModels.SrCNN;

namespace MachineLearningAnomalyDetection.Models;

public class SrCnnInput : IValidatableObject
{
    public SrCnnOptions? Options { get; set; }

    public List<SrCnnTrainerInput> TrainingData { get; set; } = new()
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

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var data = TrainingData;

        if (data is null || data.Count < SrCnnLimits.MinPoints)
        {
            yield return Error(
                $"trainingData must contain at least {SrCnnLimits.MinPoints} points.",
                nameof(TrainingData));
        }
        else if (data.Count > SrCnnLimits.MaxPoints)
        {
            yield return Error(
                $"trainingData must not exceed {SrCnnLimits.MaxPoints} points.",
                nameof(TrainingData));
        }
        else
        {
            for (var i = 0; i < data.Count; i++)
            {
                if (double.IsNaN(data[i].Value) || double.IsInfinity(data[i].Value))
                {
                    yield return Error($"trainingData[{i}].value must be a finite number.", nameof(TrainingData));
                    break;
                }
            }
        }

        if (Options is { } options)
        {
            if (options.Threshold is < 0 or > 1)
            {
                yield return Error("options.threshold must be between 0 and 1.", $"{nameof(Options)}.{nameof(SrCnnOptions.Threshold)}");
            }

            if (options.Sensitivity is < 0 or > 100)
            {
                yield return Error("options.sensitivity must be between 0 and 100.", $"{nameof(Options)}.{nameof(SrCnnOptions.Sensitivity)}");
            }

            if (options.Period < 0)
            {
                yield return Error("options.period must be 0 or greater.", $"{nameof(Options)}.{nameof(SrCnnOptions.Period)}");
            }

            if (options.BatchSize != -1 && options.BatchSize < SrCnnLimits.MinPoints)
            {
                yield return Error(
                    $"options.batchSize must be -1 or at least {SrCnnLimits.MinPoints}.",
                    $"{nameof(Options)}.{nameof(SrCnnOptions.BatchSize)}");
            }
        }
    }

    private static ValidationResult Error(string message, string member) => new(message, new[] { member });
}
