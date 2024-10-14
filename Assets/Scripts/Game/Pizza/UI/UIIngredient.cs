using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIIngredient : MonoBehaviour
{
    [SerializeField] Image img;
    [SerializeField] TMP_Text txt;
    float timer;
    bool checkTime = false;
    Action<UIIngredient> PushAction;

    public void Setup(PizzaIngredient type, float time, Transform parent)
    {
        img.sprite = PizzaGameData.Instance.SpriteList.GetIngredientSprite((int)type);
        timer = time;
        txt.text = time.ToString();
        transform.SetParent(parent);
        transform.localScale = Vector3.one;
        gameObject.SetActive(true);
        checkTime = true;
    }
    void Push()
    {
        checkTime = false; 
        PushAction.Invoke(this);
        gameObject.SetActive(false); 
    }

    public void SetPushAction(Action<UIIngredient> action) => PushAction = action;

    void Update()
    {
        if (!checkTime) return;
        if (timer <= 0) { Push(); }

        timer -= Time.deltaTime;
        txt.text = $"{Math.Round(timer, 1)}";
    }
}

public enum PizzaIngredient
{
    TomatoPaste,
    SauceWhite,
    SauceBrown,
    CheeseL,
    CheeseR,
    BellPepper,
    Mushroom,
    Olive,
    Pepperoni,
    Corn,
    RibPattie,
    MaxCount
}