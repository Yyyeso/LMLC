using UnityEngine;

public class DrawGiz : MonoBehaviour
{
    [SerializeField] Color color = Color.magenta;
    [SerializeField] float radius = 0.5f;


    void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}