using AutoMapper;
using Domain.ApiModels;
using Domain.Interfaces;
using Domain.Interfaces.Infrastructure;

namespace Domain.Services;

public class WeatherForecastService : IWeatherForecastService
{
    private readonly IWeatherForecastProxy _serviceProxy;
    private readonly IMapper _mapper;

    public WeatherForecastService(IWeatherForecastProxy serviceProxy, IMapper mapper)
    {
        _serviceProxy=serviceProxy;
        _mapper=mapper;
    }

    public async Task<WeatherForecast?> GetAsync(CancellationToken cancellationToken)
    {
        var forecast = await _serviceProxy.GetAsync(cancellationToken);

        return _mapper.Map<ApiModels.WeatherForecast>(forecast);
    }
}
