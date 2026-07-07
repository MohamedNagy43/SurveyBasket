namespace SurveyBasket.Api.Services;

public class ResultService(ApplicationDbContext context) : IResultService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Result<PollVoteResponse>> GetPollVotesAsync(int pollId, CancellationToken cancellationToken = default)
    {

        var PollVotesRespons =
            await _context.Polls
            .Where(x => x.Id == pollId)
            .AsNoTracking()
            .Select(p => new PollVoteResponse(
                p.Title,
                p.Votes.Select(v => new VoteResponse(
                    v.User.FirstName + " " + v.User.LastName,
                    v.SubmittedOn,
                    v.VoteAnswers.Select(x => new QuestionAnswerResponse(
                        x.Question.Content,
                        x.Answer.Content
                    ))
                ))
            )).SingleOrDefaultAsync(cancellationToken);


        return PollVotesRespons is null ?
             Result.Failure<PollVoteResponse>(Errors<Poll>.NotFound)
             :
             Result.Success(PollVotesRespons);
    }

    public async Task<Result<IEnumerable<VotesPerDayResponse>>> GetVotesPerDayAsync(int pollId, CancellationToken cancellationToken = default)
    {
        var isPollExist = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken);

        if (!isPollExist)
            return Result.Failure<IEnumerable<VotesPerDayResponse>>(Errors<Poll>.NotFound);


        var respnse =
            await _context.Votes.Where(x => x.PollId == pollId)
            .AsNoTracking()
            .GroupBy(x => DateOnly.FromDateTime(x.SubmittedOn))
            .Select(group => new VotesPerDayResponse(
                group.Key,
                group.Count()
            )).ToListAsync(cancellationToken);

        return Result.Success(respnse.AsEnumerable());
    }

    public async Task<Result<IEnumerable<VotesPerQuestionResponse>>> GetVotesPerQuestionsAsync(int pollId, CancellationToken cancellationToken = default)
    {
        var isPollExist = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken);

        if (!isPollExist)
            return Result.Failure<IEnumerable<VotesPerQuestionResponse>>(Errors<Poll>.NotFound);

        var response =
            await _context.VoteAnswers
            .Where(x => x.Vote.PollId == pollId)
            .AsNoTracking()
            .GroupBy(x => x.Question.Content)
            .Select(questionGroup => new VotesPerQuestionResponse(
                questionGroup.Key,
                questionGroup.GroupBy(x => x.Answer.Content)
                .Select(answerGroup => new VotesPerAnswerResponse(
                    answerGroup.Key,
                    answerGroup.Count()
                ))
            )).ToListAsync(cancellationToken);

        return Result.Success(response.AsEnumerable());

    }
}
