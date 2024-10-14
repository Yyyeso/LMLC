using UnityEngine;

public class PizzaResultBG : MonoBehaviour
{
    [SerializeField] private PizzaSetCharacter characterSetter;
    [SerializeField] private Animator character;
    readonly string Clear = "Clear";
    readonly string Fail = "Fail";

    public void SetAnim(bool isClear)
    {
        characterSetter.SetCharacter(PizzaGameData.Instance.CharacterIndex);

        if (isClear) character.SetTrigger(Clear);
        else character.SetTrigger(Fail);
    }
    public void SetAnim()
    {
        characterSetter.SetCharacter(PizzaGameData.Instance.CharacterIndex);
        character.Play("Idle");
    }
}