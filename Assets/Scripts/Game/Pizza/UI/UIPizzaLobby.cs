using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class UIPizzaLobby : UIPizzaBase
{
    [SerializeField] private Button btnSearchRoom;
    [SerializeField] private Button btnCreateRoom;
    [SerializeField] private Button btnAdd;
    [SerializeField] private Button btnSubtract;
    [SerializeField] private TMP_InputField InputSearchRoom;
    [SerializeField] private TMP_InputField InputCreateRoom;
    [SerializeField] private TMP_Text txtCount;
    [SerializeField] private TMP_Text txtMaxCount;
    [SerializeField] private GameObject objEmpty;
    [SerializeField] private GameObject objRoom;
    [SerializeField] private RectTransform parent;
    [SerializeField] private RectTransform pool;

    Stack<UIPizzaRoom> roomPool = new();
    List<UIPizzaRoom> roomList = new();
    int max = 2;

    protected override void Init()
    {
        SetMaxCount(2);
    }

    protected override void AddListener()
    {
        btnSearchRoom.onClick.AddListener(SearchRoom);
        btnCreateRoom.onClick.AddListener(CreateRoom);
        btnSubtract.onClick.AddListener(() => { SetMaxCount(max -= 2); ; });
        btnAdd.onClick.AddListener(() => { SetMaxCount(max += 2); ; });
    }

    void SetMaxCount(int val)
    {
        max = val;
        txtMaxCount.text = max.ToString();
        btnSubtract.interactable = (max > 2);
        btnAdd.interactable = (max < 8);
    }

    void CreateRoom()
    {
        if (InputCreateRoom.text == string.Empty)
        {
            UI.OpenUI<UIPopUpButton>().SetMessage($"생성할 방 제목을 입력하세요.", "방 생성 실패");
            return;
        }
        PizzaGameData.Instance.OnLoading();
        string roomName = InputCreateRoom.text;
        InputCreateRoom.text = string.Empty;

        RoomOptions options = new()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = max,
        };
        PizzaNetworkManager.Instance.CreateRoom(roomName, options);
    }

    void SearchRoom()
    {
        UI.OpenUI<UIPopUpButton>().SetMessage($"검색어 [ {InputSearchRoom.text} ]와 일치하는 방이 없습니다.", "방 검색 실패");
        InputSearchRoom.text = string.Empty;
    }

    UIPizzaRoom PopRoom()
    {
        UIPizzaRoom room;
        if (roomPool.Count <= 0)
        {
            room = Instantiate(objRoom).GetComponent<UIPizzaRoom>();

        }
        else
        {
            room = roomPool.Pop();
        }

        room.gameObject.transform.SetParent(parent);
        room.gameObject.SetActive(true);
        roomList.Add(room);
        return room;
    }

    void PushRoom(UIPizzaRoom room)
    {
        roomPool.Push(room);
        roomList.Remove(room);
        room.gameObject.transform.SetParent(pool);
        room.gameObject.SetActive(false);
    }

    void PushAll()
    {
        int count = roomList.Count;
        for (int i = 0; i < count; i++)
        {
            PushRoom(roomList[0]);
        }
    }

    public void SetRoomList(Dictionary<string, RoomInfo> roomList)
    {
        PushAll();
        objEmpty.SetActive(roomList.Count <= 0);
        txtCount.text = roomList.Count.ToString();
        foreach (var room in roomList)
        {
            PopRoom().SetUI(room.Value);
        }
    }
}