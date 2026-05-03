using Hangfire;
using Microsoft.AspNetCore.Identity.UI.Services;
using SurveyBasket.Api.Helpers;

namespace SurveyBasket.Api.Services;

public class NotificationService(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    IEmailSender emailSender,
    IHttpContextAccessor httpContextAccessor) : INotificationService
{
    private readonly ApplicationDbContext _context = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task SendNewPollNotificationAsync(int? pollId = null)
    {
        IEnumerable<Poll> polls;

        if (pollId.HasValue)
        {
            var poll = await _context.Polls.SingleOrDefaultAsync(x => x.Id == pollId && x.IsPublished);
            polls = [poll!];
        }
        else
        {
            polls = await _context.Polls
                .Where(x => x.IsPublished && x.StartsAt == DateOnly.FromDateTime(DateTime.UtcNow))
                .AsNoTracking()
                .ToListAsync();
        }

        // send Notifications For Memebers
        var users = await _userManager.Users.Where(x => x.EmailConfirmed).ToListAsync();

        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;
        foreach (var user in users)
        {
            foreach (var poll in polls)
            {
                var body = await EmailBodyBuilder.BuildEmailBodyAsync("PollNotification", new Dictionary<string, string>
                {
                    {"{{name}}",user.FirstName},
                    {"{{pollTill}}",poll.Title},
                    {"{{endDate}}",poll.EndsAt.ToString()},
                    {"{{url}}",$"{origin}/polls/start/{poll.Id}"}, // front end direction
                });

                BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, $"SurveyBasket: new poll - {poll.Title}", body));
            }
        }

    }
}
