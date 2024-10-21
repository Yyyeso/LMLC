using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    [SerializeField] Button btnTest;
    [SerializeField] GameObject[] go;

    [SerializeField] SpriteRenderer rend;
    [SerializeField] Transform stage;

    Stack<GameObject> pool = new();

    [SerializeField] Vector2 size = Vector2.one;
    [SerializeField] float delay = 1;
    [SerializeField] float fadeDuration = 0.15f;
    Vector2 center;
    float originRadius;
    float radius;
    enum Type
    {
        Square,
        Circle
    }

    [SerializeField] Transform[] trList;
    [SerializeField] int count;
    [SerializeField] Type type;
    [SerializeField] TMP_Text txtCount;
    int cutCount = 0;
    void Start()
    {
        center = stage.position;
        originRadius = stage.lossyScale.x * 0.5f;

        btnTest.onClick.AddListener(ShredCheese);
    }
    int[] arr = new int[16] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
    void Shuffle()
    {
        for (int i = arr.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (arr[randomIndex], arr[i]) = (arr[i], arr[randomIndex]);
        }
    }

    async void ShredCheese()
    {
        btnTest.interactable = false;
        cutCount = 0;
        radius = originRadius - (size.x * 0.5f);
        for (int i = 0; i < count; i++)
        {
            CreateShredCheese();
            await UniTask.Delay(200);
        }
        await UniTask.Delay((int)(delay * 1000) - 200);
        btnTest.interactable = true;
    }
    void CreateShredCheese()
    {
        Shuffle();
        float radians = Random.Range(0, 37) * 10 * Mathf.Deg2Rad;
        for (int i = 0; i < 10; i++)
        {
            var t = trList[arr[i]];
            GameObject p = (pool.Count <= 0) ? Instantiate(go[(int)type]) : pool.Pop();
            Set(p, t);
            cutCount++;
            txtCount.text = cutCount.ToString();
        }
        Vector2 Rotate(Vector2 vec)
        {
            Vector2 direction = vec - center;
            float x = Mathf.Cos(radians) * direction.x - Mathf.Sin(radians) * direction.y;
            float y = Mathf.Sin(radians) * direction.x + Mathf.Cos(radians) * direction.y;

            return new Vector2(x + center.x, y + center.y);
        }
        async void Set(GameObject p, Transform t)
        {
            var inner = p.GetComponent<RangeEffect>();
            float r = (t.lossyScale.x * 0.5f) - (size.x * 0.5f);
            p.transform.position = Rotate(RandPosInCircle(t.position, r));
            p.transform.localScale = size;
            p.SetActive(true);
            _ = p.GetComponent<SpriteRenderer>().DOFade(0.8f, 0.15f);
            inner.SetInner(delay);
            await UniTask.Delay((int)(delay * 1000));
            await p.GetComponent<SpriteRenderer>().DOFade(0, 0.15f);
            inner.Release();
            pool.Push(p);
        }
    }

    void Kale()
    {
        btnTest.interactable = false;

    }

    async void CherryTomato()
    {
        btnTest.interactable = false;
        radius = originRadius - (size.x * 0.5f);
        for (int i = 0; i < count; i++)
        {
            CreateCherryTomato();
            await UniTask.Delay(100);
        }
        await UniTask.Delay((int)(delay * 1000) - 100);
        btnTest.interactable = true;
    }

    async void CreateCherryTomato()
    {
        GameObject p = (pool.Count <= 0)? Instantiate(go[(int)type]) : pool.Pop();
        var inner = p.GetComponent<RangeEffect>();
        p.transform.position = RandPosInCircle(center, radius);
        p.transform.localScale = size;
        p.SetActive(true);
        _ = p.GetComponent<SpriteRenderer>().DOFade(0.8f, 0.15f);
        inner.SetInner(delay);
        await UniTask.Delay((int)(delay * 1000));
        await p.GetComponent<SpriteRenderer>().DOFade(0, 0.15f);
        inner.Release();
        pool.Push(p);
    }

    Vector2 RandPosInCircle(Vector2 center, float radius)
    {
        float angle = Random.Range(0f, Mathf.PI * 2);
        float distance = Random.Range(0f, radius);

        float x = Mathf.Cos(angle) * distance;
        float y = Mathf.Sin(angle) * distance;

        return new Vector2(center.x + x, center.y + y);
    }
}