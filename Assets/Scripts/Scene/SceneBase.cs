using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class SceneBase : MonoBehaviour
{
    protected abstract UniTask Load();

    async void Start()
    {
        await Load();
        await UniTask.Delay(200);
        await SceneLoadManager.Instance.OnCompleteLoading();
    }
}