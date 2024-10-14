using UnityEngine;

public class PizzaAttackArea : MonoBehaviour
{
    [SerializeField] private GameObject tomatoPaste;
    [SerializeField] private GameObject bellPepper;
    [SerializeField] private GameObject mushroom;
    [SerializeField] private GameObject cheeseLeft;
    [SerializeField] private GameObject cheeseRight;
    [SerializeField] private GameObject sauceWhite;
    [SerializeField] private GameObject sauceBrown;
    [SerializeField] private GameObject[] olive;
    [SerializeField] private GameObject[] pepperoni;
    [SerializeField] private GameObject corn;
    [SerializeField] private GameObject[] ribPattie;

    public GameObject TomatoPaste => tomatoPaste;
    public GameObject BellPepper => bellPepper;
    public GameObject Mushroom => mushroom;
    public GameObject CheeseLeft => cheeseLeft;
    public GameObject CheeseRight => cheeseRight;
    public GameObject SauceWhite => sauceWhite;
    public GameObject SauceBrown => sauceBrown;
    public GameObject[] Olive => olive;
    public GameObject[] Pepperoni => pepperoni;
    public GameObject Corn => corn;
    public GameObject[] RibPattie => ribPattie;
}
