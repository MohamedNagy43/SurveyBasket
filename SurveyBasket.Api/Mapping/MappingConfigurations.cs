using SurveyBasket.Api.Contracts.Responses;

namespace SurveyBasket.Api.Mapping;

public class MappingConfigurations : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Poll, PollResponse>()
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.Title, src => src.Title, src => src.Title != null) // mapping with source Condition
            .TwoWays();  
    }
}
