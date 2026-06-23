# Machine Learning Anomaly Detection

An ASP.NET Core Web API that detects anomalies in time-series data using the
**SR-CNN (Spectral Residual – Convolutional Neural Network)** algorithm from
[ML.NET](https://dotnet.microsoft.com/apps/machinelearning-ai/ml-dotnet).

The API wraps ML.NET's `DetectEntireAnomalyBySrCnn` so you can POST a series of
numeric values and get back, for each point, whether it is an anomaly along with
the raw anomaly score and magnitude.

## How it works

SR-CNN is an unsupervised anomaly-detection technique: you don't need a labelled
training set. It transforms the time series into the spectral-residual domain to
highlight points that deviate from the expected pattern, then scores each point.
This makes it a good fit for monitoring metrics such as request rates, latency,
CPU usage, sales figures, or any other ordered numeric signal.

## Project structure

| Project | Type | Description |
| --- | --- | --- |
| [`AIModels`](AIModels/) | Class library (`net5.0`) | Wraps the ML.NET SR-CNN detector. [`SrCnnTrainer`](AIModels/SrCNN/SrCnnTrainer.cs) runs `DetectEntireAnomalyBySrCnn` over the supplied data. |
| [`MachineLearningAnomalyDetection`](MachineLearningAnomalyDetection/) | ASP.NET Core Web API (`net5.0`) | Exposes the detector over HTTP with Swagger UI. |

Key types:

- [`SrCnnTrainerInput`](AIModels/SrCNN/SrCnnTrainerInput.cs) — a single `Value` in the series.
- [`SrCnnTrainerOutput`](AIModels/SrCNN/SrCnnTrainerOutput.cs) — the raw `Prediction` vector returned by ML.NET.
- [`SrCnnInput`](MachineLearningAnomalyDetection/Models/SrCnnInput.cs) — the request body (detector `Options` + `TrainingData`).
- [`SrCnnOutput`](MachineLearningAnomalyDetection/Models/SrCnnOutput.cs) — the response (`IsAnomaly`, `RawScore`, `Mag`) per point.

## Requirements

- [.NET 5 SDK](https://dotnet.microsoft.com/download/dotnet/5.0)

NuGet dependencies (restored automatically on build):

- `Microsoft.ML` 1.6.0
- `Microsoft.ML.TimeSeries` 1.6.0
- `Swashbuckle.AspNetCore` 5.6.3

## Getting started

```bash
# restore and build
dotnet build

# run the API
dotnet run --project MachineLearningAnomalyDetection
```

Once running, open the Swagger UI to explore and try the API:

- https://localhost:5001/swagger
- http://localhost:5000/swagger

## API

### `POST /SrCnnAnomalyDetection:Run`

Detects anomalies across the entire supplied series.

**Request body**

```json
{
  "options": {
    "threshold": 0.3,
    "batchSize": -1,
    "sensitivity": 64,
    "detectMode": "AnomalyAndMargin",
    "period": 0,
    "deseasonalityMode": "Stl"
  },
  "trainingData": [
    { "value": 1 },
    { "value": 2 },
    { "value": 3 },
    { "value": 10 },
    { "value": 1 },
    { "value": 1 },
    { "value": 1 },
    { "value": 1 },
    { "value": 1 },
    { "value": 1 },
    { "value": 1 },
    { "value": 1 }
  ]
}
```

`options` maps to ML.NET's [`SrCnnEntireAnomalyDetectorOptions`](https://learn.microsoft.com/dotnet/api/microsoft.ml.timeseries.srcnnentireanomalydetectoroptions).
`trainingData` is the ordered list of values to evaluate.

**Response**

A list with one entry per input point:

```json
[
  { "isAnomaly": 0, "rawScore": 0.0,  "mag": 0.0 },
  { "isAnomaly": 0, "rawScore": 0.0,  "mag": 0.0 },
  { "isAnomaly": 1, "rawScore": 0.85, "mag": 0.9 }
]
```

| Field | Description |
| --- | --- |
| `isAnomaly` | `1` if the point is flagged as an anomaly, otherwise `0`. |
| `rawScore` | The raw anomaly score for the point. |
| `mag` | The spectral-residual magnitude for the point. |

### Example request

```bash
curl -k -X POST "https://localhost:5001/SrCnnAnomalyDetection:Run" \
  -H "Content-Type: application/json" \
  -d '{
        "options": { "threshold": 0.3, "sensitivity": 64, "detectMode": "AnomalyAndMargin" },
        "trainingData": [
          {"value":1},{"value":2},{"value":3},{"value":10},
          {"value":1},{"value":1},{"value":1},{"value":1}
        ]
      }'
```

## License

No license has been specified for this repository.
