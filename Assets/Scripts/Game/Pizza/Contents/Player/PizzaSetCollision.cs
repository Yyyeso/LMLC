using UnityEngine;

public class PizzaSetCollision : MonoBehaviour
{
    readonly string Wall = "Wall";
    readonly string Ingredient = "Ingredient";
    bool triggerEnter = false;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(Wall) && !triggerEnter) { PizzaGameData.Instance.Player.OnCollision = true; }
        
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(Wall)) { PizzaGameData.Instance.Player.OnCollision = false; }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Ingredient)) { triggerEnter = true; }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Ingredient)) { triggerEnter = false; }
    }
}
