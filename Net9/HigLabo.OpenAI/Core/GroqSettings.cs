namespace HigLabo.OpenAI;

public class GroqSettings
{
    public string ApiKey { get; set; } = "";

    public GroqSettings() { }
    public GroqSettings(string apiKey)
    {
        this.ApiKey = apiKey;
    }
}
