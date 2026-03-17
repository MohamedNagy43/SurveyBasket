using SurveyBasket.Api.Contracts.Votes;

namespace SurveyBasket.Api.Services;

public class VoteService(ApplicationDbContext context) : IVoteService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Result> AddAsync(int pollId, string userId, VoteRequest request, CancellationToken cancellationToken)
    {
        bool isPollAvailable = await _context.Polls.AnyAsync(x =>x.Id == pollId&& x.IsPublished&& x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow)&& x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);

        if (!isPollAvailable)
            return Result.Failure(Errors<Poll>.NotFound);

        bool hasVotedBefore = await _context.Votes.AnyAsync(x => x.PollId == pollId && x.UserId == userId, cancellationToken);

        if (hasVotedBefore)
            return Result.Failure(VoteErrors.DuplicatedVote);

        // Check questions Exist in this poll
        var availableQuestions = await _context.Questions
            .Where(x => x.PollId == pollId && x.IsActive)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        bool isEqual = availableQuestions.SequenceEqual(request.Answers.Select(x => x.QuestionId));

        if (!isEqual)
            return Result.Failure(VoteErrors.InvalidQuestions);

        // check Answers Exist in Every Question Id

        foreach (var voteAnswer in request.Answers)
        {
            var isExistAnswer =
                await _context.Answers.AnyAsync(x => x.Id == voteAnswer.AnswerId && x.QuestionId == voteAnswer.QuestionId, cancellationToken);

            if (!isExistAnswer)
                return Result.Failure(VoteErrors.InvalidAnswers);
        }

        // Adding
        var vote = new Vote
        {
            PollId = pollId,
            UserId = userId,
            VoteAnswers = request.Answers.Select(x => new VoteAnswer { QuestionId = x.QuestionId, AnswerId = x.AnswerId }).ToList(),
        };

        await _context.AddAsync(vote, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
