using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISelectMode : UIBase
{
    [SerializeField] Button btnSet;
    [SerializeField] Button btnClose;
    [SerializeField] Button btnChar;
    [SerializeField] Button btnItem;

    [SerializeField] GameObject objNewChar;
    [SerializeField] GameObject objNewItem;

    [SerializeField] Button btnSolo;
    [SerializeField] Button btnTeam;

    [SerializeField] GameObject objLockSolo;
    [SerializeField] GameObject objLockTeam;

    protected override void Init()
    {
        SetLock();
    }

    protected override void AddListener()
    {
        btnSet.onClick.AddListener(OpenSet);
        btnClose.onClick.AddListener(CloseAction);
        btnChar.onClick.AddListener(OpenChar);
        btnItem.onClick.AddListener(OpenItem);

        btnSolo.onClick.AddListener(() => SelectMode(MultiMode.Solo));
        btnTeam.onClick.AddListener(() => SelectMode(MultiMode.Team));
    }

    async void CloseAction()
    {
        await NetworkManager.Instance.Disconnect();
        CloseUI();
    }

    void OpenSet()
    {
        OpenUI<UIPopUpButton>().SetMessage("설정");
    }

    void OpenChar()
    {
        OpenUI<UIPopUpButton>().SetMessage("캐릭터 관리");
    }

    void OpenItem()
    {
        OpenUI<UIPopUpButton>().SetMessage("아이템 관리");
    }

    void SelectMode(MultiMode type)
    {
        NetworkManager.Instance.JoinRoom(type);
    }

    void SetLock()
    {
        bool openSolo = true;
        bool openTeam = false;

        btnSolo.interactable = openSolo;
        objLockSolo.SetActive(!openSolo);

        btnTeam.interactable = openTeam;
        objLockTeam.SetActive(!openTeam);
    }
}