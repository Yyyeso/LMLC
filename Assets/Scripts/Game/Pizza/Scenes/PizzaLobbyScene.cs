using UnityEngine;

public class PizzaLobbyScene : MonoBehaviour
{
    [SerializeField] private bool isVR;
    [SerializeField] private bool isMobile;

    UIManager ui;
    PizzaGameData data;


    void Start()
    {
        //if (data.NickName == null)
        //{
        ui = UIManager.Instance;
        data = PizzaGameData.Instance;
        if (isVR) data.Is3D = data.IsVR = true;
        if (isMobile) data.IsMobile = true;
        SetMainMenu();
        //}
    }

    void SetMainMenu()
    {
        data.OnLoading();
        data.PlayBGM(PizzaBGMType.Lobby);
        ui.OpenUI<UIPizzaGameMenu>().Setup();
        data.OnCompleteLoading();
    }
}