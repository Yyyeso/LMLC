using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIManager : Singleton<UIManager>
{
    #region Parent
    private static GameObject parentUI = null;
    public static Transform ParentUI
    {
        get
        {
            if (parentUI == null)
            {
                parentUI = GameObject.Find("[UIContents]");
                if (parentUI == null)
                {
                    parentUI = new GameObject("[UIContents]");
                }
            }
            return parentUI.transform;
        }
    }
    #endregion

    private readonly Dictionary<string, UIBase> _uiList = new();


    private T CreateUI<T>() where T : UIBase
    {
        var objName = typeof(T).Name;
        var go = Instantiate(Resources.Load<GameObject>($"UI/{objName}"), ParentUI);

        if (!go.TryGetComponent<UIBase>(out var ui))
        {
            Destroy(go);
            Debug.LogWarning($"{objName} Has No UIBase Component");
        }
        else _uiList.TryAdd(objName, ui);

        return (T)ui;
    }

    public T OpenUI<T>() where T : UIBase
    {
        SetEventSystem();
        if (!_uiList.TryGetValue(typeof(T).Name, out UIBase ui))
        {
            ui = CreateUI<T>();
        }
        ui.gameObject.SetActive(true);
        return (T)ui;
    }

    public void CloseUI<T>() where T : UIBase
    {
        if (_uiList.TryGetValue(typeof(T).Name, out UIBase ui))
        {
            ui.gameObject.SetActive(false);
        }
    }

    public T GetUI<T>() where T : UIBase
    {
        var uiName = typeof(T).Name;
        if (!_uiList.TryGetValue(uiName, out UIBase ui)) return null;
        return (T)ui;
    }

    public void ClearList()
    {
        foreach (var pair in _uiList)
        {
            Destroy(pair.Value.gameObject);
        }
        _uiList.Clear();
    }

    public void SetEventSystem()
    {
        if (GameObject.Find(nameof(EventSystem)) != null) { return; }

        GameObject eventSystem = new(nameof(EventSystem));
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();
    }
}