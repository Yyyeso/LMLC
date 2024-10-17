using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum PizzaTeam
{
    Meat,
    Vege,
    None
}
public class PizzaGameMulti: IPizzaGameManager
{
    PizzaRpcController rpc;
    PizzaGameData Data => PizzaGameData.Instance;
    PizzaTeam MyTeam => (Data.PlayerController.ID <= (PhotonNetwork.CurrentRoom.Players.Count / 2)) ? PizzaTeam.Meat : PizzaTeam.Vege;

    public PizzaGameMulti Setup(PizzaRpcController rpc)
    {
        this.rpc = rpc;
        return this;
    }

    public PizzaRpcController RpcController => rpc;
    public IPizzaGameManager IPizzaGameManager => this;

    public void LoadPlayer()
    {
        UIManager.Instance.CloseUI<UIPizzaStandby>();
        Data.Player = Data.PlayerController.GetComponent<IPizzaPlayerController>();
        if (Data.IsMaster)
        {
            var team = SetTeam();
            RpcController.SetCharacterIndex(team);
        }
    }

    public void SetCharacterIndex(int[] indexList)
    {
        var room = PhotonNetwork.CurrentRoom;
        int id = PhotonNetwork.CurrentRoom.Players.FirstOrDefault(p => p.Value == PhotonNetwork.LocalPlayer).Key;
        Data.CharacterIndex = indexList[id - 1];
        Data.Player.Setup(Data.CharacterIndex);
        PhotonNetwork.LocalPlayer.SetCustomProperties(ReadyLoadGame);
    }
    string LoadGameKey = "LoadGame";
    public ExitGames.Client.Photon.Hashtable ReadyLoadGame => new() { [LoadGameKey] = true };
    int[] SetTeam()
    {
        var room = PhotonNetwork.CurrentRoom;
        int[] meatList = new int[4] { 0, 1, 2, 3 };
        int[] vegeList = new int[6] { 4, 5, 6, 7, 8, 9 };
        int[] characterIndex = new int[room.Players.Count];
        int count = (room.Players.Count / 2);
        int maxCount;
        for (int i = 0; i < room.Players.Count; i++)
        {
            int idx = i;
            int t, selected;
            var myTeam = (idx < (room.Players.Count / 2)) ? PizzaTeam.Meat : PizzaTeam.Vege;
            if (myTeam == PizzaTeam.Meat)
            {
                maxCount = meatList.Length - (idx % count);
                t = Random.Range(0, maxCount);
                selected = meatList[t];
                meatList[t] = meatList[maxCount - 1];
            }
            else
            {
                maxCount = vegeList.Length - (idx % count);
                t = Random.Range(0, maxCount);
                selected = vegeList[t];
                vegeList[t] = vegeList[maxCount - 1];
            }
            characterIndex[idx] = selected;
        }
        return characterIndex;
    }

    public UIPizzaGameScene UIGame => UIManager.Instance.OpenUI<UIPizzaGameMulti>();

    public void ExitGame()
    {
        RpcController.ExitGame();
    }
    public void SetGame()
    {
        Data.MeatScore = Data.VegeScore = (PhotonNetwork.CurrentRoom.Players.Count / 2);
        UIManager.Instance.OpenUI<UIPizzaGameMulti>().Setup(MyTeam == PizzaTeam.Meat);
        UIManager.Instance.OpenUI<UIPizzaGameMulti>().SetTeamScore(Data.MeatScore, Data.VegeScore);
    }

    public async UniTask NoticeTeam()
    {
        string notice = (MyTeam == PizzaTeam.Meat) ? "당신은 고기팀입니다." : "당신은 야채팀입니다.";
        Color color = (MyTeam == PizzaTeam.Meat) ? Color.red : Color.green;
        await UIManager.Instance.OpenUI<UIPopUpNotice>().SetMessage(notice, color, 1.5f);
    }
    public void OnFailed()
    {
        Data.RpcController.SetScore((int)MyTeam);
    }

    public void SetScore(int team)
    {
        if ((PizzaTeam)team == PizzaTeam.Meat) Data.MeatScore--;
        else Data.VegeScore--;
        UIManager.Instance.OpenUI<UIPizzaGameMulti>().SetTeamScore(Data.MeatScore, Data.VegeScore);
    }
    public void SetResult()
    {
        if (Data.MeatScore == Data.VegeScore)
        {
            PizzaGameManager.Instance.SetResult();
        }
        else
        {
            PizzaTeam winner = (Data.MeatScore < Data.VegeScore) ? PizzaTeam.Vege : PizzaTeam.Meat;
            PizzaGameManager.Instance.SetResult(winner == MyTeam);
        }
    }

    public void SetGameData()
    {
        Data.PlayerController.ResetPos();
    }

    public async void OnReady()
    {
        await NoticeTeam();
        await UniTask.Delay(1200);
        PhotonNetwork.LocalPlayer.SetCustomProperties(ReadyStartGame);
    }
    string StartGameKey = "StartGameKey";
    public ExitGames.Client.Photon.Hashtable ReadyStartGame => new() { [StartGameKey] = true };
    public void StartGame()
    {
        PizzaGameManager.Instance.NextPattern();
    }
}