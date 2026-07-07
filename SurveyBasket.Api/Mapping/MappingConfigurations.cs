using SurveyBasket.Api.Contracts.Users;

namespace SurveyBasket.Api.Mapping;

public class MappingConfigurations : IRegister
{

    public void Register(TypeAdapterConfig config)
    {
        // Question
        config.NewConfig<QuestionRequest, Question>()
            .Map(dest => dest.Answers, src => src.Answers.Select(answer => new Answer { Content = answer }));

        config.NewConfig<Question, QuestionResponse>()
            .Map(dest => dest.Answers, src => src.Answers.Where(x => x.IsActive));

        // Application User
        config.NewConfig<RegisterRequest, ApplicationUser>()
            .Map(dest => dest.UserName, src => src.Email.Substring(0, src.Email.IndexOf('@')));

        config.NewConfig<CreateUserRequest, ApplicationUser>()
            .Map(dest => dest.UserName, src => src.Email.Substring(0, src.Email.IndexOf('@')))
            .Map(dest => dest.EmailConfirmed, src => true);

        config.NewConfig<UpdateUserRequest, ApplicationUser>()
            .Map(dest => dest.UserName, src => src.Email.Substring(0, src.Email.IndexOf('@')))
            .Map(dest => dest.NormalizedUserName, src => src.Email.Substring(0, src.Email.IndexOf('@')).ToUpper())
            .Map(dest => dest.EmailConfirmed, src => true);
    }
}
