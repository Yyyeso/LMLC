using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class UIPizzaRoom : MonoBehaviour
{
    bool isPrivate = false;
    [SerializeField] private GameObject objPrivate;
    [SerializeField] private TMP_Text txtRoomName;
    [SerializeField] private TMP_Text txtPlayerCount;
    [SerializeField] private Button btnEnter;
    RoomInfo room;


    void Awake()
    {
        btnEnter.onClick.AddListener(EnterRoom);
    }

    public void SetUI(RoomInfo room)
    {
        this.room = room;
        objPrivate.SetActive(isPrivate);
        txtRoomName.text = room.Name;
        txtPlayerCount.text = $"({room.PlayerCount}/{room.MaxPlayers})";
    }

    void EnterRoom()
    {
        NetworkManager.Instance.JoinRoom(room.Name);
    }
}
