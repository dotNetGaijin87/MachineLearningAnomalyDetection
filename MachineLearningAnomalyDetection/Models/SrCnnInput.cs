using AIModels.SrCNN;

namespace MachineLearningAnomalyDetection.Models;

public class SrCnnInput
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
}
