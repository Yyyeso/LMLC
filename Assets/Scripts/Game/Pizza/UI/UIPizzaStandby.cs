using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPizzaStandby : UIPizzaBase
{
    [SerializeField] private TMP_Text txtPlayerCount;
    [SerializeField] private Button btnExit;


    protected override void Init()
    {
        base.Init();
    }

    protected override void AddListener()
    {
        btnExit.onClick.AddListener(ExitStandby);
    }

    public void SetCount(int playerCount, int maxPlayers) => txtPlayerCount.text = $"({playerCount}/{maxPlayers})";

    private void ExitStandby()
    {
        var data = PizzaGameData.Instance;
        data.OnLoading();
        data.OnLobby = true;
        CloseUI();
        NetworkManager.Instance.LeaveRoom();
        data.OnCompleteLoading();
    }
}
