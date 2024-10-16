using System;
using Photon.Pun;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Object = UnityEngine.Object;
using UnityEngine.AddressableAssets;

public class ResourceLoadManager : Singleton<ResourceLoadManager>
{
    public async UniTask DownloadDependenciesAsync(string strLabel)
    {
        if (await Addressables.GetDownloadSizeAsync(strLabel) != 0) return;
        await Addressables.DownloadDependenciesAsync(strLabel,true);
    }

    public async UniTask<T> LoadAssetasync<T>(AssetReference refer)
    {
        return await Addressables.LoadAssetAsync<T>(refer);
    }

    public async UniTask<T> LoadAssetasync<T>(string key)
    {
        return await Addressables.LoadAssetAsync<T>(key);
    }

    public async UniTask<T> Instantiate<T>(string key, Vector3 position = default, Transform parent = null) where T : Object
    {
        try
        {
            var go = await Addressables.InstantiateAsync(key, parent);
            go.transform.position = position;
            return go.GetComponent<T>();
        }
        catch (Exception)
        {
            return default;
            throw new Exception($"{key} 인스턴스화에 실패했습니다.");
        }  
    }

    // photon Instantiate 사용을 위해 어드레스 에셋을 포톤 풀에 저장
    public async void SetPhotonPool(string strAddress)
    {
        var player = await LoadAssetasync<GameObject>(strAddress);
        DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
        pool!.ResourceCache.Add(strAddress, player);
    }

    public void Release<Tobject>(Tobject obj)
    {
        Addressables.Release(obj);
    }
}