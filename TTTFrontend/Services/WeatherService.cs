using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using static TTTFrontend.Pages.Weather;
using Shared.Models;

public class WeatherService
{
    private readonly HttpClient _httpClient;

    public WeatherService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<WeatherForecast>> GetWeatherDataAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<WeatherForecast>>("/weatherforecast");
    }
}