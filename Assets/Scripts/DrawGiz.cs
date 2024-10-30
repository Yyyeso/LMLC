using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGiz : MonoBehaviour
{
    [SerializeField] Color color = Color.magenta;

    void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
