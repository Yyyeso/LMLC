using DG.Tweening;
using UnityEngine;

public class TestPlayerOption : MonoBehaviour
{
    [SerializeField] GameObject[] pl;
    [SerializeField] MyPlayer pl1;
    [SerializeField] MyPlayer pl2;

    [Header("옵션 설정")]
    [Tooltip("플레이어 겹침 여부는 플레이 전 설정해주세요.")]
    [SerializeField] bool 플레이어겹침;

    [SerializeField] float 이동속도 = 1f;
    [SerializeField] float 대쉬거리 = 1f;
    [SerializeField] float 대쉬속도 = 0.15f;
    [SerializeField] float 대쉬쿨다운 = 10;
    [SerializeField] Ease dashEase = Ease.InOutQuad;

    bool isStarted;

    private void Awake()
    {
        isStarted = true;
    }

    private void OnValidate()
    {
        if (!isStarted)
        {
            pl[0].SetActive(플레이어겹침);
            pl[1].SetActive(!플레이어겹침);
        }

        if (대쉬속도 <= 0)
        {
            대쉬속도 = 0.01f;
        }
        if (대쉬쿨다운 <= 대쉬속도)
        {
            대쉬쿨다운 = 대쉬속도 + 0.1f;
        }

        pl1.TestOption(이동속도, 대쉬거리, 대쉬속도, dashEase);
        pl2.TestOption(이동속도, 대쉬거리, 대쉬속도, dashEase);
        CharacterSkill.dashCoolDown = 대쉬쿨다운;
    }

}