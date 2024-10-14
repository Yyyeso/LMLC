using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon.StructWrapping;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    #region Instance
    private static GameObject parent = null;
    static Transform Parent
    {
        get
        {
            if (parent == null)
            {
                parent = GameObject.Find("[Singleton]");
                if (parent == null)
                {
                    parent = new GameObject("[Singleton]");
                }
            }
            return parent.transform;
        }
    }
    private static NetworkManager _instance = null;
    public static NetworkManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<NetworkManager>();

                if (_instance == null)
                {
                    _instance = new GameObject { name = "[NetworkManager]" }.AddComponent<NetworkManager>();
                    _instance.transform.SetParent(Parent);
                }
            }

            return _instance;
        }
    }

    #endregion

    PizzaGameData data;
    UIManager ui;


    #region Connect
    private void Awake()
    {
        data = PizzaGameData.Instance;
        ui = UIManager.Instance;
    }

    public void Connect(bool is3D)
    {
        data.OnLoading();

        int rand = UnityEngine.Random.Range(0, (int)PizzaIngredient.MaxCount);
        data.NickName = $"{(PizzaIngredient)rand}_{System.DateTime.UtcNow.ToFileTime()}";
        data.Is3D = is3D;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.LocalPlayer.NickName = data.NickName;
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        ui.OpenUI<UIPizzaLobby>();
        PhotonNetwork.LocalPlayer.SetCustomProperties(data.NotReady);
        data.OnLobby = true;
        data.OnCompleteLoading();
    }

    public void Disconnect() => PhotonNetwork.Disconnect();

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause); 
        data.OnCompleteLoading();
    }
    #endregion

    #region Update Lobby
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            if (IsClosed(room))
            {
                if (data.RoomList.ContainsKey(room.Name)) // 삭제
                { data.RoomList.Remove(room.Name); }
            }
            else
            {
                if (data.RoomList.ContainsKey(room.Name)) // 갱신
                { data.RoomList[room.Name] = room; }
                else
                { data.RoomList.Add(room.Name, room); }   // 추가
            }
        }
        UpdateLobby(data.RoomList);
    }

    bool IsClosed(RoomInfo room)
    {
        return (room.RemovedFromList || room.MaxPlayers == 0 || room.PlayerCount == 0 || !room.IsOpen);
    }

    void UpdateLobby(Dictionary<string, RoomInfo> roomList)
    {
        if (!data.OnLobby) return;
        ui.OpenUI<UIPizzaLobby>().SetRoomList(roomList);
    }
    #endregion

    #region Create Room
    public readonly string CreateFailed = "방 만들기 실패";
    public void CreateRoom(string roomName, RoomOptions options) => PhotonNetwork.CreateRoom(roomName, options);

    public override void OnCreatedRoom()
    {
        ReadyCount = 0;
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        ui.OpenUI<UIPopUpButton>().SetMessage(message: message, title: CreateFailed);
        data.OnCompleteLoading();
    }
    #endregion

    #region Join Room
    public void JoinRoom(string roomName)
    {
        data.OnLoading();
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            var room = PhotonNetwork.CurrentRoom;
            SetGameData(room);
            ui.CloseUI<UIPizzaLobby>();
            ui.OpenUI<UIPizzaStandby>().SetCount(room.PlayerCount, room.MaxPlayers);

            data.PlayerController = PizzaResources.Instance.PlayerMulti;
            data.RpcController = data.PlayerController.GetComponent<PizzaRpcController>();
            PhotonNetwork.LocalPlayer.SetCustomProperties(data.Ready);
            data.OnCompleteLoading();
        }
    }

    void SetGameData(Room room)
    {
        data.OnLobby = false;
        data.MaxPlayers = room.MaxPlayers;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        var room = PhotonNetwork.CurrentRoom;
        ui.OpenUI<UIPizzaStandby>().SetCount(room.PlayerCount, room.MaxPlayers);
        if (data.IsMaster)
        {
            if (room.PlayerCount == room.MaxPlayers)
            {
                room.IsOpen = false;
            }
        }
    }

    void ReadyGame()
    {
        //data.RpcController.SetGame();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        if (!data.IsMaster) return;

        changedProps.TryGetValue(data.ReadyKey, out bool isReady);
        if (isReady)
        {
            if (CheckReady(++ReadyCount))
            {
                data.RpcController.SetGame();
            }
        }
        changedProps.TryGetValue(ReadyPlayAttack1, out bool atk1);
        if (atk1)
        {
            if (CheckReady(++ReadyAttack1))
            {
                data.RpcController.PlayAttack1();
                ReadyAttack1 = 0;
            }
        }
        changedProps.TryGetValue(ReadyPlayAttack2, out bool atk2);
        if (atk2)
        {
            if (CheckReady(++ReadyAttack2))
            {
                data.RpcController.PlayAttack2();
                ReadyAttack2 = 0;
            }
        }
        changedProps.TryGetValue(ReadyPlayAttack3First, out bool atk3f);
        if (atk3f)
        {
            if (CheckReady(++ReadyAttack3First))
            {
                data.RpcController.PlayAttack3First();
                ReadyAttack3First = 0;
            }
        }
        changedProps.TryGetValue(ReadyPlayAttack3Second, out bool atk3s);
        if (atk3s)
        {
            if (CheckReady(++ReadyAttack3Second))
            {
                data.RpcController.PlayAttack3Second();
                ReadyAttack3Second = 0;
            }
        }
        changedProps.TryGetValue(ReadyPlayCasting, out bool cst);
        if (cst)
        {
            if (CheckReady(++ReadyCasting))
            {
                data.RpcController.PlayCasting();
                ReadyCasting = 0;
            }
        }
        changedProps.TryGetValue(ReadyPlayCasting1, out bool cst1);
        if (cst1)
        {
            if (CheckReady(++ReadyCasting1))
            {
                int rand = Random.Range(0, 2);
                data.RpcController.PlayCasting1((byte)rand);
                ReadyCasting1 = 0;
            }
        }
        changedProps.TryGetValue(ReadyPlayCasting2, out bool cst2);
        if (cst2)
        {
            if (CheckReady(++ReadyCasting2))
            {
                int rand1 = Random.Range(0, 2);
                int rand2 = Random.Range(0, 2);
                data.RpcController.PlayCasting2((byte)rand1, (byte)rand2);
                ReadyCasting2 = 0;
            }
        }
        changedProps.TryGetValue(ReadyPlayCasting3, out bool cst3);
        if (cst3)
        {
            if (CheckReady(++ReadyCasting3))
            {
                int rand1 = Random.Range(0, 2);
                int rand2 = Random.Range(0, 2);
                data.RpcController.PlayCasting3((byte)rand1, (byte)rand2);
                ReadyCasting3 = 0;
            }
        }

        changedProps.TryGetValue(SetPepperoni, out bool p);
        if (p)
        {
            if (CheckReady(++ReadyPepperoni))
            {
                //int count = 8;
                //byte[] f = new byte[count];
                //ushort[] d = new ushort[count];

                //for (int i = 0; i < count; i++)
                //{
                //    f[i] = (byte)Random.Range(0, 211);
                //    d[i] = (ushort)Random.Range(0, 361);
                //}
                int count = 8;
                int[] f = new int[count];
                int[] d = new int[count];

                for (int i = 0; i < count; i++)
                {
                    f[i] = Random.Range(0, 211);
                    d[i] = Random.Range(0, 361);
                }
                data.RpcController.SetScatterValue(f, d);
                ReadyPepperoni = 0;
            }
        }
        changedProps.TryGetValue(SetOlive, out bool o);
        if (o)
        {
            if (CheckReady(++ReadyOlive))
            {
                int count = 5;
                int[] f = new int[count];
                int[] d = new int[count];

                for (int i = 0; i < count; i++)
                {
                    f[i] = Random.Range(0, 211);
                    d[i] = Random.Range(0, 361);
                }
                data.RpcController.SetScatterValue(f, d);
                ReadyOlive = 0;
            }
        }
        changedProps.TryGetValue(LoadGame, out bool onReady);
        if (onReady)
        {
            if (CheckReady(++ReadyLoadGame))
            {
                data.RpcController.LoadGame();
                ReadyLoadGame = 0;
            }
        }
        changedProps.TryGetValue(StartGame, out bool onStart);
        if (onStart)
        {
            if (CheckReady(++ReadyStartGame))
            {
                data.RpcController.StartGame();
                ReadyStartGame = 0;
            }
        }
    }

    #region Ready Key
    string StartGame = "StartGameKey";
    string LoadGame = "LoadGame";
    string SetPepperoni = "SetPepperoni";
    string SetOlive = "SetOlive";
    string ReadyPlayAttack1 = "ReadyPlayAttack1";
    string ReadyPlayAttack2 = "ReadyPlayAttack2";
    string ReadyPlayAttack3First = "ReadyPlayAttack3First";
    string ReadyPlayAttack3Second = "ReadyPlayAttack3Second";
    string ReadyPlayCasting = "ReadyPlayCasting";
    string ReadyPlayCasting1 = "ReadyPlayCasting1";
    string ReadyPlayCasting2 = "ReadyPlayCasting2";
    string ReadyPlayCasting3 = "ReadyPlayCasting3";

    int ReadyCount { get; set; } = 0;
    int ReadyStartGame { get; set; } = 0;
    int ReadyLoadGame { get; set; } = 0;
    int ReadyPepperoni { get; set; } = 0;
    int ReadyOlive { get; set; } = 0;
    int ReadyCasting { get; set; } = 0;
    int ReadyCasting1 { get; set; } = 0;
    int ReadyCasting2 { get; set; } = 0;
    int ReadyCasting3 { get; set; } = 0;
    int ReadyAttack1 { get; set; } = 0;
    int ReadyAttack2 { get; set; } = 0;
    int ReadyAttack3First { get; set; } = 0;
    int ReadyAttack3Second { get; set; } = 0;
    #endregion

    bool CheckReady(int readyCount) => (readyCount >= data.MaxPlayers);

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        ui.OpenUI<UIPopUpButton>().SetMessage(message: message, title: "방 참가 실패");
        data.OnCompleteLoading();
    }
    #endregion

    #region LeaveRoom
    public void LeaveRoom() => PhotonNetwork.LeaveRoom();

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        var room = PhotonNetwork.CurrentRoom;
        if (room.IsOpen)
        {
            ui.OpenUI<UIPizzaStandby>().SetCount(room.PlayerCount, room.MaxPlayers);
        }
        //else PizzaGameManager.Instance.Bye();
    }

    public void ExitGame()
    {
        data.OnLoading();
        ui.CloseUI<UIPopUpButton>();
        LoadMenu();
        LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
    }

    void LoadMenu()
    {
        ui.OpenUI<UIPizzaGameMenu>();
        data.OnLobby = true;
        data.OnCompleteLoading();
    }
    #endregion
}