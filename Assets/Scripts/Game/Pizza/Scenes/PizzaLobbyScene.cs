using UnityEngine;

public class PizzaLobbyScene : MonoBehaviour
{
    [SerializeField] private bool isMobile;

    UIManager ui;
    PizzaGameData data;


    void Start()
    {
        ui = UIManager.Instance;
        data = PizzaGameData.Instance;
        if (isMobile) data.IsMobile = true;
        SetMainMenu();
    }

    void SetMainMenu()
    {
        data.OnLoading();
        data.PlayBGM(PizzaBGMType.Lobby);
        data.OnCompleteLoading();
    }
}