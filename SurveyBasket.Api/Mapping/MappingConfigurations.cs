namespace SurveyBasket.Api.Mapping;

public class MappingConfigurations : IRegister
{

    public void Register(TypeAdapterConfig config)
    {
        // Polls
        config.NewConfig<PollRequest, Poll>()
           .Map(dest => dest.Title, src => src.Title)
           .Map(dest => dest.Summary, src => src.Summary)
           .Map(dest => dest.StartsAt, src => src.StartsAt)
           .Map(dest => dest.EndsAt, src => src.EndsAt)
           .IgnoreNonMapped(true)
           .IgnoreNullValues(true);
    }
}
