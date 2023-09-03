using AutoMapper;

namespace Domain.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Entities.WeatherForecast, ApiModels.WeatherForecast>();
    }
}
