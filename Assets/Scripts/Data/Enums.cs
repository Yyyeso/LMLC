public enum GameMode
{
    Single,
    Multi,
}

public enum SceneType
{
    Intro,
    Game,
    Loading,
}

public static class Const
{
    public const string Intro = "Intro";

    public const string Game = "Game";

    public const string Loading = "Loading";

    public const string Single = "Single";

    public const string Multi = "Multi";

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
}