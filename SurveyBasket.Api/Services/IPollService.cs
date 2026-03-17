using System.Linq.Expressions;

namespace SurveyBasket.Api.Services;

public interface IPollService
{
    Task<Result<IEnumerable<PollResponse>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<PollResponse>>> GetCurrentAsync(CancellationToken cancellationToken = default);
    Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<PollResponse>> GetAsync(Expression<Func<Poll,bool>> criteria, CancellationToken cancellationToken = default);
    Task<Result<PollResponse>> AddAsync(PollRequest poll, CancellationToken cancellationToken = default);
    Task<Result<PollResponse>> UpdateAsync(int id, PollRequest poll, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<PollResponse>> TogglePublishStatusAsync(int id, CancellationToken cancellationToken = default);
}
