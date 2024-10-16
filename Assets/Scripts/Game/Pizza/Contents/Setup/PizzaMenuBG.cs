using UnityEngine;

public class PizzaMenuBG : MonoBehaviour
{
    [SerializeField] private Animator chef;
    [SerializeField] private Animator[] yummies;

    const string Run = "Run";
    const string Posing = "Posing";

    public void SetAnim(bool value)
    {
        chef.SetBool(Posing, value);
        foreach (var yum in yummies) { yum.SetBool(Run, value); }
    }

    private void OnEnable() => SetAnim(true);

    private void OnDisable() => SetAnim(false);
}