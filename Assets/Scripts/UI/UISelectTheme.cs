using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISelectTheme : UIBase
{
    [SerializeField] Button btnSet;
    [SerializeField] Button btnClose;
    [SerializeField] Button btnChar;
    [SerializeField] Button btnItem;

    [SerializeField] GameObject objNewChar;
    [SerializeField] GameObject objNewItem;

    [SerializeField] RectTransform parent;
    [SerializeField] GameObject template;


    protected override void Init()
    {
        SetTheme();
    }

    protected override void AddListener()
    {
        btnSet.onClick.AddListener(OpenSet);
        btnClose.onClick.AddListener(CloseUI);
        btnChar.onClick.AddListener(OpenChar);
        btnItem.onClick.AddListener(OpenItem);
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

    void SetTheme()
    {
        for (int i = 0; i < (int)ThemeType.MaxCount; i++) 
        {
            Instantiate(template, parent).GetComponent<SelectTheme>().SetTheme(i);
        }
    }
}