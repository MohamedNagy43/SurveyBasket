
using SurveyBasket.Api.Persistence;

namespace SurveyBasket.Api.Services;

public class PollService(ApplicationDbContect context) : IPollService
{
    private readonly ApplicationDbContect _context = context;

    public async Task<IEnumerable<Poll>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Polls.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<Poll?> GetAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Polls.FindAsync(id, cancellationToken);

    public async Task<Poll> AddAsync(Poll poll, CancellationToken cancellationToken = default)
    {
        await _context.AddAsync(poll, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return poll;
    }

    public async Task<bool> UpdateAsync(int id, Poll poll, CancellationToken cancellationToken = default)
    {
        var InMemeoryPoll = await GetAsync(id, cancellationToken);

        if (InMemeoryPoll is null)
            return false;

        InMemeoryPoll.Title = poll.Title;
        InMemeoryPoll.Summary = poll.Summary;
        InMemeoryPoll.StartAt = poll.StartAt;
        InMemeoryPoll.EndAt = poll.EndAt;

        _context.Polls.Update(InMemeoryPoll);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await GetAsync(id, cancellationToken);

        if (poll is null)
            return false;

        _context.Polls.Remove(poll);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> TogglePublishStatusAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await GetAsync(id, cancellationToken);

        if (poll is null)
            return false;

        poll.IsPublished = !poll.IsPublished;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
