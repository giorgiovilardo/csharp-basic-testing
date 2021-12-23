using System;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace MyApp.Web.Tests;

public class WebApiTests : IClassFixture<WebApiFactory>
{
    private readonly HttpClient _client;

    public WebApiTests(WebApiFactory factory)
    {
        _client = factory.Client;
    }

    [Theory]
    [InlineData(1, 2, 3)]
    [InlineData(2, 2, 4)]
    [InlineData(2, 3, 5)]
    [InlineData(10, 10, 20)]
    public async void TestAdd(int num1, int num2, int result)
    {
        var apiResponse = await _client.PostAsJsonAsync("/add", new TwoNumbersRequestDto(num1, num2));
        var unpackedResult = await apiResponse.Content.ReadFromJsonAsync<CalculatorResponseDto>();
        Assert.Equal(result, unpackedResult!.Result);
    }
}

public class WebApiFactory : IDisposable
{
    public readonly HttpClient Client;

    public WebApiFactory()
    {
        var webApp = new WebApplicationFactory<Program>();
        Client = webApp.Server.CreateClient();
    }

    public void Dispose()
    {
    }
}
