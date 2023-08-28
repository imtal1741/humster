using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using TMPro;

public class CasinoSlot : MonoBehaviour
{
    [Header("Links")]
    public RunnerNumberRun runnerNumberRun;

    [Header("Lists")]
    public RectTransform list_1;
    public RectTransform list_2;
    public RectTransform list_3;

    [Header("Win Menu")]
    public GameObject winBlock;
    public GameObject confetti;
    public TextMeshProUGUI rewardText;
    int reward;


    [Header("Settings")]
    public int offsetValue;
    public int countObjects;
    public float speed;
    public int[] rewardTable;



    Vector3 winCell;
    int winNum;

    Sequence Sequence_1;
    Sequence Sequence_2;
    Sequence Sequence_3;

    bool isSpin;

    public void Spin()
    {
        if (isSpin)
            return;

        if (runnerNumberRun.spins <= 0)
            return;


        StartCoroutine(SpinIE());
    }

    IEnumerator SpinIE()
    {
        isSpin = true;

        runnerNumberRun.spins--;
        runnerNumberRun.spinsText.text = runnerNumberRun.spins.ToString();
        runnerNumberRun.Save();

        int x = Random.Range(0, 101);

        // 1 = 200k
        // 5 = 100k
        // 3 = 20k
        // 4 = 5k
        // 6 = 1k
        // 2 = 100


        switch (x)
        {
            case int n when (n <= 3):
                winNum = 1;
                winCell = new Vector3(winNum, winNum, winNum);
                break;
            case int n when (n > 3 && n <= 10):
                winNum = 5;
                winCell = new Vector3(winNum, winNum, winNum);
                break;
            case int n when (n > 10 && n <= 33):
                winNum = 3;
                winCell = new Vector3(winNum, winNum, winNum);
                break;
            case int n when (n > 33 && n <= 63):
                winNum = 4;
                winCell = new Vector3(winNum, winNum, winNum);
                break;
            case int n when (n > 63 && n <= 73):
                winNum = 6;
                winCell = new Vector3(winNum, winNum, winNum);
                break;
            case int n when (n > 73 && n <= 76):
                winNum = 2;
                winCell = new Vector3(winNum, winNum, winNum);
                break;
            default:
                int tempCell = Random.Range(1, countObjects + 1);
                winNum = 0;
                winCell = new Vector3(tempCell, tempCell, RandomFromRangeWithExceptions(1, countObjects, tempCell));
                break;
        }


        SpinOne(Sequence_1, list_1, (int)winCell.x);

        yield return new WaitForSeconds(0.15f);

        SpinOne(Sequence_2, list_2, (int)winCell.y);

        yield return new WaitForSeconds(0.15f);

        SpinOne(Sequence_3, list_3, (int)winCell.z);

        yield return new WaitForSeconds(speed + (speed * 0.75f) * 3);

        if (winNum > 0)
        {
            winBlock.SetActive(true);
            confetti.SetActive(true);
            reward = rewardTable[winNum];
            rewardText.text = reward.ToString() + "<sprite name=\"money\">";

            runnerNumberRun.audioEffects.PlaySound(runnerNumberRun.audioEffects.win);
        }

        isSpin = false;
    }

    public void GetReward(bool withBonus)
    {
        int bonusMult = 1;
        if (withBonus)
        {
            bonusMult = 2;
            //advert
        }

        runnerNumberRun.balance += reward * bonusMult;
        runnerNumberRun.Save();
        runnerNumberRun.UpdateBalance();
        runnerNumberRun.balanceText_2.transform.DOPunchScale(new Vector3(0.35f, 0.35f, 0.35f), 0.25f, 1, 0);

        winBlock.SetActive(false);
        confetti.SetActive(false);
    }
    public void AddSpins(int count)
    {
        runnerNumberRun.spins += count;
        runnerNumberRun.Save();
        runnerNumberRun.UpdateBalance();
    }


    void SpinOne(Sequence mySequence, RectTransform myList, int cell)
    {
        mySequence.Kill();
        mySequence = DOTween.Sequence();

        mySequence.Append(myList.DOAnchorPosY(offsetValue * countObjects, speed * 0.75f).SetEase(Ease.InSine));

        mySequence.AppendCallback(() =>
            myList.anchoredPosition = new Vector2(myList.anchoredPosition.x, 0));

        for (int i = 0; i < 2; i++)
        {
            mySequence.Append(myList.DOAnchorPosY(offsetValue * countObjects, speed * 0.75f).SetEase(Ease.Linear));

            mySequence.AppendCallback(() =>
                myList.anchoredPosition = new Vector2(myList.anchoredPosition.x, 0));
        }

        mySequence.Append(myList.DOAnchorPosY(offsetValue * (cell - 1), speed).SetEase(Ease.OutSine));
    }



    private int RandomFromRangeWithExceptions(int rangeMin, int rangeMax, int exclude)
    {
        int result = 0;

        if (exclude == rangeMax)
            result = rangeMax - 1;
        else if (exclude == rangeMin)
            result = rangeMin + 1;
        else
            result = exclude + 1;

        return result;
    }
}
