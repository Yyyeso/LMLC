using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Color = UnityEngine.Color;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using Photon.Pun;

public class PizzaGameManager : Singleton<PizzaGameManager>
{
    UIPizzaGameScene uiGame;
    UIPizzaGameCount uiCount;
    PizzaStage stage;
    PizzaChef chef;

    CancellationTokenSource cancel;
    CancellationTokenSource linked;

    IPizzaGameManager gm;
    PizzaGameData data;
    UIManager ui;


    #region Setup
    public void SetGame(IPizzaGameManager pizzaGame)
    {
        gm = pizzaGame;
        ui = UIManager.Instance;
        data = PizzaGameData.Instance;

        gm.LoadPlayer();
    }

    public async void LoadGame()
    {
        LoadStage();
        LoadUI();
        SetGameData();

        PizzaGameData.Instance.OnCompleteLoading();
        if (data.PizzaGameMulti != null)
        {
            await data.PizzaGameMulti.NoticeTeam();
        }
        await gm.OnReady();
    }

    void LoadStage()
    {
        stage = PizzaResources.Instance.Stage;
        data.Stage = stage.Stage;
        data.AttackArea = stage.AttackArea;
        data.AttackList.Init(stage.IngredientParent);
        chef = stage.Chef;
    }

    void LoadUI()
    {
        uiGame = gm.UIGame;
        uiCount = ui.OpenUI<UIPizzaGameCount>().SetFreezeAction((f) => { data.Player.IsFreeze = f; });
    }

    void SetGameData()
    {
        round = 0;
        data.GameOver = false;
        uiGame.gameObject.SetActive(true);

        chef.SetIdle();
        gm.SetGameData();
        gm.SetGame();
        stage.StageCollider.enabled = false;

        cancel = new();
        linked = CancellationTokenSource.CreateLinkedTokenSource(cancel.Token, this.GetCancellationTokenOnDestroy());
    }

    public void StartGame()
    {
        gm.StartGame();
        data.PlayBGM(PizzaBGMType.Game, 0.25f);
    }

    public async void RestartGame()
    {
        PizzaGameData.Instance.OnLoading();

        if (!cancel.IsCancellationRequested) cancel.Cancel();
        data.AttackList.RestartAttack();
        SetGameData();
        await UniTask.Delay(1200);

        PizzaGameData.Instance.OnCompleteLoading();
        StartGame();
    }
    #endregion

    #region Game
    //bool IsClosed => round >= 3;
    bool IsClosed => round >= 3;
    int curStep;
    float maxStep;

    public async void StartRound()
    {
        if (IsClosed)
        {
            GameOver(true);
            return;
        }
        try
        {
            curStep = 0;
            uiGame.SetRound(round);
            await uiCount.SetText(round, token: linked.Token);

            await GetAttackPattern(round);
            await UniTask.Delay(3000, cancellationToken: linked.Token);

            round++;
            data.AttackList.ResetAttack();
            await UniTask.Delay(300, cancellationToken: linked.Token);

            StartRound();
        }
        catch (OperationCanceledException ex)
        {
            print(ex.Message);
            return;
        }
    }

    #endregion

    #region Attack Pattern
    int randType1, randType2, orderIdx, typeIdx, round;
    AddCorn cornType;
    List<int[]> orderList = new(3)
    {
        new int[] { 0, 3, 3, 7, 7, 1, 1 },
        new int[] { 0, 5, 5, 9, 5, 7, 3, 1, 0, 9, 7, 3, 5, 2 },
        new int[] { 3, 1, 0, 10, 9, 9, 5, 5, 7, 5, 1, 0, 3, 5, 1, 5, 7, 3, 3, 7, 1, 1, 5, 0 }, };
    List<int[]> typeList = new(3)
    {
        new int[] { 0, 1, 2, 1, 2, 1, 2 },
        new int[] { 0, 1, 2, 0, 3, 4, 3, 4 },
        new int[] { 4, 0, 0, 3, 3, 4, 4, 4, 4, 4 }, };

    int[] CurOrderList => orderList[round];
    int[] CurTypeList => typeList[round];
    PizzaIngredient CurAttack
    {
        get
        {
            return CurTypeList[typeIdx] switch
            {
                0 => (PizzaIngredient)CurOrderList[orderIdx],
                1 => (PizzaIngredient)(CurOrderList[orderIdx] + randType1),
                2 => (PizzaIngredient)(CurOrderList[orderIdx] - randType1 + 1),
                _ => (PizzaIngredient)CurOrderList[orderIdx]
            };
        }
    }
    bool AddHead() => (cornType == AddCorn.Head || cornType == AddCorn.Both);
    bool AddTail() => (cornType == AddCorn.Tail || cornType == AddCorn.Both);
    void GetFirstAttack(out PizzaIngredient atk1, out PizzaIngredient atk2)
    {
        atk1 = (CurOrderList[orderIdx] == 9) ? PizzaIngredient.Corn : (PizzaIngredient)(CurOrderList[orderIdx] + randType1);
        atk2 = (CurOrderList[orderIdx + 1] == 9) ? PizzaIngredient.Corn : (PizzaIngredient)(CurOrderList[orderIdx + 1] + randType2);
    }
    AddCorn GetSecondAttack(out PizzaIngredient atk3, out PizzaIngredient atk4)
    {
        atk3 = (PizzaIngredient)(CurOrderList[orderIdx] - randType1 + 1);
        atk4 = (PizzaIngredient)(CurOrderList[orderIdx + 1] - randType2 + 1);
        return (AddCorn)CurOrderList[orderIdx + 2];
    }
    public async UniTask PlayCasting()
    {
        curAttack = 0;
        await Casting(CurAttack, false);
        if (CurAttack == PizzaIngredient.Pepperoni || CurAttack == PizzaIngredient.Olive)
        {
            print($"Set Scatter Value: {CurAttack}");
            if (CurAttack == PizzaIngredient.Pepperoni) PhotonNetwork.LocalPlayer.SetCustomProperties(SetPepperoniPos);
            else if (CurAttack == PizzaIngredient.Olive) PhotonNetwork.LocalPlayer.SetCustomProperties(SetOlivePos);
        }
        else
        {
            SetReadyAttack();
        }
    }
    int curAttack;
    public void SetReadyAttack()
    {
        switch (curAttack)
        {
            case 0:
                PhotonNetwork.LocalPlayer.SetCustomProperties(ReadyAttack1);
                break;
            case 1:
                PhotonNetwork.LocalPlayer.SetCustomProperties(ReadyAttack2);
                break;
            case 2:
                PhotonNetwork.LocalPlayer.SetCustomProperties(ReadyAttack3First);
                break;
            case 3:
                PhotonNetwork.LocalPlayer.SetCustomProperties(ReadyAttack3Second);
                break;
            default:
                PhotonNetwork.LocalPlayer.SetCustomProperties(ReadyAttack1);
                break;
        }
    }

    public async UniTask PlayCasting1(byte rand)
    {
        curAttack = 0;
        randType1 = rand;
        await Casting(CurAttack, false);
        if (CurAttack == PizzaIngredient.Pepperoni || CurAttack == PizzaIngredient.Olive)
        {
            print($"Set Scatter Value: {CurAttack}");
            if (CurAttack == PizzaIngredient.Pepperoni) PhotonNetwork.LocalPlayer.SetCustomProperties(SetPepperoniPos);
            else if (CurAttack == PizzaIngredient.Olive) PhotonNetwork.LocalPlayer.SetCustomProperties(SetOlivePos);
        }
        else
        {
            SetReadyAttack();
        }
    }
    public async UniTask PlayCasting2(byte rand1, byte rand2)
    {
        curAttack = 1;
        randType1 = rand1;
        randType2 = rand2;
        GetFirstAttack(out PizzaIngredient atk1, out PizzaIngredient atk2);
        await Casting(atk1, atk2, false);

        if (atk1 == PizzaIngredient.Pepperoni || atk1 == PizzaIngredient.Olive || atk2 == PizzaIngredient.Pepperoni || atk2 == PizzaIngredient.Olive)
        {
            print($"Set Scatter Value: {atk1}/{atk2}");
            if (atk1 == PizzaIngredient.Pepperoni) PhotonNetwork.LocalPlayer.SetCustomProperties(SetPepperoniPos);
            else if (atk1 == PizzaIngredient.Olive) PhotonNetwork.LocalPlayer.SetCustomProperties(SetOlivePos);
            if (atk2 == PizzaIngredient.Pepperoni) PhotonNetwork.LocalPlayer.SetCustomProperties(SetPepperoniPos);
            else if (atk2 == PizzaIngredient.Olive) PhotonNetwork.LocalPlayer.SetCustomProperties(SetOlivePos);
        }
        else
        {
            SetReadyAttack();
        }
    }
    public async UniTask PlayCasting3(byte rand1, byte rand2)
    {
        curAttack = 2;
        randType1 = rand1;
        randType2 = rand2;
        GetFirstAttack(out PizzaIngredient atk1, out PizzaIngredient atk2);
        cornType = GetSecondAttack(out PizzaIngredient atk3, out PizzaIngredient atk4);

        await Casting(atk1, atk2, true);
        if (AddHead()) await Casting(PizzaIngredient.Corn, true);
        await Casting(atk3, atk4, true);
        if (AddTail()) await Casting(PizzaIngredient.Corn, false);

        if (atk1 == PizzaIngredient.Pepperoni || atk1 == PizzaIngredient.Olive || atk2 == PizzaIngredient.Pepperoni || atk2 == PizzaIngredient.Olive)
        {
            print($"Set Scatter Value: {atk1}/{atk2}");
            if (atk1 == PizzaIngredient.Pepperoni) PhotonNetwork.LocalPlayer.SetCustomProperties(SetPepperoniPos);
            else if (atk1 == PizzaIngredient.Olive) PhotonNetwork.LocalPlayer.SetCustomProperties(SetOlivePos);
            if (atk2 == PizzaIngredient.Pepperoni) PhotonNetwork.LocalPlayer.SetCustomProperties(SetPepperoniPos);
            else if (atk2 == PizzaIngredient.Olive) PhotonNetwork.LocalPlayer.SetCustomProperties(SetOlivePos);
        }
        else
        {
            SetReadyAttack();
        }
    }
    public async UniTask PlayAttack1()
    {
        await BasicAttack(CurAttack);
        orderIdx++;
        typeIdx++;
        if (orderIdx >= CurOrderList.Length)
        {
            await UniTask.Delay(3000, cancellationToken: linked.Token);
            round++;
            data.AttackList.ResetAttack();
            await UniTask.Delay(300, cancellationToken: linked.Token);
            NextPattern();
        } 
        else NextCasting();
    }
    public async UniTask PlayAttack2()
    {
        GetFirstAttack(out PizzaIngredient atk1, out PizzaIngredient atk2);
        await SimulAttack(atk1, atk2);
        orderIdx += 2;
        typeIdx++;
        if (orderIdx >= CurOrderList.Length)
        {
            await UniTask.Delay(3000, cancellationToken: linked.Token);
            round++;
            data.AttackList.ResetAttack();
            await UniTask.Delay(300, cancellationToken: linked.Token);
            NextPattern();
        }
        else NextCasting();
    }
    public async UniTask PlayAttack3First()
    {
        curAttack = 3;
        GetFirstAttack (out PizzaIngredient atk1, out PizzaIngredient atk2);

        if (AddHead()) _ = Attack(PizzaIngredient.Corn);
        await SimulAttack(atk1, atk2);
        await UniTask.Delay((int)(1.5f * 1000));

        GetSecondAttack(out PizzaIngredient atk3, out PizzaIngredient atk4);
        if (atk3 == PizzaIngredient.Pepperoni || atk3 == PizzaIngredient.Olive || atk4 == PizzaIngredient.Pepperoni || atk4 == PizzaIngredient.Olive)
        {
            print($"Set Scatter Value: {atk3}/{atk4}");
            if (atk3 == PizzaIngredient.Pepperoni) PhotonNetwork.LocalPlayer.SetCustomProperties(SetPepperoniPos);
            else if (atk3 == PizzaIngredient.Olive) PhotonNetwork.LocalPlayer.SetCustomProperties(SetOlivePos);
            if (atk4 == PizzaIngredient.Pepperoni) PhotonNetwork.LocalPlayer.SetCustomProperties(SetPepperoniPos);
            else if (atk4 == PizzaIngredient.Olive) PhotonNetwork.LocalPlayer.SetCustomProperties(SetOlivePos);
        }
        else
        {
            SetReadyAttack();
        }
    }
    public async UniTask PlayAttack3Second()
    {
        GetSecondAttack(out PizzaIngredient atk3, out PizzaIngredient atk4);

        if (AddTail()) _ = Attack(PizzaIngredient.Corn);
        await SimulAttack(atk3, atk4);
        orderIdx += 3;
        typeIdx++;
        if (orderIdx >= CurOrderList.Length)
        {
            await UniTask.Delay(3000, cancellationToken: linked.Token);
            round++;
            data.AttackList.ResetAttack();
            await UniTask.Delay(300, cancellationToken: linked.Token);
            NextPattern();
        }
        else NextCasting();
    }
    public async void NextPattern()
    {
        if (IsClosed)
        {
            GameOver(true);
            return;
        }
        curStep = orderIdx = typeIdx = 0;
        uiGame.SetRound(round);
        await uiCount.SetText(round, token: linked.Token);

        switch (round)
        {
            case 0:
                maxStep = 7;
                break;
            case 1:
                maxStep = 10;
                break;
            case 2:
                maxStep = 16;
                break;
            default:
                break;
        }

        NextCasting();
    }
    public void NextCasting()
    {
        if (data.MeatScore <= 0 || data.VegeScore <= 0)
        {
            gm.SetResult();
        }

        switch (CurTypeList[typeIdx])
        {
            case 0:
                PhotonNetwork.LocalPlayer.SetCustomProperties(ReadyCasting);
                break;
            case 1:
                PhotonNetwork.LocalPlayer.SetCustomProperties(ReadyCasting1);
                break;
            case 2:
                PhotonNetwork.LocalPlayer.SetCustomProperties(ReadyCasting);
                break;
            case 3:
                PhotonNetwork.LocalPlayer.SetCustomProperties(ReadyCasting2);
                break;
            case 4:
                PhotonNetwork.LocalPlayer.SetCustomProperties(ReadyCasting3);
                break;
            default:
                PhotonNetwork.LocalPlayer.SetCustomProperties(ReadyCasting);
                break;
        }
    }

    #region Ready Key
    string SetPepperoni = "SetPepperoni";
    string SetOlive = "SetOlive";

    string ReadyPlayCasting = "ReadyPlayCasting";
    string ReadyPlayCasting1 = "ReadyPlayCasting1";
    string ReadyPlayCasting2 = "ReadyPlayCasting2";
    string ReadyPlayCasting3 = "ReadyPlayCasting3";

    string ReadyPlayAttack1 = "ReadyPlayAttack1";
    string ReadyPlayAttack2 = "ReadyPlayAttack2";
    string ReadyPlayAttack3First = "ReadyPlayAttack3First";
    string ReadyPlayAttack3Second = "ReadyPlayAttack3Second";

    public ExitGames.Client.Photon.Hashtable SetPepperoniPos => new() { [SetPepperoni] = true };
    public ExitGames.Client.Photon.Hashtable SetOlivePos => new() { [SetOlive] = true };

    public ExitGames.Client.Photon.Hashtable ReadyCasting => new() { [ReadyPlayCasting] = true };
    public ExitGames.Client.Photon.Hashtable ReadyCasting1 => new() { [ReadyPlayCasting1] = true };
    public ExitGames.Client.Photon.Hashtable ReadyCasting2 => new() { [ReadyPlayCasting2] = true };
    public ExitGames.Client.Photon.Hashtable ReadyCasting3 => new() { [ReadyPlayCasting3] = true };

    public ExitGames.Client.Photon.Hashtable ReadyAttack1 => new() { [ReadyPlayAttack1] = true };
    public ExitGames.Client.Photon.Hashtable ReadyAttack2 => new() { [ReadyPlayAttack2] = true };
    public ExitGames.Client.Photon.Hashtable ReadyAttack3First => new() { [ReadyPlayAttack3First] = true };
    public ExitGames.Client.Photon.Hashtable ReadyAttack3Second => new() { [ReadyPlayAttack3Second] = true };
    #endregion

    UniTask GetAttackPattern(int order)
    {
        int delay = (int)(1.5f * 1000);
        return order switch
        {
            0 => AttackPattern1(delay),
            1 => AttackPattern2(delay),
            2 => AttackPattern3(delay),
            _ => default,
        };
    }
    async UniTask AttackPattern1(int delay)
    {
        maxStep = 7;
        //print("1: 토마토 페이스트");
        await SingleAttack(PizzaIngredient.TomatoPaste);

        //print("2: 치즈▶치즈");
        await ComboAttack(PizzaIngredient.CheeseL);

        //print("3: 페퍼로니▶올리브");
        await ComboAttack(PizzaIngredient.Olive);

        //print("4: 소스▶소스");
        await ComboAttack(PizzaIngredient.SauceWhite);
    }
    async UniTask AttackPattern2(int delay)
    {
        maxStep = 10;
        //print("1: 토마토 페이스트");
        await SingleAttack(PizzaIngredient.TomatoPaste);

        //print("2: 피망▶버섯");
        await ComboAttack(PizzaIngredient.BellPepper);

        //print("3: 옥수수");
        await SingleAttack(PizzaIngredient.Corn);

        //print("4: 피망/버섯+페페로니/올리브");
        await SingleSimulAttack(RandomAttack(PizzaIngredient.BellPepper), RandomAttack(PizzaIngredient.Olive));

        //print("5: 치즈+소스▶치즈+소스");
        await ComboSimulAttack(PizzaIngredient.CheeseL, PizzaIngredient.SauceWhite, delay, AddCorn.None);

        //print("6: 옥수수+페페로니/올리브");
        await SingleSimulAttack(PizzaIngredient.Corn, RandomAttack(PizzaIngredient.Olive));

        //print("7: 치즈+피망▶옥수수+치즈+버섯");
        await ComboSimulAttack(PizzaIngredient.CheeseL, PizzaIngredient.BellPepper, delay, AddCorn.Tail);
    }
    async UniTask AttackPattern3(int delay)
    {
        maxStep = 16;
        //print("1: 치즈+소스▶치즈+소스");
        await ComboSimulAttack(PizzaIngredient.CheeseL, PizzaIngredient.SauceWhite, 0, AddCorn.None);
        
        //print("2: 떡갈비");
        await SingleAttack(PizzaIngredient.RibPattie);

        //print("3: 옥수수");
        await SingleAttack(PizzaIngredient.Corn);

        //print("4: 피망/버섯+페페로니/올리브");
        await SingleSimulAttack(RandomAttack(PizzaIngredient.BellPepper), RandomAttack(PizzaIngredient.Olive));

        //print("5: 옥수수+피망/버섯");
        await SingleSimulAttack(PizzaIngredient.Corn, RandomAttack(PizzaIngredient.BellPepper));

        //print("6: 피망+소스▶버섯+소스");
        await ComboSimulAttack(PizzaIngredient.BellPepper, PizzaIngredient.SauceWhite, 0, AddCorn.None);

        //print("7: 옥수수+치즈+피망▶치즈+버섯");
        await ComboSimulAttack(PizzaIngredient.CheeseL, PizzaIngredient.BellPepper, 0, AddCorn.Head);

        //print("8: 옥수수+피망+페퍼로니▶옥수수+버섯+올리브"); //옥수수+페퍼로니+피망▶옥수수+버섯
        await ComboSimulAttack(PizzaIngredient.BellPepper, PizzaIngredient.Olive, delay, AddCorn.Both);

        //print("9: 옥수수+치즈+페퍼로니▶치즈+올리브"); // 옥수수+치즈▶치즈+페퍼로니/올리브
        await ComboSimulAttack(PizzaIngredient.CheeseL, PizzaIngredient.Olive, 0, AddCorn.Head);

        //print("10: 피망+소스▶버섯+소스");
        await ComboSimulAttack(PizzaIngredient.BellPepper, PizzaIngredient.SauceWhite, 0, AddCorn.None);
    }

    async UniTask SingleAttack(PizzaIngredient type)
    {
        await Casting(type, false);
        await Attack(type);
        curStep++;
        uiGame.SetProgress(curStep / maxStep);
    }
    async UniTask ComboAttack(PizzaIngredient type)
    {
        #region Set Attack Order
        int rand = Random.Range(0, 2);
        PizzaIngredient attack1 = type + rand;
        PizzaIngredient attack2 = type - rand + 1;
        #endregion

        await SingleAttack(attack1);
        await SingleAttack(attack2);
    }
    async UniTask SingleSimulAttack(PizzaIngredient type1, PizzaIngredient type2)
    {
        await Casting(type1, type2, false);
        await SimulAttack(type1, type2);
    }
    async UniTask ComboSimulAttack(PizzaIngredient type1, PizzaIngredient type2, int delay, AddCorn cornType)
    {
        #region Set Attack Order
        int rand1 = Random.Range(0, 2);
        int rand2 = Random.Range(0, 2);
        PizzaIngredient attack1 = type1 + rand1;
        PizzaIngredient attack2 = type2 + rand2;
        PizzaIngredient attack3 = type1 - rand1 + 1;
        PizzaIngredient attack4 = type2 - rand2 + 1;

        bool AddHead() => (cornType == AddCorn.Head || cornType == AddCorn.Both);
        bool AddTail() => (cornType == AddCorn.Tail || cornType == AddCorn.Both);
        #endregion

        #region Casting
                       await Casting(attack1, attack2, true);
        if (AddHead()) await Casting(PizzaIngredient.Corn, true);
                       await Casting(attack3, attack4, true);
        if (AddTail()) await Casting(PizzaIngredient.Corn, false);
        #endregion

        #region Attack
        if (AddHead())   _ = Attack(PizzaIngredient.Corn);
                       await SimulAttack(attack1, attack2);
        if (delay > 0) await UniTask.Delay(delay);
        if (AddTail())   _ = Attack(PizzaIngredient.Corn);
                       await SimulAttack(attack3, attack4);
        #endregion
    }
    #endregion

    #region Attack Type
    PizzaIngredient RandomAttack(PizzaIngredient type) => type + Random.Range(0, 2);
    async UniTask Casting(PizzaIngredient type, bool delay) => await chef.SetIngredient(type, delay, linked.Token);
    async UniTask Casting(PizzaIngredient type1, PizzaIngredient type2, bool delay) => await chef.SetIngredient(type1, type2, delay, linked.Token);
    async UniTask Attack(PizzaIngredient type) => await data.AttackList.Pop(type).SetCancellationToken(linked.Token).Play();
    async UniTask SimulAttack(PizzaIngredient type1, PizzaIngredient type2)
    {
        _ = Attack(type1);
        await Attack(type2);
        curStep++;
        uiGame.SetProgress(curStep / maxStep);
    }
    async UniTask BasicAttack(PizzaIngredient type)
    {
        print($"Attack [{type}]");

        await Attack(type);
        curStep++;
        uiGame.SetProgress(curStep / maxStep);
    }
    #endregion

    #region GameOver
    public void GameOver(bool isClear = false)
    {
        if (data.GameOver) return;
        data.GameOver = true;
        if (!isClear)
        {
            gm.OnFailed();
        }
        else
        {
            gm.SetResult();
        }
    }

    public void SetResult(bool isClear)
    {
        cancel.Cancel();
        PizzaBGMType type = (isClear)? PizzaBGMType.Clear: PizzaBGMType.Fail;
        data.PlayBGM(type, 0.25f);
        uiGame.CloseUI();
        ui.OpenUI<UIPizzaGameResult>().SetResult(isClear);
    }

    public void SetResult()
    {
        cancel.Cancel();
        data.PlayBGM(PizzaBGMType.Lobby, 0.25f);
        uiGame.CloseUI();
        ui.OpenUI<UIPizzaGameResult>().SetResult();
    }

    public void SetOutside() => stage.StageCollider.enabled = true;

    void ExitGameRPC() => gm.ExitGame();

    public void Bye()
    {
        if (data.GameOver) return; 
        data.GameOver = true;

        string result = "상대 플레이어가 게임 진행 중 퇴장하여 승리하였습니다.";
        ui.OpenUI<UIPopUpButton>()
            .SetMessage(message: result, title: "부전승")
            .AddConfirmAction(ExitGameRPC);
    }
    #endregion
}