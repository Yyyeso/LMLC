using UnityEngine;
using UnityEngine.UI;
using ScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode;
using ScreenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode;

public class UIResolution : MonoBehaviour, IResizeUI
{
    void Start() => SetResolution(ResolutionManager.Instance);

    private void OnEnable() => ResolutionManager.Instance.Add(this);

    private void OnDisable()
    {
        GetComponent<RectTransform>().eulerAngles = Vector3.zero;
        if (ResolutionManager.Instance != null)
            ResolutionManager.Instance.Delete(this);
    }

    public void Resize(ResolutionManager screen) => SetResolution(screen);

    public void SetResolution(ResolutionManager screen)
    {
        CanvasScaler scaler = transform.parent.GetComponent<CanvasScaler>();
        scaler.uiScaleMode         = ScaleMode.ScaleWithScreenSize;
        scaler.screenMatchMode     = ScreenMatchMode.Expand;
        scaler.referenceResolution = screen.Ref;

        RectTransform rect = GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new(0.5f, 0.5f);
        rect.sizeDelta = new(1920, 1080);
        rect.eulerAngles = (screen.ResolutionType == ResolutionType.PC)? Vector3.zero : Vector3.back * 90;
    }
}