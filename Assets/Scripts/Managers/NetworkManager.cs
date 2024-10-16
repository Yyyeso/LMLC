using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using Random = UnityEngine.Random;
using ExitGames.Client.Photon.StructWrapping;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Cysharp.Threading.Tasks;

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

    UIManager ui;
    SceneLoadManager scene;
    const int maxCount = 8;

    #region Test Data
    MultiMode MultiMode { get; set; } = MultiMode.Solo;

    public string NickName { get; set; } = null;
    public bool IsMaster => PhotonNetwork.LocalPlayer.IsMasterClient;
    public string ReadyKey => "Ready";
    public Hashtable Ready => new() { [ReadyKey] = true };
    public Hashtable NotReady => new() { [ReadyKey] = false };

    public int MaxPlayers { get; set; } = 8;
    public bool OnLobby { get; set; }
    #endregion


    private void Awake()
    {
        ui = UIManager.Instance;
        scene = SceneLoadManager.Instance;
    }

    #region Connect
    public async void Connect()
    {
        await scene.OnStartLoading();
        NickName = $"{Random.Range(0, int.MaxValue)}_{System.DateTime.UtcNow.ToFileTime()}";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.LocalPlayer.NickName = NickName;
        PhotonNetwork.JoinLobby();
    }

    public override async void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        PhotonNetwork.LocalPlayer.SetCustomProperties(NotReady);
        ui.OpenUI<UISelectMode>();
        await scene.OnCompleteLoading();
    }

    public async UniTask Disconnect()
    {
        await scene.OnStartLoading();
        PhotonNetwork.Disconnect();
    }

    public override async void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        print($"OnDisconnected: {cause}");
        await UniTask.Delay(300);
        await scene.OnCompleteLoading();
    }
    #endregion

    #region Join Room
    public async void JoinRoom(MultiMode mode)
    {
        await scene.OnStartLoading();
        MultiMode = mode;
        RoomOptions options = new()
        {
            IsOpen = true,
            MaxPlayers = maxCount,
        };
        PhotonNetwork.JoinRandomOrCreateRoom(roomOptions: options);
    }

    public override void OnCreatedRoom() => ReadyCount = 0;

    public override async void OnJoinedRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            var room = PhotonNetwork.CurrentRoom;
            SetGameData(room);
            // data.RpcController = data.PlayerController.GetComponent<PizzaRpcController>();
            PhotonNetwork.LocalPlayer.SetCustomProperties(Ready);
            await scene.OnCompleteLoading();
        }
    }

    void SetGameData(Room room)
    {
        OnLobby = false;
        MaxPlayers = room.MaxPlayers;

        ui.OpenUI<UIGameMatching>()
            .Setup(MultiMode, room.MaxPlayers)
            .SetCount(room.PlayerCount);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        var room = PhotonNetwork.CurrentRoom;
        ui.GetUI<UIGameMatching>().SetCount(room.PlayerCount);

        if (IsMaster)
        {
            if (room.PlayerCount == room.MaxPlayers)
            {
                room.IsOpen = false;
            }
        }
    }

    public const string CreateFailed = "방 만들기 실패";
    public const string JoinFailed   = "방 참가 실패";
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        ui.OpenUI<UIPopUpButton>().SetMessage(message: message, title: CreateFailed);
    }
    public override async void OnJoinRoomFailed(short returnCode, string message)
    {
        ui.OpenUI<UIPopUpButton>().SetMessage(message: message, title: JoinFailed);
        await UniTask.Delay(250);
        await scene.OnCompleteLoading();
    }
    #endregion

    #region LeaveRoom
    public async UniTask LeaveRoom()
    {
        await scene.OnStartLoading();
        PhotonNetwork.LeaveRoom();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        var room = PhotonNetwork.CurrentRoom;
        if (room.IsOpen)
        {
            ui.GetUI<UIGameMatching>().SetCount(room.PlayerCount);
        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
    }
    #endregion

    #region Ready
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        if (!IsMaster) return;

        changedProps.TryGetValue(ReadyKey, out bool isReady);
        if (isReady)
        {
            if (CheckReady(++ReadyCount))
            {
                //data.RpcController.SetGame();
            }
        }
        changedProps.TryGetValue(ReadyPlayAttack1, out bool atk1);
        if (atk1)
        {
            if (CheckReady(++ReadyAttack1))
            {
                //data.RpcController.PlayAttack1();
                ReadyAttack1 = 0;
            }
        }
        changedProps.TryGetValue(ReadyPlayAttack2, out bool atk2);
        if (atk2)
        {
            if (CheckReady(++ReadyAttack2))
            {
                //data.RpcController.PlayAttack2();
                ReadyAttack2 = 0;
            }
        }
        changedProps.TryGetValue(ReadyPlayAttack3First, out bool atk3f);
        if (atk3f)
        {
            if (CheckReady(++ReadyAttack3First))
            {
                //data.RpcController.PlayAttack3First();
                ReadyAttack3First = 0;
            }
        }
        changedProps.TryGetValue(ReadyPlayAttack3Second, out bool atk3s);
        if (atk3s)
        {
            if (CheckReady(++ReadyAttack3Second))
            {
                //data.RpcController.PlayAttack3Second();
                ReadyAttack3Second = 0;
            }
        }
        changedProps.TryGetValue(ReadyPlayCasting, out bool cst);
        if (cst)
        {
            if (CheckReady(++ReadyCasting))
            {
                //data.RpcController.PlayCasting();
                ReadyCasting = 0;
            }
        }
        changedProps.TryGetValue(ReadyPlayCasting1, out bool cst1);
        if (cst1)
        {
            if (CheckReady(++ReadyCasting1))
            {
                int rand = Random.Range(0, 2);
                //data.RpcController.PlayCasting1((byte)rand);
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
                //data.RpcController.PlayCasting2((byte)rand1, (byte)rand2);
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
                //data.RpcController.PlayCasting3((byte)rand1, (byte)rand2);
                ReadyCasting3 = 0;
            }
        }

        changedProps.TryGetValue(SetPepperoni, out bool p);
        if (p)
        {
            if (CheckReady(++ReadyPepperoni))
            {
                int count = 8;
                int[] f = new int[count];
                int[] d = new int[count];

                for (int i = 0; i < count; i++)
                {
                    f[i] = Random.Range(0, 211);
                    d[i] = Random.Range(0, 361);
                }
                //data.RpcController.SetScatterValue(f, d);
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
                //data.RpcController.SetScatterValue(f, d);
                ReadyOlive = 0;
            }
        }
        changedProps.TryGetValue(LoadGame, out bool onReady);
        if (onReady)
        {
            if (CheckReady(++ReadyLoadGame))
            {
                //data.RpcController.LoadGame();
                ReadyLoadGame = 0;
            }
        }
        changedProps.TryGetValue(StartGame, out bool onStart);
        if (onStart)
        {
            if (CheckReady(++ReadyStartGame))
            {
                //data.RpcController.StartGame();
                ReadyStartGame = 0;
            }
        }
    }

    bool CheckReady(int readyCount) => (readyCount >= MaxPlayers);
    #endregion

    #region Ready Key
    const string StartGame = "StartGameKey";
    const string LoadGame = "LoadGame";
    const string SetPepperoni = "SetPepperoni";
    const string SetOlive = "SetOlive";
    const string ReadyPlayAttack1 = "ReadyPlayAttack1";
    const string ReadyPlayAttack2 = "ReadyPlayAttack2";
    const string ReadyPlayAttack3First = "ReadyPlayAttack3First";
    const string ReadyPlayAttack3Second = "ReadyPlayAttack3Second";
    const string ReadyPlayCasting = "ReadyPlayCasting";
    const string ReadyPlayCasting1 = "ReadyPlayCasting1";
    const string ReadyPlayCasting2 = "ReadyPlayCasting2";
    const string ReadyPlayCasting3 = "ReadyPlayCasting3";

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
}