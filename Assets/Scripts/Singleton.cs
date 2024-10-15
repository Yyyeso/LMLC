using UnityEngine;
using Unity.VisualScripting;

public class Singleton<T> : MonoBehaviour, ISingleton where T : Component
{
    #region Parent
    private static GameObject parentDonDestroy;
    private static GameObject parent;

    public static Transform ParentDonDestroy
    {
        get
        {
            if (parentDonDestroy == null)
            {
                parentDonDestroy = GameObject.Find("[DonDestroy]");
                if (parentDonDestroy == null)
                {
                    parentDonDestroy = new("[DonDestroy]");
                    DontDestroyOnLoad(parentDonDestroy);
                }
            }
            return parentDonDestroy.transform;
        }
    }
    public static Transform ParentSingleton
    {
        get
        {
            if (parent == null)
            {
                parent = GameObject.Find("[Singleton]");
                if (parent == null)
                {
                    parent = new("[Singleton]");
                }
            }
            return parent.transform;
        }
    }
    #endregion

    static bool _Shutdown;
    static T _Instance;

    public static T Instance
    {
        get
        {
            if (_Shutdown) return default;
            if (_Instance == null)
            {
                _Instance = (T)FindObjectOfType(typeof(T));

                if (_Instance == null)
                {
                    GameObject obj = new($"[{typeof(T)}]");
                    _Instance = obj.AddComponent<T>();

                    if (obj.GetComponent<IDonDestroy>() != null)
                        obj.transform.SetParent(ParentDonDestroy);
                    else
                        obj.transform.SetParent(ParentSingleton);
                }
            }
            return _Instance;
        }
    }


    public void Release()
    {
        Destroy(_Instance.gameObject);
        _Instance = null;
    }

    public virtual void Awake() => Init();

    protected virtual void Init() => print($"Init {typeof(T)}");

    private void OnApplicationQuit() => _Shutdown = true;
}