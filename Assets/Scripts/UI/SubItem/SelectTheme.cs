using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectTheme : MonoBehaviour
{
    [SerializeField] Image[] imgBG;
    [SerializeField] Image[] imgStars;

    [SerializeField] Button btnTheme;
    [SerializeField] Image imgIcon;
    [SerializeField] TMP_Text txtTitle;
    [SerializeField] TMP_Text txtProgress;
    [SerializeField] Slider sliderProgress;

    [SerializeField] GameObject objLock;

    [SerializeField] Color colorStarEmpty;
    [SerializeField] Color colorStarFilled;
    [SerializeField] Sprite sprTest;

    ThemeType type;

    float[] h = new float[] { 0.78f, 0.02f, 0.11f, 0.22f, 0.44f, 0.59f, 0.74f };
    void SetColor(int idx)
    {
        float h = this.h[idx];
        imgBG[0].color = Color.HSVToRGB(h, 0.05f, 1);
        imgBG[1].color = Color.HSVToRGB(h, 0.5f,  1);
        imgBG[2].color = Color.HSVToRGB(h, 0.25f, 1);
    }

    void Awake() => btnTheme.onClick.AddListener(StartGame);
   
    public void SetTheme(int idx)
    {
        type = (ThemeType)idx;
        SetProgress(0);
        SetStar(0);
        Lock(idx != 0);
        SetColor(idx);
        gameObject.SetActive(true);
    }

    void SetProgress(float value)
    {
        if      (value > 3) value = 3;
        else if (value < 0) value = 0;

        sliderProgress.value = value;
        txtProgress.text = $"{(int)(value * 100)}%";
    }

    void SetStar(int idx)
    {
        if (idx > 3) idx = 3;
        idx -= 1;
        for (int i = 0; i < imgStars.Length; i++)
        {
            imgStars[i].color = (i < idx) ? colorStarFilled : colorStarEmpty;
        }
    }

    void Lock(bool isLocked)
    {
        objLock.SetActive(isLocked);
        btnTheme.interactable = !isLocked;

        txtTitle.text  = (isLocked) ? Const.Unknown : Const.GetThemeName(type);
        imgIcon.sprite = (isLocked) ? null : sprTest;
    }

    private void StartGame()
    {
        UIManager.Instance.OpenUI<UIPopUpButton>().SetMessage($"{type} 선택");
    }
}