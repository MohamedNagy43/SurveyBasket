namespace SurveyBasket.Api.Entities;

public class Vote
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int PollId { get; set; }
    public DateTime SubmittedOn { get; set; } = DateTime.Now;

    public ApplicationUser User { get; set; } = default!;
    public Poll Poll { get; set; } = default!;
    public ICollection<VoteAnswer> VoteAnswers { get; set; } = [];
}
