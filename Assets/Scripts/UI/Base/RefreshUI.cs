using UnityEngine;
using Cysharp.Threading.Tasks;

public class RefreshUI : MonoBehaviour
{
    [SerializeField] private RectTransform[] _rects;


    /// <summary> UI 정렬 </summary>
    public async void Refresh(float time = 0.01f)
    {
        if (_rects.Length < 0 || !gameObject.activeInHierarchy) return;

        int delay = (int)(1000 * time);
        await UniTask.Delay(delay);
        ForceRebuild();
    }

    void ForceRebuild()
    {
        foreach (RectTransform rect in _rects)
        {
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }
    }
}