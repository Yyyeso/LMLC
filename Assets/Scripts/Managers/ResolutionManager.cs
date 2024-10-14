using UnityEngine;
using System.Collections.Generic;

public enum ResolutionType
{
    PC,
    MOBILE,
}

public class ResolutionManager : Singleton<ResolutionManager>, IDonDestroy, ISubject<IResizeUI>
{
    bool _enabled = true;
    public bool Enabled
    {
        get { return _enabled; }
        set
        { 
            _enabled = value;
            if (!value)
            {
                Rect rect = Camera.main.rect;
                rect.width = 1;
                rect.height = 1;
                rect.x = 0;
                rect.y = 0;
                Camera.main.rect = rect;
            }
        }
    }
    public bool Swapable { get; set; } = false;
    public ResolutionType ResolutionType { get; private set; } = ResolutionType.PC;
    public Vector2 Ref => (ResolutionType == ResolutionType.PC) ? new(1920, 1080) : new(1080, 1920);


    #region Check Screen Size
    public Vector2 ScreenSize => new(Screen.width, Screen.height);
    private Vector2 CurSize = Vector2.one;

    void LateUpdate()
    {
        if (!Enabled) return;
        if (CurSize == ScreenSize) return;
        CurSize = ScreenSize;
        if (Swapable) Swap();
        ResizeCam(Camera.main);
    }
    #endregion

    #region Set Resolution
    private const float swapValue = 1;
    private float ScreenRatio => ((float)Screen.width / Screen.height);
    private float TargetRatio => (ResolutionType == ResolutionType.PC) ? ((float)16 / 9) : ((float)9 / 16);

    void Swap()
    {
        var curType = (ScreenRatio < swapValue) ? ResolutionType.MOBILE : ResolutionType.PC;
        if(ResolutionType != curType) ResolutionType = curType;

        foreach (var ui in uiList)
        { 
            ui.Resize(this);
        }
    }

    void ResizeCam(Camera cam)
    {
        Rect rect = cam.rect;
        float scaleheight = ScreenRatio / TargetRatio;

        if (scaleheight < 1) // 세로로 긴 형태
        {
            rect.height = scaleheight;
            rect.y = (1 - scaleheight) * 0.5f;
            rect.width = 1;
            rect.x = 0;
        }
        else                 // 가로로 긴 형태
        {
            float scalewidth = 1 / scaleheight;
            rect.width = scalewidth;
            rect.x = (1 - scalewidth) * 0.5f;
            rect.height = 1;
            rect.y = 0;
        }

        cam.rect = rect;
        GL.Clear(true, true, Color.black);
    }
    #endregion

    #region Register
    private List<IResizeUI> uiList = new();

    public void Add(IResizeUI ui)
    {
        if (uiList.Contains(ui)) return;
        uiList.Add(ui);
        ui.Resize(this);
    }

    public void Delete(IResizeUI ui)
    {
        if (uiList.Contains(ui)) uiList.Remove(ui);
    }
    #endregion
}

public interface IResizeUI
{
    void Resize(ResolutionManager screen);
}