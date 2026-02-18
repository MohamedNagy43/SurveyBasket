
namespace SurveyBasket.Api.Services;

public class PollService : IPollService
{
    private static readonly List<Poll> _polls = [
        new Poll(){Id=1,Title="title",Description="dis"}];


    public List<Poll> GetAll() => _polls;
    public Poll? Get(int id) => _polls.SingleOrDefault(pol => pol.Id == id);

    public Poll Add(Poll poll)
    {
        poll.Id = _polls.Count + 1;
        _polls.Add(poll);
        return poll;
    }

    public bool Update(int id, Poll poll)
    {
        var InMemeoryPoll = Get(id);

        if (InMemeoryPoll is not null)
        {
            InMemeoryPoll.Title = poll.Title;
            InMemeoryPoll.Description = poll.Description;
            return true;
        }
        return false;
    }

    public bool Delete(int id)
    {
        var InMemeoryPoll = Get(id);

        if (InMemeoryPoll is null)
            return false;

        return _polls.Remove(InMemeoryPoll);
    }
}
