namespace Orchestra.ATS.Domain;

public class Application
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public Guid CandidateId { get; set; }
    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

    public Job? Job { get; set; }
    public Candidate? Candidate { get; set; }
}