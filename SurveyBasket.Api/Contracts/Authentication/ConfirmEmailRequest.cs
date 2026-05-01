using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace SurveyBasket.Api.Contracts.Authentication;

public record ConfirmEmailRequest(
    string UserId,
    string Code
);
