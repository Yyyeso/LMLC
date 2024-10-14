using UnityEngine;

public class PizzaMenuBG : MonoBehaviour
{
    [SerializeField] private Animator chef;
    [SerializeField] private Animator[] anim;
    readonly string Run = "Run";

    public Animator Chef => chef;


    void Start()
    {
        foreach (var a in anim)
        { a.SetBool(Run, true); }
    }
}