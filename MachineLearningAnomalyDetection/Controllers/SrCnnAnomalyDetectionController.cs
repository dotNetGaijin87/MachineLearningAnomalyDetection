using AIModels.SrCNN;
using MachineLearningAnomalyDetection.Models;
using Microsoft.AspNetCore.Mvc;

namespace MachineLearningAnomalyDetection.Controllers;

[ApiController]
public class SrCnnAnomalyDetectionController : ControllerBase
{
    private readonly ILogger<SrCnnAnomalyDetectionController> _logger;
    private readonly SrCnnTrainer _srCnnTrainer;

    public SrCnnAnomalyDetectionController(SrCnnTrainer srCnnTrainer, ILogger<SrCnnAnomalyDetectionController> logger)
    {
        _logger = logger;
        _srCnnTrainer = srCnnTrainer;
    }

    [HttpPost("[controller]:Run")]
    public ActionResult<IEnumerable<SrCnnOutput>> Run(SrCnnInput model)
    {
        var options = model.Options ?? new SrCnnOptions();

        IEnumerable<SrCnnTrainerOutput> predictions;
        try
        {
            predictions = _srCnnTrainer.Run(model.TrainingData, options);
        }
        // Backstop for inputs validation doesn't pre-check: map the detector's rejection to a
        // clean 400 instead of letting it surface as a 500 with an internal stack trace.
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            _logger.LogWarning(ex, "SR-CNN detection rejected the request.");
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }

        return predictions
            .Select(x => new SrCnnOutput
            {
                IsAnomaly = x.IsAnomaly,
                RawScore = x.RawScore,
                Mag = x.Mag,
            })
            .ToList();
    }
}
