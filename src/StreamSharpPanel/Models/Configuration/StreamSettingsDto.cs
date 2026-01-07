using System.Text.Json.Serialization;

namespace StreamSharpPanel.Models.Configuration;


public class StreamSettingsDto
{
    public StreamInfoDto[] Data { get; init; } = [];
}

public class StreamInfoDto
{
    public string BroadcasterId { get; init; } = null!;
    public string BroadcasterLogin { get; init; } = null!;
    public string BroadcasterName { get; init; } = null!;
    public string BroadcasterLanguage { get; init; } = null!;

    public string GameId { get; init; } = "";
    public string GameName { get; init; } = "";

    public string Title { get; init; } = "";
    public List<string> Tags { get; init; } = [];

    public int Delay { get; init; }

    public string[] ContentClassificationLabels { get; init; } = [];
    public bool IsBrandedContent { get; init; }
}


public class StreamInfo
{
    internal StreamInfo(StreamInfoDto dto)
    {

        Title = dto.Title;
        GameId = dto.GameId;
        Tags = dto.Tags;
        BroadcasterLanguage = dto.BroadcasterLanguage;
        
        //Delay = dto.Delay;
        IsBrandedContent = dto.IsBrandedContent;

        foreach (var label in ContentClassificationLabels.IntersectBy(dto.ContentClassificationLabels, x => x.Id))
        {
            label.IsEnabled = true;
        }
    }

    public string Title { get; set; } = "";
    public string GameId { get; set; } = "";
    public string BroadcasterLanguage { get; set; } = null!;
    public List<string> Tags { get; set; } = [];
    public bool IsBrandedContent { get; set; }

    // todo: how to deal with partner-only property?
    //public int Delay { get; set; }

    public ContentClassificationLabel[] ContentClassificationLabels { get; init; } =
    [
        ContentClassificationLabel.DebatedSocialIssuesAndPolitics(),
        ContentClassificationLabel.DrugsIntoxication(),
        ContentClassificationLabel.SexualThemes(),
        ContentClassificationLabel.ViolentGraphic(),
        ContentClassificationLabel.Gambling(),
        ContentClassificationLabel.ProfanityVulgarity(),
    ];
}

public record ContentClassificationLabel
{
    internal static ContentClassificationLabel DebatedSocialIssuesAndPolitics() => new("DebatedSocialIssuesAndPolitics");
    internal static ContentClassificationLabel DrugsIntoxication() => new("DrugsIntoxication");
    internal static ContentClassificationLabel SexualThemes() => new("SexualThemes");
    internal static ContentClassificationLabel ViolentGraphic() => new("ViolentGraphic");
    internal static ContentClassificationLabel Gambling() => new("Gambling");
    internal static ContentClassificationLabel ProfanityVulgarity() => new("ProfanityVulgarity");

    private ContentClassificationLabel(string label) => Id = label;

    public string Id { get; init; }
    public bool IsEnabled  { get; set; }

    public override string ToString() => Id switch
    {
        "DebatedSocialIssuesAndPolitics" => "Debated Social Issues And Politics",
        "DrugsIntoxication" => "Drugs Or Intoxication",
        "SexualThemes" => "Sexual Themes",
        "ViolentGraphic" => "Violent Graphic",
        "Gambling" => "Gambling",
        "ProfanityVulgarity" => "Profanity Or Vulgarity",
        _ =>  ""
    };
}
