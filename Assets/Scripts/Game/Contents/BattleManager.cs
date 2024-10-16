
using Cysharp.Threading.Tasks;

public class BattleManager : Singleton<BattleManager>
{
    private bool _isRoundEnd => CurRound == LastRound;

    public int StageLevel { get; private set; }
    public int LastRound { get; private set; }
    public int CurRound { get; private set; }
    public bool IsGameEnd { get; private set; }

    public void Initialize(int stageLevel)
    {
        StageLevel = stageLevel;

        LoadCurrentStage();
        ReadyForNext();
    }

    void LoadCurrentStage()
    {
    }

    public void ReadyForNext()
    {
        CurRound++;
    }

    public void StartRound()
    {
    }

    async void OnPatternCompleted()
    {
        await UniTask.Delay(100);
        //await UniTask.WaitUntil(() => monsterManager.SpawnedMonsterList.Count <= 0);
        OnRoundFinished();
    }

    void OnRoundFinished()
    {
        if (_isRoundEnd)
        {
            GameWin();
            return;
        }

        ReadyForNext();
    }

    void GameWin()
    {
        if (!IsGameEnd)
            IsGameEnd = true;
        else return;
    }

    public void GameOver()
    {
        if (!IsGameEnd)
            IsGameEnd = true;
        else return;
    }
}