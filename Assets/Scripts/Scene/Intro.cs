using Cysharp.Threading.Tasks;

public class Intro : SceneBase
{
    PizzaMenuBG bg;

    protected override async UniTask Load()
    {
        await SceneLoadManager.Instance.OnStartLoading(false);
        if(bg == null) bg = await ResourceLoadManager.Instance.Instantiate<PizzaMenuBG>("BGMenu");
        await UniTask.Delay(1000);
        UIManager.Instance.OpenUI<UIMain>();
    }
}