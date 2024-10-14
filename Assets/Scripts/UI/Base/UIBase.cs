using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

public abstract class UIBase : MonoBehaviour
{
    UIManager ui;
    protected UIManager UI => ui = (ui != null)? ui : UIManager.Instance;


    #region Setup
    private bool _isLoaded;
    public event Action<UIBase> OnCloseAction;

    protected virtual UniTask LoadAssets() => default;

    protected virtual void Init() { }

    protected virtual void AddListener() { }

    private async void Awake()
    {
        await LoadAssets();
        _isLoaded = true;
    }

    private async void Start()
    {
        await UniTask.WaitUntil(() => _isLoaded);
        Init();
        AddListener();
    }

    protected virtual void OnDisable() => OnCloseAction?.Invoke(this);
    #endregion

    public T OpenUI<T>() where T : UIBase => UI.OpenUI<T>();

    public void CloseUI() => gameObject.SetActive(false);
}