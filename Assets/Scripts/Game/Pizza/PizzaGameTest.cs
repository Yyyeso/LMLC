using UnityEngine;
using UnityEngine.SceneManagement;

public class PizzaGameTest : MonoBehaviour
{
    [SerializeField] private bool test;


    private void Start()
    {
        if (test) return;
        SceneManager.LoadScene(nameof(PizzaLobbyScene));
    }
}