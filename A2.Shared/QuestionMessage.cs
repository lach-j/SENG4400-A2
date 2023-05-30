using Newtonsoft.Json;

namespace A2.Shared;

public class QuestionMessage
{
    [JsonProperty("question")]
    public string Question { get; set; }
}