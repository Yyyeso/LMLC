using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TriggerCheck : MonoBehaviour
{
    public LayerMask layerMask;
    public MyPlayer go;
    float detectionRadius;


    public MyPlayer DDDDD()
    {
        float distance = Vector3.Distance(go.transform.position, transform.position);
        if (distance <= transform.lossyScale.x * 0.5f) return go;
        else return null;
    }

    public List<MyPlayer> Check()
    {
        detectionRadius = transform.lossyScale.x * 0.5f;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, layerMask);

        print(colliders.Length);
        if (colliders.Length <= 0) return null;

        List<MyPlayer> list = new();
        foreach (Collider2D collider in colliders)
        {
            if (collider.transform.parent.TryGetComponent<MyPlayer>(out var pl))
            {
                list.Add(pl);
            }
        }
        return list;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        detectionRadius = transform.lossyScale.x * 0.5f;

        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
