using Microsoft.ML.Data;

namespace AIModels.SrCNN;

public class SrCnnTrainerOutput
{
    [VectorType]
    public double[] Prediction { get; set; } = default!;
}
