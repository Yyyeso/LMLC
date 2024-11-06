using Cysharp.Threading.Tasks;

public class Game : SceneBase
{
    protected override async UniTask Load()
    {
        await UniTask.Delay(1000);
        await GameManager.Instance.Setup(null);
    }
}