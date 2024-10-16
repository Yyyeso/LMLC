public enum GameMode
{
    Single,
    Multi,
}

public enum MultiMode
{
    Solo,
    Team,
}

public enum SceneType
{
    Intro,
    Game,
    Loading,
}

public enum ThemeType
{
    Pizza,
    Salad,
    Bibimbap,
    Bagel,
    Taco,
    Cake,
    Grill,
    MaxCount
}

public enum CharacterType
{
    Normal,
    Special
}

public static class Const
{
    public const string Intro = "Intro";
    public const string Game = "Game";
    public const string Loading = "Loading";

    public const string Single = "Single";
    public const string Multi = "Multi";

    public const string Pizza = "Pizza";
    public const string Salad = "Salad";
    public const string Bibimbap = "Bibimbap";
    public const string Bagel = "Bagel";
    public const string Taco = "Taco";
    public const string Cake = "Cake";
    public const string Grill = "Grill";
    public const string Unknown = "???";

    public static string GetSceneName(SceneType type)
    {
        return type switch
        {
            SceneType.Intro   => Intro,
            SceneType.Game    => Game,
            SceneType.Loading => Loading,
            _                 => string.Empty
        };
    }

    public static string GetGameMode(GameMode type)
    {
        return type switch
        {
            GameMode.Single => Single,
            GameMode.Multi  => Multi,
            _               => string.Empty
        };
    }

    public static string GetThemeName(ThemeType type)
    {
        return type switch
        {
            ThemeType.Pizza    => Pizza,
            ThemeType.Salad    => Salad,
            ThemeType.Bibimbap => Bibimbap,
            ThemeType.Bagel    => Bagel,
            ThemeType.Taco     => Taco,
            ThemeType.Cake     => Cake,
            ThemeType.Grill    => Grill,
            _                  => string.Empty
        };
    }
}