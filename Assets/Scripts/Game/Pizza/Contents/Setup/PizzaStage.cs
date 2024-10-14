using UnityEngine;

public class PizzaStage : MonoBehaviour
{
    [SerializeField] private PizzaChef chef;
    [SerializeField] private PizzaAttackArea area;
    [SerializeField] private Transform ingredientParent;
    [SerializeField] private Transform stage;
    [SerializeField] private Collider2D colStage;


    public PizzaChef Chef => chef;
    public PizzaAttackArea AttackArea => area;
    public Transform IngredientParent => ingredientParent;
    public Transform Stage => stage;
    public Collider2D StageCollider => colStage;

}