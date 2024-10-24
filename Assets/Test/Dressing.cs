using Cysharp.Threading.Tasks;

public class Dressing : TestRange
{
    protected override async UniTask Create(int idx)
    {
        await UIManager.Instance.OpenUI<UIPopUpNotice>().SetMessage("미구현", Delay);
    }
}