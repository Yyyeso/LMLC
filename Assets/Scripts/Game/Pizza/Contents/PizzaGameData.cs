using System;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PizzaGameData : Singleton<PizzaGameData>
{
    #region Resolution
    public Transform GameParent => Resource.GameParent;
    #endregion
    public PizzaGameMulti PizzaGameMulti { get; set; }
    #region Player
    public string NickName { get; set; } = null;
    public PizzaRpcController RpcController { get; set; }
    public PizzaPlayerController PlayerController { get; set; }
    public IPizzaPlayerController Player { get; set; }
    public bool IsMaster => PhotonNetwork.LocalPlayer.IsMasterClient;
    public string ReadyKey => "Ready";
    public Hashtable Ready => new() { [ReadyKey] = true };
    public Hashtable NotReady => new() { [ReadyKey] = false };
    #endregion

    #region Room
    public Dictionary<string, RoomInfo> RoomList { get; set; } = new();
    public int MaxPlayers { get; set; } = 8;
    public bool OnLobby { get; set; }
    #endregion

    #region Project Settings
    public bool IsMulti { get; set; } = false;
    public bool Is3D { get; set; } = false;
    public bool IsVR { get; set; } = false;
    public bool IsMobile { get; set; } = false;
    #endregion

    #region Resources
    PizzaSpriteList sprite;
    PizzaSoundList sound;
    PizzaAttackList attack;
    //PizzaVRList vrList;

    PizzaResources Resource => PizzaResources.Instance;
    public PizzaSpriteList SpriteList => sprite = (sprite != null) ? sprite : Resource.SpriteList;
    public PizzaSoundList SoundList => sound = (sound != null) ? sound : Resource.SoundList;
    public PizzaAttackList AttackList => attack = (attack != null) ? attack : Resource.AttackList;
    //public PizzaVRList VRList => vrList = (vrList != null) ? vrList : Resource.VRList;

    SoundManager Sound => SoundManager.Instance;
    public void PlayBGM(PizzaBGMType type, float customVolume = 0.3f)
    {
        Sound.PlayBGM(SoundList.GetBGM((int)type), customVolume);
    }
    public async UniTask PlaySFX(PizzaSFXType type, float customVolume = 1f, bool useAwait = false)
    {
        await Sound.PlaySFX(SoundList.GetAudioClip((int)type), customVolume, useAwait);
    }
    public async UniTask DelayPlaySFX(PizzaSFXType type, float delay, float customVolume = 1f, bool useAwait = false)
    {
        await UniTask.Delay((int)(delay * 1000));
        await PlaySFX(type, customVolume, useAwait);
    }
    #endregion

    #region Game
    public byte[] RandomForce { get; set; }
    public ushort[] RandomDegree { get; set; }
    public PizzaAttackArea AttackArea { get; set; }
    public Transform Stage { get; set; }
    public bool GameOver { get; set; } = false;
    public bool IsOutside { get; set; }
    public int CharacterIndex { get; set; }
    public int MeatScore { get; set; }
    public int VegeScore { get; set; }
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

    #region Loading
    UIPizzaGameLoading uiLoading;

    public void OnLoading()
    {
        if (uiLoading == null)
        {
            uiLoading = Resource.UILoading;
        }
        uiLoading.gameObject.SetActive(true);
        uiLoading.Setup();
    }

    public void OnCompleteLoading()
    {
        if (uiLoading == null) return;

        uiLoading.Stop();
        uiLoading.gameObject.SetActive(false);
    }
    #endregion
}

#region Enum
public enum AddCorn
{
    None,
    Head,
    Tail,
    Both
}

public enum PizzaSFXType
{
    Button,
    Combo,
    Paste,
    Ingredient,
    Cheese,
    Corn,
    Waterdrop,
    Casher
}

public enum PizzaBGMType
{
    Lobby,
    Game,
    Clear,
    Fail,
    None
}
#endregion