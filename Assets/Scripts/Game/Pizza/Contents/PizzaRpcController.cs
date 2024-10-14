using Photon.Pun;
using UnityEngine;

public class PizzaRpcController : MonoBehaviour
{
    [SerializeField] private PhotonView pv;


    public void SetScore(int team)
    {
        pv.RPC(nameof(SetScoreRPC), RpcTarget.All, team);
    }
    [PunRPC] public void SetScoreRPC(int team)
    {
        PizzaGameData.Instance.PizzaGameMulti.SetScore(team);
        //NetworkManager.Instance.ExitGame();
    }
    public void SetCharacterIndex(int[] indexList)
    {
        pv.RPC(nameof(SetCharacterIndexRPC), RpcTarget.All, indexList);
    }
    [PunRPC]
    public void SetCharacterIndexRPC(int[] indexList)
    {
        PizzaGameData.Instance.PizzaGameMulti.SetCharacterIndex(indexList);
        //NetworkManager.Instance.ExitGame();
    }

    public void LoadGame()
    {
        pv.RPC(nameof(LoadGameRPC), RpcTarget.All);
    }
    [PunRPC]
    public void LoadGameRPC()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(NotReadyLoadGame);
        PizzaGameManager.Instance.LoadGame();
    }
    public void StartGame()
    {
        pv.RPC(nameof(StartGameRPC), RpcTarget.All);
    }
    [PunRPC]
    public void StartGameRPC()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(NotReadyStartGameRPC);
        PizzaGameManager.Instance.StartGame();
    }

    public void ExitGame()
    {
        //pv.RPC(nameof(ExitGameRPC), RpcTarget.All);
    }
    //[PunRPC]
    public void ExitGameRPC()
    {
        //NetworkManager.Instance.ExitGame();
    }
    public void TestTimer(int val)
    {
        //pv.RPC(nameof(TestTimerRPC), RpcTarget.All, val);
    }
    //[PunRPC] public void TestTimerRPC(int val) => Time.timeScale += (5 * val);

    #region Game Control
    public void SetGame() => pv.RPC(nameof(SetGameRPC), RpcTarget.All);
    [PunRPC]
    void SetGameRPC()
    {
        var data = PizzaGameData.Instance;
        data.OnLoading();
        data.PizzaGameMulti = new PizzaGameMulti().Setup(data.RpcController);
        var multi = data.PizzaGameMulti.IPizzaGameManager;
        PizzaGameManager.Instance.SetGame(multi);
    }

    //public void StartGame()
    //{
    //    //pv.RPC(nameof(StartGameRPC), RpcTarget.All); 
    //}
    ////[PunRPC] 
    //public void StartGameRPC() => PizzaGameManager.Instance.StartGameRPC();
    #endregion

    #region Test
    public void PlayAttack1() => pv.RPC(nameof(PlayAttack1RPC), RpcTarget.All);
    [PunRPC] void PlayAttack1RPC() 
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(NotReadyAttack1);
        _ = PizzaGameManager.Instance.PlayAttack1();
    }
    public void PlayAttack2() => pv.RPC(nameof(PlayAttack2RPC), RpcTarget.All);
    [PunRPC] void PlayAttack2RPC()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(NotReadyAttack2);
        _ = PizzaGameManager.Instance.PlayAttack2();
    }
    public void PlayAttack3First() => pv.RPC(nameof(PlayAttack3FirstRPC), RpcTarget.All);
    [PunRPC] void PlayAttack3FirstRPC()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(NotReadyAttack3First);
        _ = PizzaGameManager.Instance.PlayAttack3First();
    }
    public void PlayAttack3Second() => pv.RPC(nameof(PlayAttack3SecondRPC), RpcTarget.All);
    [PunRPC] void PlayAttack3SecondRPC()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(NotReadyAttack3Second);
        _ = PizzaGameManager.Instance.PlayAttack3Second();
    }

    public void PlayCasting() => pv.RPC(nameof(PlayCastingRPC), RpcTarget.All);
    [PunRPC] void PlayCastingRPC()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(NotReadyCasting);
        _ = PizzaGameManager.Instance.PlayCasting();
    }
    public void PlayCasting1(byte rand) => pv.RPC(nameof(PlayCasting1RPC), RpcTarget.All, rand);
    [PunRPC] void PlayCasting1RPC(byte rand)
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(NotReadyCasting1);
        _ = PizzaGameManager.Instance.PlayCasting1(rand);
    }
    public void PlayCasting2(byte rand1, byte rand2) => pv.RPC(nameof(PlayCasting2RPC), RpcTarget.All, rand1, rand2);
    [PunRPC] void PlayCasting2RPC(byte rand1, byte rand2)
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(NotReadyCasting2);
        _ = PizzaGameManager.Instance.PlayCasting2(rand1, rand2);
    }
    public void PlayCasting3(byte rand1, byte rand2) => pv.RPC(nameof(PlayCasting3RPC), RpcTarget.All, rand1, rand2);
    [PunRPC] void PlayCasting3RPC(byte rand1, byte rand2)
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(NotReadyCasting3);
        _ = PizzaGameManager.Instance.PlayCasting3(rand1, rand2);
    }
    public void SetScatterValue(int[] randomForce, int[] randomDegree) => pv.RPC(nameof(SetScatterValueRPC), RpcTarget.All, randomForce, randomDegree);
    [PunRPC]
    void SetScatterValueRPC(int[] randomForce, int[] randomDegree)
    {
        byte[] f = new byte[randomForce.Length];
        ushort[] d = new ushort[randomForce.Length];

       
        for (int i = 0; i < randomForce.Length; i++)
        {
            f[i] = (byte)randomForce[i];
            d[i] = (ushort)randomDegree[i];
        }

        PizzaGameData.Instance.RandomForce = f;
        PizzaGameData.Instance.RandomDegree = d;
        PhotonNetwork.LocalPlayer.SetCustomProperties(SetPepperoniPos);
        PhotonNetwork.LocalPlayer.SetCustomProperties(SetOlivePos);

        PizzaGameManager.Instance.SetReadyAttack();
    }
    #endregion

    #region Ready Key
    string StartGameKey = "StartGameKey";
    string LoadGameKey = "LoadGame";
    string SetPepperoni = "SetPepperoni";
    string SetOlive = "SetOlive";

    string ReadyPlayCasting = "ReadyPlayCasting";
    string ReadyPlayCasting1 = "ReadyPlayCasting1";
    string ReadyPlayCasting2 = "ReadyPlayCasting2";
    string ReadyPlayCasting3 = "ReadyPlayCasting3";

    string ReadyPlayAttack1 = "ReadyPlayAttack1";
    string ReadyPlayAttack2 = "ReadyPlayAttack2";
    string ReadyPlayAttack3First = "ReadyPlayAttack3First";
    string ReadyPlayAttack3Second = "ReadyPlayAttack3Second";
    public ExitGames.Client.Photon.Hashtable NotReadyStartGameRPC => new() { [StartGameKey] = false };

    public ExitGames.Client.Photon.Hashtable NotReadyLoadGame => new() { [LoadGameKey] = false };
    public ExitGames.Client.Photon.Hashtable SetPepperoniPos => new() { [SetPepperoni] = false };
    public ExitGames.Client.Photon.Hashtable SetOlivePos => new() { [SetOlive] = false };

    public ExitGames.Client.Photon.Hashtable NotReadyCasting => new() { [ReadyPlayCasting] = false };
    public ExitGames.Client.Photon.Hashtable NotReadyCasting1 => new() { [ReadyPlayCasting1] = false };
    public ExitGames.Client.Photon.Hashtable NotReadyCasting2 => new() { [ReadyPlayCasting2] = false };
    public ExitGames.Client.Photon.Hashtable NotReadyCasting3 => new() { [ReadyPlayCasting3] = false };

    public ExitGames.Client.Photon.Hashtable NotReadyAttack1 => new() { [ReadyPlayAttack1] = false };
    public ExitGames.Client.Photon.Hashtable NotReadyAttack2 => new() { [ReadyPlayAttack2] = false };
    public ExitGames.Client.Photon.Hashtable NotReadyAttack3First => new() { [ReadyPlayAttack3First] = false };
    public ExitGames.Client.Photon.Hashtable NotReadyAttack3Second => new() { [ReadyPlayAttack3Second] = false };
    #endregion
}