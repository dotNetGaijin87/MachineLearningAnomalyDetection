namespace AIModels.SrCNN;

/// <summary>
/// Options for SR-CNN anomaly detection. Owned by this library so the public API contract
/// never exposes ML.NET's option types — the trainer maps this onto ML.NET's
/// <c>SrCnnEntireAnomalyDetectorOptions</c> internally.
/// </summary>
public class SrCnnOptions
{
    /// <summary>Anomaly threshold in [0, 1]; a point scoring above it is flagged.</summary>
    public double Threshold { get; set; } = 0.3;

    /// <summary>Points processed per batch; -1 treats the whole series as a single batch.</summary>
    public int BatchSize { get; set; } = 2000;

    /// <summary>Boundary sensitivity in [0, 100]; higher widens the expected margin.</summary>
    public double Sensitivity { get; set; } = 70;

    public AnomalyDetectionMode DetectMode { get; set; } = AnomalyDetectionMode.AnomalyOnly;

    /// <summary>Series period; 0 lets the detector infer it.</summary>
    public int Period { get; set; }

    public SeasonalityMode DeseasonalityMode { get; set; } = SeasonalityMode.Stl;
}

/// <summary>What the SR-CNN detector emits for each point.</summary>
public enum AnomalyDetectionMode
{
    /// <summary>Anomaly flag, raw score and magnitude only.</summary>
    AnomalyOnly,

    /// <summary>Also emit the expected value and the upper/lower boundary margin.</summary>
    AnomalyAndMargin,

    /// <summary>Also emit the expected value.</summary>
    AnomalyAndExpectedValue,
}

/// <summary>How seasonality is removed from the series before detection.</summary>
public enum SeasonalityMode
{
    Stl,
    Mean,
    Median,
}
