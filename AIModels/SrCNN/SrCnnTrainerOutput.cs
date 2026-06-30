using Microsoft.ML.Data;

namespace AIModels.SrCNN;

public class SrCnnTrainerOutput
{
    [VectorType]
    public double[] Prediction { get; set; } = default!;
    public double IsAnomaly => Prediction[0];
    public double RawScore => Prediction[1];
    public double Mag => Prediction[2];
}
