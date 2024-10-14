using Photon.Pun;
using UnityEngine;

public class PizzaResources : Singleton<PizzaResources>
{
    GameObject gameParent = null;
    public Transform GameParent
    {
        get
        {
            if (gameParent == null)
            {
                gameParent = GameObject.Find("[GameObject]");
                if (gameParent == null)
                {
                    gameParent = new GameObject("[GameObject]");
                }
            }
            return gameParent.transform;
        }
    }
    public UIPizzaGameLoading UILoading => Instantiate(UILoadingPath, UIManager.ParentUI).GetComponent<UIPizzaGameLoading>();
    public PizzaPlayerController Player => Instantiate<PizzaPlayerController>(PlayerPath, GameParent);
    public PizzaStage             Stage => Instantiate<PizzaStage>(nameof(PizzaStage), GameParent);
    public PizzaMenuBG           BGMenu => Instantiate<PizzaMenuBG>(nameof(PizzaMenuBG), GameParent);
    public PizzaResultBG       BGResult => Instantiate<PizzaResultBG>(nameof(PizzaResultBG), GameParent);
    public PizzaSpriteList   SpriteList => Instantiate<PizzaSpriteList>(nameof(PizzaSpriteList), GameParent);
    public PizzaSoundList     SoundList => Instantiate<PizzaSoundList>(nameof(PizzaSoundList), GameParent);
    public PizzaAttackList   AttackList => Instantiate<PizzaAttackList>(nameof(PizzaAttackList), GameParent);
    public PizzaPlayerController PlayerMulti => InstantiatePlayer();

    #region
    readonly string UILoadingPath = "UI/UIPizzaGameLoading";
    readonly string PlayerPath = "PizzaPlayer";
    //readonly string MultiPlayerPath = "PizzaPlayerMulti";

    GameObject Instantiate(string path, Transform parent = null) => Instantiate(Resources.Load<GameObject>(path), parent);
    T Instantiate<T>(string path, Transform parent = null) where T : Component
    {
        var go = Instantiate("Prefabs/" + path, parent).GetComponent<T>();
        go.name = path;
        return go;
    }
    public PizzaPlayerController InstantiatePlayer()
    {
        Vector3 vec = new(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
        var player = PhotonNetwork.Instantiate("Prefabs/PizzaPlayerMulti", Vector3.zero, Quaternion.Euler(vec));
        player.name = $"Player_{PizzaGameData.Instance.NickName}";
        return player.GetComponent<PizzaPlayerController>();
    }
    #endregion
}