using Cysharp.Threading.Tasks;

public class Game : SceneBase
{
    protected override async UniTask Load()
    {
        await UniTask.Delay(1000);
        SceneLoadManager.Instance.LoadScene(SceneType.Intro);
    }
}