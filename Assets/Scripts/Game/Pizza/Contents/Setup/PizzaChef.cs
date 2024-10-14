using DG.Tweening;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

public class PizzaChef : MonoBehaviour
{
    [SerializeField] private Transform temp;
    [SerializeField] private SpriteRenderer[] rend; 
    Transform[] tr = new Transform[2];
    PizzaIngredient[] ingredient = new PizzaIngredient[2];
    Animator anim;
    Color color = Color.white;
    float d = 0.5f;
    readonly string SingleAction = "SingleAction";
    Vector3 vec = Vector3.one * 0.8f;
    CancellationToken token;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        tr[0] = rend[0].transform.parent;
        tr[1] = rend[1].transform.parent;
    }

    public void SetIdle()
    {
        anim.ResetTrigger(SingleAction);
        anim.ResetTrigger("DoubleAction");
        anim.SetBool("Posing", false);
        anim.Play("ChefIdle");
        color.a = 0;
        rend[0].color = color;
        rend[1].color = color;
    }

    public async UniTask SetIngredient(PizzaIngredient ingredient1, bool delay, CancellationToken token)
    {
        this.token = token;
        ingredient[0] = ingredient1;
        if (ingredient1 == PizzaIngredient.CheeseL || ingredient1 == PizzaIngredient.CheeseR)
        {
            vec.x = (ingredient1 == PizzaIngredient.CheeseR) ? -0.8f : 0.8f;
        }
        else
        {
            vec.x *= -1;
        }
        transform.parent.transform.localScale = vec;
        anim.SetTrigger(SingleAction);
        await UniTask.Delay(2000, cancellationToken: token);
        _ = PizzaGameData.Instance.PlaySFX(PizzaSFXType.Cheese);
        await UniTask.Delay(300, cancellationToken: token);
        if (delay) await UniTask.Delay(1935, cancellationToken: token);
    }

    public async UniTask SetIngredient(PizzaIngredient ingredient1, PizzaIngredient ingredient2, bool delay, CancellationToken token)
    {
        this.token = token;
        ingredient[0] = ingredient1;
        ingredient[1] = ingredient2;
        if (ingredient1 == PizzaIngredient.CheeseL || ingredient2 == PizzaIngredient.CheeseL
            || ingredient1 == PizzaIngredient.CheeseR || ingredient2 == PizzaIngredient.CheeseR)
        {
            vec.x = (ingredient1 == PizzaIngredient.CheeseR) ? -0.8f : 0.8f;
        }
        else
        {
            vec.x *= -1;
        }
        transform.parent.transform.localScale = vec;
        anim.SetTrigger("DoubleAction");
        await UniTask.Delay(2000, cancellationToken: token);
        _ = PizzaGameData.Instance.PlaySFX(PizzaSFXType.Cheese);
        await UniTask.Delay(300, cancellationToken: token);
        if (delay) await UniTask.Delay(1935, cancellationToken: token);
    }


    void Setup(int idx)
    {
        rend[idx].transform.SetParent(tr[idx]);
        rend[idx].transform.localPosition = Vector3.right * 0.45f;
        rend[idx].transform.localScale = Vector3.one * 0.65f;
        rend[idx].transform.localEulerAngles = Vector3.forward * (180 * idx);
        rend[idx].sprite = PizzaGameData.Instance.SpriteList.GetIngredientSprite((int)ingredient[idx]);
        color.a = 1;
        rend[idx].color = color;
    }

    void SetupDouble()
    {
        Setup(0);
        Setup(1);
    }

    async void DoubleAction()
    {
        _ = PlayAnim(0);
        await UniTask.Delay(50, cancellationToken: token);
        await PlayAnim(1);
    }

    async UniTask PlayAnim(int idx)
    {
        _ = PizzaGameData.Instance.PlaySFX(PizzaSFXType.Cheese);
        rend[idx].transform.SetParent(temp);
        float t = d * 0.5f;
        _ = rend[idx].transform.DOLocalMove(Vector3.zero, d).SetEase(Ease.OutQuart).ToUniTask(cancellationToken: token);
        await rend[idx].transform.DOScale(Vector3.one * 1.5f, t).ToUniTask(cancellationToken: token);
        await rend[idx].transform.DOScale(Vector3.one * 0.5f, t).ToUniTask(cancellationToken: token);
        await rend[idx].DOFade(0, d).ToUniTask(cancellationToken: token);
    }
}
