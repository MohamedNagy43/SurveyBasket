using SurveyBasket.Api.Contracts.Questions;

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
           .IgnoreNonMapped(true);

        // Question
        config.NewConfig<QuestionRequest, Question>()
            .Map(dest => dest.Answers, src => src.Answers.Select(answer => new Answer { Content = answer }));

        config.NewConfig<Question, QuestionResponse>()
            .Map(dest => dest.Answers, src => src.Answers.Where(x => x.IsActive));

        // Application User
        config.NewConfig<RegisterRequest, ApplicationUser>()
            .Map(dest => dest.UserName, src => src.Email.Substring(0,src.Email.IndexOf('@')))
            ;
    }
}
