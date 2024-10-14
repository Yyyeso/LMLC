using UnityEngine;

public class PizzaAttackList : MonoBehaviour
{
    [SerializeField] private GameObject[] attackList;
    PizzaAttack[] initAttack = new PizzaAttack[(int)PizzaIngredient.MaxCount];
    PizzaAttack[] attackPool = new PizzaAttack[(int)PizzaIngredient.MaxCount];
    Transform parent;
    bool[] isFirst = new bool[11] 
    { true, true, true, true, true, true, true, true, true, true, true };

    public PizzaAttackList Init(Transform parent)
    {
        this.parent = parent;
        for (int i = 0; i < (int)PizzaIngredient.MaxCount; i++)
        { 
            initAttack[i] = attackPool[i] = null;
        }
        return this;
    }

    public PizzaAttack Pop(PizzaIngredient type)
    {
        if (attackPool[(int)type] == null && initAttack[(int)type] == null)
        {
            var go = Instantiate(attackList[(int)type], parent);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.transform.localRotation = Quaternion.identity;

            initAttack[(int)type] = go.GetComponent<PizzaAttack>().Setup();
            initAttack[(int)type].SetPushAction(() => { Push(type); });
        }
        else
        {
            if (attackPool[(int)type] == null)
            {
                var go = Instantiate(attackList[(int)type], parent);
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;
                go.transform.localRotation = Quaternion.identity;

                attackPool[(int)type] = initAttack[(int)type];
                initAttack[(int)type] = go.GetComponent<PizzaAttack>().Setup();
                initAttack[(int)type].SetPushAction(() => { Push(type); });
            }
            else
            {
                (attackPool[(int)type], initAttack[(int)type]) = (initAttack[(int)type], attackPool[(int)type]);
            }
        }
        initAttack[(int)type].IsFirst = isFirst[(int)type];
        isFirst[(int)type] = false;
        initAttack[(int)type].gameObject.SetActive(true);
        initAttack[(int)type].ResetAttack();
        return initAttack[(int)type];
    }

    void Push(PizzaIngredient type)
    {
        attackPool[(int)type]?.Push();
    }

    public void ResetAttack()
    {
        foreach (var attack in initAttack) { attack?.ResetAttack(); }
    }

    public void RestartAttack()
    {
        for (int i = 0; i < isFirst.Length; i++) { isFirst[i] = true; }
        ResetAttack();
    }
}
