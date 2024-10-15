using TMPro;
using UnityEngine;

public class UIGameMulti : UIGameBase
{
    [SerializeField] private TMP_Text txtTeamScore;
    [SerializeField] private RectTransform rectOurTeam;
    [SerializeField] private RectTransform rectArrow;


    public void SetTeamScore(int left, int right) => txtTeamScore.text = $"<color=red>{left}</color>:<color=green>{right}</color>";

    public UIGameMulti SetTeam(bool isLeft)
    {
        rectOurTeam.anchoredPosition = TeamPos(isLeft);
        rectArrow.eulerAngles = ArrowRot(isLeft);
        return this;
    }

    Vector3 TeamPos(bool isLeft) => (isLeft) ? new(-41, 0, 0) : new(41, 0, 0);

    Vector3 ArrowRot(bool isLeft) => (isLeft) ? new(0, 180, 180) : new(0, 0, 180);
}