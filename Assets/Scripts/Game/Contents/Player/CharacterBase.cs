using UnityEngine;

public abstract class CharacterBase : MonoBehaviour
{
    int hp;
    float speed;
    int cc;
    int ce;
    int mvpc;
    CharacterType type;
    protected abstract void Skill();
    public void Release()
    {
    }
}