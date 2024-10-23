using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ShredCheese : TestRange
{
    [SerializeField] List<Transform> transforms;

    int[] arr = new int[16] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
    void Shuffle()
    {
        for (int i = arr.Length - 1; i > 0; i--)
        {
            Shuffle();
            int randomIndex = Random.Range(0, i + 1);
            (arr[randomIndex], arr[i]) = (arr[i], arr[randomIndex]);
        }
    }
    float radians;
    protected override void Create(int idx)
    {
        Shuffle();
        radians = Random.Range(0, 37) * 10 * Mathf.Deg2Rad;

        for (int i = 0; i < 10; i++)
        {
            CreateRange(arr[i]);
        }
    }

    Vector2 Rotate(Vector2 vec)
    {
        Vector2 direction = vec - (Vector2)center;
        float x = Mathf.Cos(radians) * direction.x - Mathf.Sin(radians) * direction.y;
        float y = Mathf.Sin(radians) * direction.x + Mathf.Cos(radians) * direction.y;

        return new Vector2(x + center.x, y + center.y);
    }

    protected override Vector3 GetPos(int idx)
    {
        var t = transforms[idx];
        float r = (t.lossyScale.x * 0.5f) - (size.x * 0.5f);
        return Rotate(RandPosInCircle(t.position, r));
    }
}