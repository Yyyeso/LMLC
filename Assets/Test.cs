using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class Test : MonoBehaviour
{
    [SerializeField] List<AtkList> atkList;
    [SerializeField] Camera mainCam;
    [SerializeField] MyPlayer pl;
    [SerializeField] Transform plRot;
    [SerializeField] Button btnTest;
    [SerializeField] Button btnDash;
    [SerializeField] Button btnView;
    [SerializeField] Image imgDash;
    [SerializeField] GameObject[] go;

    [SerializeField] SpriteRenderer rend;
    [SerializeField] Transform stage;

    Stack<GameObject> pool = new();

    public MyPlayer Player => pl;

    [SerializeField] Vector2 size = Vector2.one;
    [SerializeField] float delay = 1;
    [SerializeField] float fadeDuration = 0.15f;
    Vector2 center;
    float originRadius;
    float radius;
    public Vector2 Center => center;
    public float OriginRadius => originRadius;
    enum Type
    {
        Square,
        Circle
    }

    public float Delay => delay;
    public float FadeDuration => fadeDuration;

    public Dictionary<ShapeType, Stack<GameObject>> dict = new(6)
    {
        { ShapeType.Circle,      new() },
        { ShapeType.Square,      new() },
        { ShapeType.Lettuce,     new() },
        { ShapeType.Kale,        new() },
        { ShapeType.BeanSprouts, new() },
        { ShapeType.Shiitake,    new() },
    };

    [SerializeField] Transform[] trList;
    [SerializeField] int count;
    [SerializeField] Type type;
    [SerializeField] TMP_Text txtCount;
    [SerializeField] TMP_Text txtAttackName;
    int cutCount = 0;

    
    List<UniTask> taskList = new List<UniTask>();

    public Dictionary<ShapeType, GameObject> shapeList = new();


    async void Play()
    {
        btnTest.interactable = false;
        await SetAttackName("샐러드");
        await UniTask.Delay(1000);

        foreach (var atk in atkList[0].AttackList)
        {
            await SetAttackName(atk.gameObject.name);
            await atk.Play(this);
        }

        await SetAttackName("비빔밥");
        await UniTask.Delay(1000);

        foreach (var atk in atkList[1].AttackList)
        {
            await SetAttackName(atk.gameObject.name);
            await atk.Play(this);
        }

        btnTest.interactable = true;
    }

    private async UniTask LoadAllShape()
    {
        async UniTask LoadShape(ShapeType shapeType)
        {
            shapeList[shapeType] = await ResourceLoadManager.Instance.LoadAssetasync<GameObject>(shapeType.ToString());
        }

        taskList.Add(LoadShape(ShapeType.Circle));
        taskList.Add(LoadShape(ShapeType.Square));
        taskList.Add(LoadShape(ShapeType.Lettuce));
        taskList.Add(LoadShape(ShapeType.Kale));
        taskList.Add(LoadShape(ShapeType.BeanSprouts));
        taskList.Add(LoadShape(ShapeType.Shiitake));

        await UniTask.WhenAll(taskList);
    }

    async void Start()
    {
        center = stage.position;
        originRadius = stage.lossyScale.x * 0.5f;


        await LoadAllShape();

        btnTest.onClick.AddListener(Play);
        btnDash.onClick.AddListener(TestDash);
        btnView.onClick.AddListener(SetCam);
        SetCam();
        await SetAttackName("로드 완료");
    }
    bool isTopView = true;
    void SetCam()
    {
        isTopView = !isTopView;
        var angle = (isTopView) ? Vector3.zero : new(-60, 0, 0);
        //plRot.transform.eulerAngles = angle;
        mainCam.transform.eulerAngles = angle;
        //pl.View = angle;
        mainCam.orthographicSize = (isTopView) ? 5 : 4;
    }

    async void TestDash()
    {
        btnDash.interactable = false;
        pl.Dash();
        imgDash.fillAmount = 1;
        await imgDash.DOFillAmount(0, pl.DashCoolDown);
        btnDash.interactable = true;
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

    async UniTask ShredCheese()
    {
        //btnTest.interactable = false;
        await SetAttackName("ShredCheese");
        cutCount = 0;
        radius = originRadius - (size.x * 0.5f);
        for (int i = 0; i < count; i++)
        {
            CreateShredCheese();
            await UniTask.Delay(200);
        }
        await UniTask.Delay((int)(delay * 1000) - 200);
        //txtCount.text = string.Empty;
        //btnTest.interactable = true;
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

    async UniTask SetAttackName(string name)
    {
        txtAttackName.text = name;
        await UniTask.Delay(1000);
        txtAttackName.text = string.Empty;
    }

    async void CherryTomato()
    {
        btnTest.interactable = false;
        await SetAttackName("방울토마토");
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