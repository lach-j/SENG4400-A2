using A2.Shared;

namespace A2.Dashboard.Data;

public class AnswerService
{
    private const int MaxSize = 50;
    public List<AnswerDto> RecentAnswers { get; private set; } = new();

    public void AddAnswer(AnswerDto answer)
    {
        Console.WriteLine(RecentAnswers.Count);
        if (RecentAnswers.Count >= MaxSize)
        {
            RecentAnswers = RecentAnswers.TakeLast(MaxSize-1).ToList();
        }

        RecentAnswers.Add(answer);
    }
}