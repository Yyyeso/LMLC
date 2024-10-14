using UnityEngine;
using UnityEngine.U2D.Animation;


public class PizzaSetCharacter : MonoBehaviour
{
    [SerializeField] private SpriteResolver spriteResolver;
    [SerializeField] private SpriteRenderer[] bodyRenderers;
    [SerializeField] private SpriteRenderer[] eyeRenderers;

    string[] characterColor = new string[10] 
    { "#E13418", "#7E1200", "#ECA200", "#A4412F", "#7E5929", "#915D2D", "#3E6742", "#DE8500", "#66303D", "#9A3300" };
    string[] eyeColor = new string[10] 
    { "#4F332A", "#4D1506", "#2C74AE", "#5A0800", "#5A0800", "#5A0800", "#312A3A", "#612B12", "#550010", "#550010" };
    string[] characterBody = new string[10] 
    { "Body1", "Body2", "Body3", "Body4", "Body5", "Body6", "Body7", "Body8", "Body9", "Body10" };

    PizzaGameData data;


    public void SetCharacter(int idx)
    {
        data ??= PizzaGameData.Instance;

        spriteResolver.SetCategoryAndLabel("Body", characterBody[idx]);
        Color c = data.GetColor(characterColor[idx]);
        c.a = 1;
        Color.RGBToHSV(c, out float h, out float s, out float v);
        if (v < 0.15f) { v = 0.15f; }
        Color color = Color.HSVToRGB(h, s, v);
        foreach (var renderer in bodyRenderers) { renderer.color = color; }
        eyeRenderers[0].color = eyeRenderers[1].color = data.GetColor(eyeColor[idx]);
    }
}