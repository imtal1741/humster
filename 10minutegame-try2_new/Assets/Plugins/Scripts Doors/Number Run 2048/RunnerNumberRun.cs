using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using EZCameraShake;
using TMPro;
using DG.Tweening;
using Lean.Localization;

public class RunnerNumberRun : MonoBehaviour
{
    int level;

    [Header("Main Links")]
    public Transform mainPlayer;
    public Animator anim;
    public AudioEffects audioEffects;
    public CoinsManager coinsManager;
    public LevelSaveLoad levelSaveLoad;
    public YandexSDK yandexSDK;

    [Header("Tutorial")]
    public GameObject tutorialObject;
    public GameObject tutorialUI;

    [Header("Effects")]
    public GameObject blueParticlePrefab;
    public GameObject redParticlePrefab;
    public GameObject confetti;

    [Header("End Text")]
    public TextMeshProUGUI rewardText;
    public TextMeshProUGUI multText;

    [Header("Balance")]
    public TextMeshProUGUI balanceText;
    public TextMeshProUGUI balanceText_2;
    public TextMeshProUGUI spinsText;
    public LeanToken leanTokenLevel;
    [HideInInspector] public int balance;
    [HideInInspector] public int spins;

    [Header("Casino")]
    public GameObject casinoButton;

    [Header("Upgrade 1")]
    public Button upgradeButton;
    public TextMeshProUGUI nowStateText;
    public TextMeshProUGUI nextStateText;
    public TextMeshProUGUI upg_1_priceText;
    int upg_1;
    int upg_1_price;
    public GameObject TextMaxBefore;
    public GameObject TextMaxAfter;

    [Header("Upgrade 2")]
    public Button upgradeButton2;
    public TextMeshProUGUI nowMultText;
    public TextMeshProUGUI nextMultText;
    public TextMeshProUGUI upg_2_priceText;
    float upg_2 = 1;
    int upg_2_price;

    [Header("Numbers")]
    public List<GameObject> LeftNum = new List<GameObject>();
    public List<GameObject> MidNum = new List<GameObject>();
    public List<GameObject> RightNum = new List<GameObject>();


    public int nowStateMid = 0;
    public int nowStateLeft = 0;
    public int nowStateRight = 0;
    [HideInInspector] public bool isMerge = true;
    public bool isAnimate = false;
    Sequence MidSequence;
    Sequence LeftSequence;
    Sequence RightSequence;

    [Header("Animations")]
    public Transform leftTween;
    public Transform midTween;
    public Transform rightTween;
    public Transform leftScale;
    public Transform midScale;
    public Transform rightScale;

    [Header("Colliders")]
    public BoxCollider leftCollider;
    public BoxCollider midCollider;
    public BoxCollider rightCollider;


    [Header("Camera")]
    public Transform camera;
    Vector3 camOffset;
    Vector3 selfOffset;
    bool canMoveForward;
    [HideInInspector] public bool canMoveDown;
    [HideInInspector] public float moveDownAddSpeed;
    bool canTap;
    bool isFinish;
    bool isDead;

    [Header("Finish Glasses")]
    public List<TriggerBonus> BreakableObjects = new List<TriggerBonus>();
    public float maxYPos;

    [Header("Other")]
    public GameObject TapToStartText;


    void Start()
    {
        isMerge = true;
        canTap = true;
        isDead = false;

        leftCollider.enabled = false;
        midCollider.enabled = true;
        rightCollider.enabled = false;


        camOffset = camera.localPosition;
        selfOffset = mainPlayer.localPosition;

        //Application.targetFrameRate = 30;
    }

    void LateUpdate()
    {
        if (canMoveForward)
        {
            mainPlayer.Translate(Vector3.forward * 5.5f * Time.deltaTime, Space.World);
        }
        if (canMoveDown)
        {
            mainPlayer.Translate((Vector3.down * 12.5f - new Vector3(0, moveDownAddSpeed, 0)) * Time.deltaTime, Space.World);

            mainPlayer.transform.position = new Vector3(mainPlayer.transform.position.x, mainPlayer.position.y > maxYPos ? mainPlayer.position.y : maxYPos, mainPlayer.transform.position.z);
        }


        camera.position = mainPlayer.localPosition - selfOffset + camOffset + transform.position;


        if (isFinish == false)
            camera.position = new Vector3(camera.position.x, camOffset.y, camera.position.z);
    }


    public void UpdateBalance()
    {
        balanceText.text = balance.ToString() + "<sprite name=\"money\">";
        if (balanceText_2)
            balanceText_2.text = balance.ToString() + "<sprite name=\"money\">";
        spinsText.text = spins.ToString();

        UpdateFirstUpgrade();
        UpdateSecondUpgrade();
    }

    public void UpdateStat(int lvl, int bal, int upgrade_1, float upgrade_2, int _spins)
    {
        if (lvl < 1)
        {
            level = 1;
            balance = 500;
        }
        else
        {
            level = lvl;
            balance = bal;
        }

        if (level > levelSaveLoad.saves.level.Count)
        {
            int x = Random.Range(3, levelSaveLoad.saves.level.Count);
            while (x == level)
            {
                x = Random.Range(3, levelSaveLoad.saves.level.Count);
            }

            levelSaveLoad.nowLevel = x;
        }
        else
        {
            levelSaveLoad.nowLevel = level;
        }

        if (level == 1)
            tutorialObject.SetActive(true);
        else
            tutorialObject.SetActive(false);


        leanTokenLevel.SetValue(level);

        spins = _spins;
        upg_1 = upgrade_1;
        levelSaveLoad.upgradeLvl = upgrade_1;

        if (upgrade_2 < 1)
            upg_2 = 1;
        else
            upg_2 = upgrade_2;

        nowStateMid = upg_1;


        UpdateBalance();



        levelSaveLoad.LoadField();
    }


    public void BuyFirstUpgrade()
    {
        if (nowStateMid >= 12)
            return;

        if (balance < upg_1_price)
            return;

        balance -= upg_1_price;
        upg_1++;
        MidNum[nowStateMid].SetActive(false);
        nowStateMid = upg_1;
        levelSaveLoad.upgradeLvl = upg_1;
        levelSaveLoad.LoadField();
        Save();

        UpdateBalance();
    }
    public void BuySecondUpgrade()
    {
        if (balance < upg_2_price)
            return;

        balance -= upg_2_price;
        upg_2 = Mathf.Round((upg_2 + 0.1f) * 100f) / 100f;
        Save();

        UpdateBalance();
    }
    void UpdateFirstUpgrade()
    {
        if (nowStateMid >= 12)
        {
            upgradeButton.interactable = false;
            nowStateText.gameObject.SetActive(false);
            nextStateText.gameObject.SetActive(false);
            TextMaxBefore.SetActive(true);
            TextMaxAfter.SetActive(true);
        }
        else
        {
            upgradeButton.interactable = true;

            nowStateText.text = MidNum[nowStateMid].name;
            nextStateText.text = MidNum[nowStateMid + 1].name;

            nowStateText.gameObject.SetActive(true);
            nextStateText.gameObject.SetActive(true);
            TextMaxBefore.SetActive(false);
            TextMaxAfter.SetActive(false);
        }
        upg_1_price = Mathf.RoundToInt(4500 * Mathf.Pow(1.15f, upg_1));
        upg_1_priceText.text = upg_1_price.ToString();
        if (balance < upg_1_price)
        {
            upgradeButton.interactable = false;
        }

        MidNum[nowStateMid].SetActive(true);

        MidSequence.Kill();
        MidSequence = DOTween.Sequence();

        GameObject particle = Instantiate(blueParticlePrefab);
        particle.transform.parent = midScale;
        particle.transform.localPosition = new Vector3(0, 1.5f, -0.5f);

        midTween.localScale = Vector3.one;
        MidSequence.Append(midTween.DOPunchScale(new Vector3(0.35f, 0.35f, 0.35f), 0.25f, 1, 0));
    }
    void UpdateSecondUpgrade()
    {
        nowMultText.text = "x" + upg_2.ToString();
        nextMultText.text = "x" + (upg_2 + 0.1f).ToString("F1");
        upg_2_price = Mathf.RoundToInt(500 * Mathf.Pow(1.07f, (upg_2 - 1) / 0.1f));
        upg_2_priceText.text = upg_2_price.ToString();
        if (balance < upg_2_price)
        {
            upgradeButton2.interactable = false;
        }
        else
        {
            upgradeButton2.interactable = true;
        }
    }




    public void Finish()
    {
        if (isFinish == true)
            return;

        isFinish = true;
        canTap = false;


        int finishState = nowStateMid;
        int tempState = Mathf.CeilToInt(Mathf.Abs(nowStateLeft - nowStateRight) / 2) + 1;
        if (nowStateLeft < nowStateRight)
        {
            tempState += nowStateLeft;
        }
        else
        {
            tempState += nowStateRight;
        }
        int afterMergeState = tempState;



        if (isMerge == false)
        {
            if (isAnimate)
            {
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Split"))
                {
                    SplitMerge();
                    finishState = afterMergeState;
                }
            }
            else
            {
                SplitMerge();
                finishState = afterMergeState;
            }
        }

        anim.SetBool("Finish", true);

        for (int i = 0; i < BreakableObjects.Count; i++)
        {
            if (i >= 4)
            {
                if (i <= finishState)
                {
                    BreakableObjects[i].gameObject.SetActive(true);
                }
            }

            if (i == finishState)
                maxYPos = BreakableObjects[i].transform.position.y;
        }

    }

    public void TouchButton()
    {
        if (canTap == false)
            return;

        if (canMoveForward == false)
        {
            canMoveForward = true;

            if (upgradeButton.gameObject.activeSelf)
            {
                upgradeButton.gameObject.SetActive(false);
                upgradeButton2.gameObject.SetActive(false);
                casinoButton.SetActive(false);
                TapToStartText.SetActive(false);
            }
        }
        else
        {
            SplitMerge();
        }
    }

    // 1 - Left, 2 - Mid, 3 - Right
    public void UpdateState(int LineNum, int State)
    {
        if (State == 100)
        {
            StartCoroutine(JumpAnim());
            return;
        }
        State = State >= 0 ? 1 : -1;

        switch (LineNum)
        {
            case 1:
                LeftSequence.Kill();
                LeftSequence = DOTween.Sequence();

                if (State < 0)
                {
                    audioEffects.PlaySoundPitch(audioEffects.kill, true);

                    GameObject particle = Instantiate(redParticlePrefab);
                    particle.transform.parent = leftScale;
                    particle.transform.localPosition = new Vector3(0, 1.5f, -0.5f);

                    LeftSequence.Append(leftTween.DOPunchPosition(new Vector3(0.3f, 0f, 0f), 0.3f, 25, 0.5f));

                    if (nowStateLeft == 0)
                    {
                        leftScale.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
                        StartCoroutine(Dead());
                        break;
                    }
                }
                else
                {
                    audioEffects.PlaySoundPitch(audioEffects.pop, false);

                    GameObject particle = Instantiate(blueParticlePrefab);
                    particle.transform.parent = leftScale;
                    particle.transform.localPosition = new Vector3(0, 1.5f, -0.5f);

                    leftScale.localScale = Vector3.one;
                    LeftSequence.Append(leftScale.DOPunchScale(new Vector3(0.35f, 0.35f, 0.35f), 0.25f, 1, 0));
                }
                LeftNum[nowStateLeft].SetActive(false);
                nowStateLeft = Mathf.Clamp(nowStateLeft + State, 0, MidNum.Count - 1);
                LeftNum[nowStateLeft].SetActive(true);
                levelSaveLoad.UpdateColors();

                break;
            case 2:
                MidSequence.Kill();
                MidSequence = DOTween.Sequence();

                if (State < 0)
                {
                    audioEffects.PlaySoundPitch(audioEffects.kill, true);

                    GameObject particle = Instantiate(redParticlePrefab);
                    particle.transform.parent = midScale;
                    particle.transform.localPosition = new Vector3(0, 1.5f, -0.5f);

                    MidSequence.Append(midTween.DOPunchPosition(new Vector3(0.3f, 0f, 0f), 0.3f, 25, 0.5f));

                    if (nowStateMid == 0)
                    {
                        midScale.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
                        StartCoroutine(Dead());
                        break;
                    }
                }
                else
                {
                    audioEffects.PlaySoundPitch(audioEffects.pop, false);

                    GameObject particle = Instantiate(blueParticlePrefab);
                    particle.transform.parent = midScale;
                    particle.transform.localPosition = new Vector3(0, 1.5f, -0.5f);

                    midScale.localScale = Vector3.one;
                    MidSequence.Append(midScale.DOPunchScale(new Vector3(0.35f, 0.35f, 0.35f), 0.25f, 1, 0));
                }
                MidNum[nowStateMid].SetActive(false);
                nowStateMid = Mathf.Clamp(nowStateMid + State, 0, MidNum.Count);
                MidNum[nowStateMid].SetActive(true);
                levelSaveLoad.UpdateColors();

                break;
            case 3:
                RightSequence.Kill();
                RightSequence = DOTween.Sequence();

                if (State < 0)
                {
                    audioEffects.PlaySoundPitch(audioEffects.kill, true);

                    GameObject particle = Instantiate(redParticlePrefab);
                    particle.transform.parent = rightScale;
                    particle.transform.localPosition = new Vector3(0, 1.5f, -0.5f);

                    RightSequence.Append(rightTween.DOPunchPosition(new Vector3(0.3f, 0f, 0f), 0.3f, 25, 0.5f));

                    if (nowStateRight == 0)
                    {
                        rightScale.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
                        StartCoroutine(Dead());
                        break;
                    }
                }
                else
                {
                    audioEffects.PlaySoundPitch(audioEffects.pop, false);

                    GameObject particle = Instantiate(blueParticlePrefab);
                    particle.transform.parent = rightScale;
                    particle.transform.localPosition = new Vector3(0, 1.5f, -0.5f);

                    rightScale.localScale = Vector3.one;
                    RightSequence.Append(rightScale.DOPunchScale(new Vector3(0.35f, 0.35f, 0.35f), 0.25f, 1, 0));
                }
                RightNum[nowStateRight].SetActive(false);
                nowStateRight = Mathf.Clamp(nowStateRight + State, 0, MidNum.Count - 1);
                RightNum[nowStateRight].SetActive(true);
                levelSaveLoad.UpdateColors();

                break;
        }
    }

    void DoMerge()
    {
        if (isDead)
            return;

        isMerge = true;
        isAnimate = false;


        int tempState = Mathf.CeilToInt(Mathf.Abs(nowStateLeft - nowStateRight) / 2) + 1;

        if (nowStateLeft < nowStateRight)
        {
            tempState += nowStateLeft;
        }
        else
        {
            tempState += nowStateRight;
        }

        nowStateMid = tempState;
        levelSaveLoad.UpdateColors();


        MidNum[nowStateMid].SetActive(true);
        LeftNum[nowStateLeft].SetActive(false);
        RightNum[nowStateRight].SetActive(false);

        leftCollider.enabled = false;
        midCollider.enabled = true;
        rightCollider.enabled = false;
    }

    void SplitMerge()
    {
        if (nowStateMid == 0)
            return;

        if (isAnimate)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Split"))
                anim.CrossFade("Merge", 0f);
            else
                anim.CrossFade("Split", 0f);

            return;
        }

        if (isMerge)
        {
            isMerge = false;
            isAnimate = true;
            nowStateLeft = nowStateMid - 1;
            nowStateRight = nowStateMid - 1;

            MidNum[nowStateMid].SetActive(false);
            LeftNum[nowStateLeft].SetActive(true);
            RightNum[nowStateRight].SetActive(true);

            leftCollider.enabled = true;
            midCollider.enabled = false;
            rightCollider.enabled = true;

            anim.CrossFade("Split", 0f);
        }
        else
        {
            isAnimate = true;
            anim.CrossFade("Merge", 0f);
        }
    }




    public void ResetAnimate()
    {
        isAnimate = false;
        levelSaveLoad.UpdateColors();

        leftCollider.enabled = false;
        rightCollider.enabled = false;
        leftCollider.enabled = true;
        rightCollider.enabled = true;
    }
    IEnumerator JumpAnim()
    {
        anim.SetBool("Jump", true);
        yield return new WaitForSeconds(0.25f);
        anim.SetBool("Jump", false);
    }

    public void StartFinishAnim()
    {
        canMoveForward = false;
    }
    public void EndFinishAnim()
    {
        canMoveDown = true;
    }



    public void EndGame()
    {
        canMoveDown = false;
        confetti.SetActive(true);

        int reward = Mathf.RoundToInt((nowStateMid + 1) * upg_2 * 120);
        balance += reward;
        spins++;
        level++;
        Save();
        rewardText.text = "+" + reward.ToString();
        multText.text = "x" + upg_2.ToString();
        StartCoroutine(MoneyAnimIE(1f, true, reward));
        StartCoroutine(ReloadScene(3f));
    }
    IEnumerator Dead()
    {
        isDead = true;
        canTap = false;
        canMoveForward = false;
        //ShakeCamera
        CameraShaker.Instance.ShakeOnce(2f, 0.5f, 0.1f, 0.75f);

        yield return new WaitForSeconds(1.25f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    IEnumerator ReloadScene(float duration)
    {
        yield return new WaitForSeconds(duration);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }



    public void EnableTutorial()
    {
        tutorialUI.SetActive(true);
        Time.timeScale = 0.25f;
    }

    public void DisableTutorial()
    {
        Time.timeScale = 1f;
        tutorialUI.SetActive(false);
    }
    IEnumerator MoneyAnimIE(float duration, bool multTextActive, int reward)
    {
        yield return new WaitForSeconds(duration);

        rewardText.gameObject.SetActive(true);
        if (multTextActive)
            multText.gameObject.SetActive(true);
        coinsManager.AddCoinsSimple(mainPlayer, reward);
    }
    public void Save()
    {
        yandexSDK.SaveNumberRun(level, balance, upg_1, upg_2, spins);
    }
}
