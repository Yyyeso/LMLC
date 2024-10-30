using UnityEngine;

public class AtkTest : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Const.Player))
        {
            Color c = Color.magenta;
            c.a = 0.6f;
            spriteRenderer.color = c;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(Const.Player))
        {
            Color c = Color.green;
            c.a = 0.6f;
            spriteRenderer.color = c;
        }
    }
}