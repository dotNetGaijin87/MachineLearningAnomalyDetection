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
        if (model.Options is null)
        {
            return BadRequest($"{nameof(model.Options)} is required.");
        }

        var predictions = _srCnnTrainer.Run(model.TrainingData, model.Options);

        return predictions
            .Select(x => new SrCnnOutput
            {
                IsAnomaly = x.Prediction[0],
                RawScore = x.Prediction[1],
                Mag = x.Prediction[2],
            })
            .ToList();
    }
}
