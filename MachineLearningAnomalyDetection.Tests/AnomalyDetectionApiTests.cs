using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace MachineLearningAnomalyDetection.Tests;

public class AnomalyDetectionApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string Route = "/SrCnnAnomalyDetection:Run";

    private readonly WebApplicationFactory<Program> _factory;

    public AnomalyDetectionApiTests(WebApplicationFactory<Program> factory) => _factory = factory;

    private static object SpikeRequest(object? options = null) => new
    {
        options,
        trainingData = new double[] { 1, 2, 3, 10, 1, 1, 1, 1, 1, 1, 1, 1 }
            .Select(v => new { value = v }),
    };

    private sealed record Prediction(double IsAnomaly, double RawScore, double Mag);

    [Fact]
    public async Task Detects_spike_and_returns_200()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync(Route, SpikeRequest());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var predictions = await response.Content.ReadFromJsonAsync<List<Prediction>>();
        Assert.NotNull(predictions);
        Assert.Equal(12, predictions!.Count);
        Assert.Equal(1d, predictions[3].IsAnomaly);
    }

    [Fact]
    public async Task Accepts_detect_mode_as_a_string_enum()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync(Route, SpikeRequest(new { detectMode = "AnomalyAndMargin" }));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Too_few_points_returns_400_not_500()
    {
        var client = _factory.CreateClient();
        var request = new { trainingData = new[] { new { value = 1.0 }, new { value = 2.0 } } };

        var response = await client.PostAsJsonAsync(Route, request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
