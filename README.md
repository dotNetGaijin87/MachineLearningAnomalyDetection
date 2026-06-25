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
| [`AIModels`](AIModels/) | Class library (`net8.0`) | Wraps the ML.NET SR-CNN detector. [`SrCnnTrainer`](AIModels/SrCNN/SrCnnTrainer.cs) runs `DetectEntireAnomalyBySrCnn` over the supplied data. |
| [`MachineLearningAnomalyDetection`](MachineLearningAnomalyDetection/) | ASP.NET Core Web API (`net8.0`) | Exposes the detector over HTTP with Swagger UI. Bootstrapped via the minimal hosting model in [`Program.cs`](MachineLearningAnomalyDetection/Program.cs). |
| [`MachineLearningAnomalyDetection.Tests`](MachineLearningAnomalyDetection.Tests/) | xUnit test project (`net8.0`) | Validation unit tests, trainer/behaviour tests, and in-memory API acceptance tests (`WebApplicationFactory`). |

Key types:

- [`SrCnnTrainerInput`](AIModels/SrCNN/SrCnnTrainerInput.cs) — a single `Value` in the series.
- [`SrCnnTrainerOutput`](AIModels/SrCNN/SrCnnTrainerOutput.cs) — the raw `Prediction` vector returned by ML.NET.
- [`SrCnnInput`](MachineLearningAnomalyDetection/Models/SrCnnInput.cs) — the request body (detector `Options` + `TrainingData`).
- [`SrCnnOutput`](MachineLearningAnomalyDetection/Models/SrCnnOutput.cs) — the response (`IsAnomaly`, `RawScore`, `Mag`) per point.

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (LTS)

NuGet dependencies (restored automatically on build):

- `Microsoft.ML` 5.0.0
- `Microsoft.ML.TimeSeries` 5.0.0
- `Swashbuckle.AspNetCore` 10.2.3

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

`trainingData` is the ordered list of values to evaluate.

`options` is **optional** — omit it to use the defaults below. It is the API's own
[`SrCnnOptions`](AIModels/SrCNN/SrCnnOptions.cs) contract (not an ML.NET type), mapped onto the
detector internally so the API stays decoupled from the ML library:

| Field | Type | Default | Description |
| --- | --- | --- | --- |
| `threshold` | number | `0.3` | Anomaly threshold in `[0, 1]`; points scoring above it are flagged. |
| `batchSize` | integer | `2000` | Points processed per batch; `-1` treats the whole series as one batch. |
| `sensitivity` | number | `70` | Boundary sensitivity in `[0, 100]`; higher widens the expected margin. |
| `detectMode` | string | `"AnomalyOnly"` | One of `AnomalyOnly`, `AnomalyAndMargin`, `AnomalyAndExpectedValue`. |
| `period` | integer | `0` | Series period; `0` lets the detector infer it. |
| `deseasonalityMode` | string | `"Stl"` | One of `Stl`, `Mean`, `Median`. |

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

**Validation & errors**

The request is validated at the boundary before it reaches the detector. Invalid input
returns `400 Bad Request` with a problem-details body (never a `500` with a stack trace):

- `trainingData` must contain between **12** and **100,000** points (SR-CNN requires at least 12).
- every `value` must be a finite number (no `NaN` / `Infinity`).
- `threshold` ∈ `[0, 1]`, `sensitivity` ∈ `[0, 100]`, `period` ≥ `0`, and `batchSize` must be `-1` or ≥ `12`.

### Example request

```bash
curl -k -X POST "https://localhost:5001/SrCnnAnomalyDetection:Run" \
  -H "Content-Type: application/json" \
  -d '{
        "options": { "threshold": 0.3, "sensitivity": 64, "detectMode": "AnomalyAndMargin" },
        "trainingData": [
          {"value":1},{"value":2},{"value":3},{"value":10},
          {"value":1},{"value":1},{"value":1},{"value":1},
          {"value":1},{"value":1},{"value":1},{"value":1}
        ]
      }'
```

## Running the tests

```bash
dotnet test
```

The suite covers boundary validation, the trainer's behaviour and enum mapping, and the
HTTP endpoint end-to-end (hosted in memory via `WebApplicationFactory`).

## License

No license has been specified for this repository.
