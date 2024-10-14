using UnityEngine;

public class PizzaSpriteList : MonoBehaviour
{
    [SerializeField] private Sprite[] ingredient;
    [SerializeField] private Sprite[] rps;


    public Sprite GetIngredientSprite(int idx) => ingredient[idx];

    public Sprite GetRpsSprite(int idx) => rps[idx];
}