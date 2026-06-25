namespace AIModels.SrCNN;

/// <summary>Series-length bounds enforced before data reaches the SR-CNN detector.</summary>
public static class SrCnnLimits
{
    /// <summary>ML.NET's SR-CNN detector requires at least this many points.</summary>
    public const int MinPoints = 12;

    /// <summary>Upper bound enforced at the API boundary to cap per-request cost.</summary>
    public const int MaxPoints = 100_000;
}
