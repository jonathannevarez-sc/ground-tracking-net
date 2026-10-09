using System.Text.Json;
using GroundRoutes.Api.Models;
using Xunit;

namespace GroundRoutes.Api.Tests;

public sealed class GroundRoutePayloadValidatorTests
{
    [Theory]
    [InlineData("{\"name\":\"SYN-Route\"}", true)]
    [InlineData("{}", false)]
    [InlineData("[]", false)]
    [InlineData("null", false)]
    public void IsValid_RequiresANonEmptyJsonObject(string json, bool expected)
    {
        using var document = JsonDocument.Parse(json);

        Assert.Equal(expected, GroundRoutePayloadValidator.IsValid(document.RootElement));
    }
}
