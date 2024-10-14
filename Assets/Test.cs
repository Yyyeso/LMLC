using UnityEngine;

public class Test : MonoBehaviour
{
    void Start()
    {
        UIManager.Instance.OpenUI<UILoading>().SetTip("UIManager.Instance.OpenUI<UILoading>().SetTip();");
    }
}