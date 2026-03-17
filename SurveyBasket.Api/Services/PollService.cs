
using System.Linq.Expressions;

namespace SurveyBasket.Api.Services;

public class PollService(ApplicationDbContext context) : IPollService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Result<IEnumerable<PollResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var response = await _context.Polls.AsNoTracking().ToListAsync(cancellationToken);
        return Result.Success(response.Adapt<IEnumerable<PollResponse>>());
    }
    public async Task<Result<IEnumerable<PollResponse>>> GetCurrentAsync(CancellationToken cancellationToken = default)
    {
        var response =
            await _context.Polls
            .Where(x => x.IsPublished
            && x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow)
            && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow))
            .ProjectToType<PollResponse>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result.Success(response.AsEnumerable());
    }

    public async Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        Poll? poll = await _context.Polls.FindAsync(id, cancellationToken);

        return (poll is not null) ?
            Result.Success(poll.Adapt<PollResponse>())
            :
            Result.Failure<PollResponse>(Errors<Poll>.NotFound);
    }

    public async Task<Result<PollResponse>> GetAsync(Expression<Func<Poll, bool>> criteria, CancellationToken cancellationToken = default)
    {
        Poll? poll = await _context.Polls.FirstOrDefaultAsync(criteria, cancellationToken);

        return (poll is not null) ?
            Result.Success(poll.Adapt<PollResponse>())
            :
            Result.Failure<PollResponse>(Errors<Poll>.NotFound);
    }

    public async Task<Result<PollResponse>> AddAsync(PollRequest request, CancellationToken cancellationToken = default)
    {
        bool IsExistTitle = await _context.Polls.AnyAsync(p => p.Title == request.Title, cancellationToken);
        if (IsExistTitle)
            return Result.Failure<PollResponse>(PollErrors.DuplicatedTitle);

        Poll poll = request.Adapt<Poll>();

        await _context.AddAsync(poll, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(poll.Adapt<PollResponse>());
    }

    public async Task<Result<PollResponse>> UpdateAsync(int id, PollRequest request, CancellationToken cancellationToken = default)
    {
        Poll? currentPoll = await _context.Polls.FindAsync(id);
        if (currentPoll is null)
            return Result.Failure<PollResponse>(Errors<Poll>.NotFound);


        bool IsExistTitle = await _context.Polls.AnyAsync(p => p.Title == request.Title && p.Id != id);
        if (IsExistTitle)
            return Result.Failure<PollResponse>(PollErrors.DuplicatedTitle);


        request.Adapt(destination: currentPoll);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(currentPoll.Adapt<PollResponse>());
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {

        Poll? currentPoll = await _context.Polls.FindAsync(id);
        if (currentPoll is null)
            return Result.Failure(Errors<Poll>.NotFound);


        _context.Polls.Remove(currentPoll);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<PollResponse>> TogglePublishStatusAsync(int id, CancellationToken cancellationToken = default)
    {
        Poll? currentPoll = await _context.Polls.FindAsync(id);
        if (currentPoll is null)
            return Result.Failure<PollResponse>(Errors<Poll>.NotFound);

        currentPoll.IsPublished = !currentPoll.IsPublished;
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(currentPoll.Adapt<PollResponse>());
    }

}
