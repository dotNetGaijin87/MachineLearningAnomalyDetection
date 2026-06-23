namespace MachineLearningAnomalyDetection.Models;

public class SrCnnOutput
{
    public double IsAnomaly { get; set; }
    public double RawScore { get; set; }
    public double Mag { get; set; }
}
