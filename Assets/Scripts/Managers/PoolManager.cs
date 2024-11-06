using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    class Pool
    {
        private readonly Stack<GameObject> _poolStack = new();
        GameObject Original { get; set; }
        Transform Root { get; set; }

        public Pool(GameObject original, Transform root)
        {
            Original = original;
            Root = new GameObject($"{original.name}_Root").transform;
            Root.SetParent(root);

            for (int i = 0; i < 5; i++)
            {
                Push(Create());
            }
        }

        GameObject Create()
        {
            var go = Instantiate(Original);
            go.name = Original.name;
            return go;
        }

        public void Push(GameObject poolable)
        {
            poolable.transform.SetParent(Root);
            poolable.SetActive(false);
            _poolStack.Push(poolable);
        }

        public GameObject Pop()
        {
            var poolable = (_poolStack.Count > 0) ? _poolStack.Pop() : Create();
            poolable.SetActive(true);
            return poolable;
        }
    }

    private readonly Dictionary<string, Pool> _pool = new();
    private GameObject _root;
    public Transform Root
    {
        get
        {
            if (_root == null)
            {
                _root = GameObject.Find("[Pool]");
                if (_root == null)
                {
                    _root = new("[Pool]");
                }
            }
            return _root.transform;
        }
    }


    public T Pop<T>(GameObject original) where T : MonoBehaviour
    {
        if (!_pool.ContainsKey(original.name)) CreatePool(original);
        return _pool[original.name].Pop().GetComponent<T>();
    }

    public T Pop<T>(string key) where T : MonoBehaviour
    {
        _pool.TryGetValue(key, out Pool pool);
        if(pool == null) return null;
        return pool.Pop().GetComponent<T>();
    }

    public void CreatePool(GameObject original)
    {
        Pool pool = new(original, Root);
        _pool.Add(original.name, pool);
    }

    public void Push(GameObject poolable)
    {
        if (_pool.TryGetValue(poolable.name, out Pool pool))
        {
            pool.Push(poolable);
        }
        else
        {
            Destroy(poolable);
        }
    }

    public void Clear()
    {
        if (_root == null) return;
        foreach (Transform child in Root)
        {
            Destroy(child.gameObject);
        }
        _pool.Clear();
    }
}