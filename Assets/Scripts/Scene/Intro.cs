using Cysharp.Threading.Tasks;

public class Intro : SceneBase
{
    protected override async UniTask Load()
    {
        SceneLoadManager.Instance.OnStartLoading();
        await UniTask.Delay(1000);
        UIManager.Instance.OpenUI<UIMain>();
    }
}