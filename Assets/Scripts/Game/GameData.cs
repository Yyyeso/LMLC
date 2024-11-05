using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameData : Singleton<GameData>
{
    #region Player
    public string NickName { get; set; } = null;
    public RpcController RpcController { get; set; }
    public MyPlayer Player { get; set; }
    public bool IsMaster => PhotonNetwork.LocalPlayer.IsMasterClient;
    public Hashtable IsReady => new() { [Const.Ready] = true };
    public Hashtable IsNotReady => new() { [Const.Ready] = false };
    #endregion

    #region Room
    public Dictionary<string, RoomInfo> RoomList { get; set; } = new();
    public int MaxPlayers { get; set; } = 8;
    public bool OnLobby { get; set; }
    #endregion

    #region Project Settings
    public bool IsMulti { get; set; } = false;
    public bool IsPC { get; set; } = false;
    #endregion

    #region Sound
    PizzaSoundList sound;

    public PizzaSoundList SoundList => sound = (sound != null) ? sound : PizzaResources.Instance.SoundList;

    public void PlayBGM(PizzaBGMType type, float customVolume = 0.3f)
    {
        SoundManager.Instance.PlayBGM(SoundList.GetBGM((int)type), customVolume);
    }
    public async UniTask PlaySFX(PizzaSFXType type, float customVolume = 1f, bool useAwait = false)
    {
        await SoundManager.Instance.PlaySFX(SoundList.GetAudioClip((int)type), customVolume, useAwait);
    }
    public async UniTask DelayPlaySFX(PizzaSFXType type, float delay, float customVolume = 1f, bool useAwait = false)
    {
        await UniTask.Delay((int)(delay * 1000));
        await PlaySFX(type, customVolume, useAwait);
    }
    #endregion

    #region Game
    public Transform Stage { get; set; }
    public bool GameOver { get; set; } = false;
    public bool IsOutside { get; set; }
    public int CharacterIndex { get; set; }
    #endregion


    #region Utility
    public Color GetColor(string hexCode)
    {
        ColorUtility.TryParseHtmlString(hexCode, out Color color);
        return color;
    }

    public Vector3 GetAnglePos(float force, float degree)
    {
        float radian = degree * Mathf.Deg2Rad;

        return new Vector3(Mathf.Cos(radian) * force, Mathf.Sin(radian) * force, 0);
    }

    public float GetDegree(Vector2 start, Vector2 end)
    {
        Vector2 gap = end - start;
        float radian = Mathf.Atan2(gap.y, gap.x);

        return radian * Mathf.Rad2Deg;
    }
    #endregion
}

#region Enums
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
#endregion

public static class Const
{
    public const string Ready = "Ready";
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

    public const string Player = "Player";
    public const string Dash_Start = "Dash";
    public const string Dash_End = "Dash2";
    public const string Run = "Run";
    public const string DamageText = "DamageText";

    public static string GetSceneName(SceneType type)
    {
        return type switch
        {
            SceneType.Intro => Intro,
            SceneType.Game => Game,
            SceneType.Loading => Loading,
            _ => string.Empty
        };
    }

    public static string GetGameMode(GameMode type)
    {
        return type switch
        {
            GameMode.Single => Single,
            GameMode.Multi => Multi,
            _ => string.Empty
        };
    }

    public static string GetThemeName(ThemeType type)
    {
        return type switch
        {
            ThemeType.Pizza => Pizza,
            ThemeType.Salad => Salad,
            ThemeType.Bibimbap => Bibimbap,
            ThemeType.Bagel => Bagel,
            ThemeType.Taco => Taco,
            ThemeType.Cake => Cake,
            ThemeType.Grill => Grill,
            _ => string.Empty
        };
    }
}