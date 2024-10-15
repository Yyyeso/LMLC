using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class SceneLoadManager : Singleton<SceneLoadManager>, IDonDestroy
{
    #region UI
    UILoading ui;
    UILoading UI => ui = (ui != null)? ui : CreateUI();


    UILoading CreateUI()
    {
        var go = Instantiate(Resources.Load<GameObject>("UI/UILoading"), transform.parent);
        return go.GetComponent<UILoading>();
    }

    public void OnStartLoading() => UI.gameObject.SetActive(true);

    public void OnCompleteLoading() => UI.CloseUI();
    #endregion

    SceneType sceneType;

    public void LoadScene(SceneType type)
    {
        OnStartLoading();
        sceneType = type;
        SceneManager.LoadScene(Const.GetSceneName(SceneType.Loading));
    }

    public async void LoadSceneAsync() 
    {
        await SceneManager.LoadSceneAsync(Const.GetSceneName(sceneType));
    }

    //public async UniTask LoadSceneAsync()
    //{
    //await UniTask.Yield();

    //AsyncOperation op = SceneManager.LoadSceneAsync(Const.GetSceneName(sceneType));
    //op.allowSceneActivation = false;

    //float timer = 0.0f;

    //while (!op.isDone)
    //{
    //    await UniTask.Yield();
    //    timer += Time.deltaTime;
    //    print(op.progress * 100);
    //    if (op.progress == 1.0f)
    //    {
    //        op.allowSceneActivation = true;
    //        break;
    //    }
    //}
    //}
}