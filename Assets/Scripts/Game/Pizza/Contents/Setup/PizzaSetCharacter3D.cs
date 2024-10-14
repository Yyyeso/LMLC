using UnityEngine;

public class PizzaSetCharacter3D : MonoBehaviour
{
    [SerializeField] GameObject[] body;

    public void SetCharacter(int idx)
    {
        for (int i = 0; i < body.Length; i++)
        {
            body[i].SetActive(i == idx);
        }
    }
}