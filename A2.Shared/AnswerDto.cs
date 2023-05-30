using Newtonsoft.Json;

namespace A2.Shared;

public class AnswerDto
{
    [JsonProperty("answer")]
    public List<int> Answer { get; set; }
    
    [JsonProperty("time_taken")]
    public long? TimeTaken { get; set; }
}