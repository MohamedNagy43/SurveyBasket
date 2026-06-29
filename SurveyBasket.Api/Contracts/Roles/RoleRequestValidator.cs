using FluentValidation;
using System.Text.Json;

namespace SurveyBasket.Api.Contracts.Roles;

public class RoleRequestValidator : AbstractValidator<RoleRequest>
{
    private readonly List<string> _namedPermissions = Permissions.GetAllPermissions();

    public RoleRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(1, 200);

        RuleFor(x => x.Permissions)
            .NotEmpty();


        RuleFor(x => x.Permissions)
            .Must(x => x.Count == x.Distinct().Count()).When(x => x.Permissions is not null)
            .WithMessage("you can not add dublicated values");

    }
}
