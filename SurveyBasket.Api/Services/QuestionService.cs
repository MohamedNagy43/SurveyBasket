using Microsoft.Extensions.Caching.Hybrid;

namespace SurveyBasket.Api.Services;

public class QuestionService(ApplicationDbContext context, HybridCache hybridCache) : IQuestionService
{
    private readonly ApplicationDbContext _context = context;
    private readonly HybridCache _hybridCache = hybridCache;
    private readonly string _CachePrefix = "availableQuestions";

    public async Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId, CancellationToken cancellationToken = default)
    {
        bool isPollExist = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);

        if (!isPollExist)
            return Result.Failure<IEnumerable<QuestionResponse>>(Errors<Poll>.NotFound);

        var response =
            await _context.Questions.Where(q => q.PollId == pollId)
            .Include(q => q.Answers)
            .ProjectToType<QuestionResponse>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result.Success(response.AsEnumerable());
    }
    public async Task<Result<IEnumerable<QuestionResponse>>> GetAvailableAsync(int pollId, string userId, CancellationToken cancellationToken = default)
    {
        bool isPollAvailable = await _context.Polls.AnyAsync(x =>
        x.Id == pollId
        && x.IsPublished
        && x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow)
        && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);

        if (!isPollAvailable)
            return Result.Failure<IEnumerable<QuestionResponse>>(Errors<Poll>.NotFound);

        bool hasVotedBefore = await _context.Votes.AnyAsync(x => x.PollId == pollId && x.UserId == userId, cancellationToken);

        if (hasVotedBefore)
            return Result.Failure<IEnumerable<QuestionResponse>>(VoteErrors.DuplicatedVote);

        // cache response
        string cacheKey = $"{_CachePrefix}-{pollId}";

        var CachedQuestions = await _hybridCache.GetOrCreateAsync<IEnumerable<QuestionResponse>>(cacheKey, async CacheEntry =>
               await _context.Questions.Where(q => q.PollId == pollId && q.IsActive)
               .Include(q => q.Answers)
               .AsNoTracking()
               .ProjectToType<QuestionResponse>()
               .ToListAsync(cancellationToken),
              cancellationToken: cancellationToken
        );

        return Result.Success(CachedQuestions.AsEnumerable());

    }
    public async Task<Result<QuestionResponse>> GetAsync(int pollId, int id, CancellationToken cancellationToken = default)
    {
        var response =
            await _context.Questions.Where(x => x.Id == id && x.PollId == pollId)
            .Include(q => q.Answers)
            .ProjectToType<QuestionResponse>()
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);

        return response is null ?
            Result.Failure<QuestionResponse>(Errors<Question>.NotFound)
            : Result.Success(response);
    }
    public async Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken = default)
    {
        bool isPollExist = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);

        if (!isPollExist)
            return Result.Failure<QuestionResponse>(Errors<Poll>.NotFound);

        bool isQuestionExistForThisPoll = _context.Questions.Any(q => q.PollId == pollId && q.Content == request.Content);

        if (isQuestionExistForThisPoll)
            return Result.Failure<QuestionResponse>(QuestionErrors.DuplicatedContent);

        var newQuestion = request.Adapt<Question>();
        newQuestion.PollId = pollId;

        await _context.AddAsync(newQuestion, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // remove cache
        await _hybridCache.RemoveAsync($"{_CachePrefix}-{pollId}", cancellationToken);

        return Result.Success(newQuestion.Adapt<QuestionResponse>());
    }
    public async Task<Result<QuestionResponse>> UpdateAsync(int pollId, int id, QuestionRequest request, CancellationToken cancellationToken = default)
    {
        var question = await _context.Questions
            .Include(x => x.Answers)
            .SingleOrDefaultAsync(x => x.Id == id && x.PollId == pollId, cancellationToken);

        if (question is null)
            return Result.Failure<QuestionResponse>(Errors<Question>.NotFound);

        bool isQuestionExistForThisPoll
            = await _context.Questions
            .AnyAsync(x => x.Content == request.Content && x.PollId == pollId && x.Id != id, cancellationToken);

        if (isQuestionExistForThisPoll)
            return Result.Failure<QuestionResponse>(QuestionErrors.DuplicatedContent);

        //update
        question.Content = request.Content;

        var currentAnswers = question.Answers.Select(x => x.Content).ToList();

        // newAnswers
        var newAnswers = request.Answers.Except(currentAnswers).ToList();

        newAnswers.ForEach(answer =>
        {
            question.Answers.Add(new Answer { Content = answer });
        });

        // Activition
        question.Answers.ToList().ForEach(answer =>
        {
            answer.IsActive = request.Answers.Contains(answer.Content);
        });


        await _context.SaveChangesAsync(cancellationToken);


        // remove cache
        await _hybridCache.RemoveAsync($"{_CachePrefix}-{pollId}", cancellationToken);

        return Result.Success(question.Adapt<QuestionResponse>());
    }
    public async Task<Result<QuestionResponse>> ToggleStatusAsync(int pollId, int id, CancellationToken cancellationToken = default)
    {
        var question =
            await _context.Questions.Include(x => x.Answers)
            .SingleOrDefaultAsync(x => x.Id == id && x.PollId == pollId, cancellationToken);
        if (question is null)
            return Result.Failure<QuestionResponse>(Errors<Question>.NotFound);

        question.IsActive = !question.IsActive;
        await _context.SaveChangesAsync(cancellationToken);

        // remove cache
        await _hybridCache.RemoveAsync($"{_CachePrefix}-{pollId}", cancellationToken);

        return Result.Success(question.Adapt<QuestionResponse>());
    }

}
