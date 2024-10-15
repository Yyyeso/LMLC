using UnityEngine;

public class Loading : MonoBehaviour
{
    void Awake()
    {
        SceneLoadManager.Instance.LoadSceneAsync();
    }
}