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

    public async UniTask OnStartLoading(bool useTransition = true)
    {
        UI.gameObject.SetActive(true);
        if (useTransition) await UI.Transition(true);
    }

    public async UniTask OnCompleteLoading()
    {
        await UI.Transition(false);
        UI.CloseUI();
    }
    #endregion

    SceneType sceneType;

    public async void LoadScene(SceneType type)
    {
        await OnStartLoading();
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