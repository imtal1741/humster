using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
using Lean.Localization;
using PixelCrushers.DialogueSystem;
using TMPro;
//using CrazyGames;


public class YandexSDK : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void ShowAdv();
	
	[DllImport("__Internal")]
    private static extern void GetLang();

    [DllImport("__Internal")]
    private static extern void GetDomainSite();

    [DllImport("__Internal")]
    private static extern void GetDeviceSDK();

    [DllImport("__Internal")]
    private static extern void SaveExtern(string date);

    [DllImport("__Internal")]
    private static extern void LoadExtern();

    [DllImport("__Internal")]
    private static extern void AdvWeapon();

    [Header("SetupArena")]
    public SetupArena setupArena;

    [Header("If have Levels Manager")]
    public UILevelsManager levelsManager;

    [Header("If have Record Text")]
    public LeanToken recordValue;

    [Header("If have MORE GAMES")]
    public OpenLink openLink;

    [HideInInspector] public bool isMobile;

    [HideInInspector] public int savedRecord;
    [HideInInspector] public int savedMoney;
    [HideInInspector] public int savedUpgrade_1;
    [HideInInspector] public float savedUpgrade_2;
    [HideInInspector] public int savedDifficulty;
    [HideInInspector] public int savedSpins;

    [Header("If have mobile. Update Joystick Controls || have ShowAdvert")]
    public PlayerRespawn playerRespawn;
    [Header("Health")]
    public Health health;

    [Header("need Health")]
    public bool haveCheckpointsSave;
    [Header("need PlayerRespawn")]
    public bool haveDifficulty;

    [Header("If have Record RunnerNumberRun")]
    public RunnerNumberRun runnerNumberRun;

    [Header("Analytics")]
    public bool enableAnalytics;
    public AnalyticsInit analyticsInit;

    [Header("Try Show Adv On Start")]
    public bool showAdvOnStart;
    public GameObject advertTimerGameObject;
    public TextMeshProUGUI advertTimer;
    bool advertTimerActive;


    private float nextAdv;
    [HideInInspector] public bool nowAdShow;


    public bool setNowLvlText;
    [HideInInspector] public int nowScoreValue;
	
	
	[Header("Language")]
	public LeanLocalization leanLocalization;
    public List<string> shortLang = new List<string>();
    public List<string> longName = new List<string>();


    public bool screenOrientationLandscape = true;


    // CrazyGames


    //public void Start()
    //{
    //    SetPlayerInfo();

    //    if (showAdvOnStart)
    //        ShowAdvert();
    //}

    //public void GetDevice(string _isMobile)
    //{
    //    if (_isMobile == "true")
    //        isMobile = true;
    //    else
    //        isMobile = false;
    //}

    //public void Save()
    //{
    //    PlayerPrefs.SetInt("Score", savedRecord);
    //    PlayerPrefs.Save();
    //}

    //public void SetPlayerInfo()
    //{
    //    if (setupArena)
    //    {
    //        savedRecord = PlayerPrefs.GetInt("Score");

    //        setupArena.level = savedRecord;
    //        setupArena.Setup();
    //    }
    //}

    //public void ShowAdvert()
    //{
    //    CrazyAds.Instance.beginAdBreak(AdSuccess, AdError);
    //}

    //public void ButtonAdvWeapon()
    //{
    //    CrazyAds.Instance.beginAdBreakRewarded(UnlockAdvWeapon, AdError);
    //}

    //public void UnlockAdvWeapon()
    //{
    //    setupArena.UnlockWeapon();
    //}

    //void AdSuccess()
    //{
    //    print("Ad has been displayed");
    //}

    //void AdError()
    //{
    //    print("Ad has not been displayed");
    //}


    // Yandex


    public void Start()
    {
        StartCoroutine(StartInit());
    }

    IEnumerator StartInit()
    {
        if (setNowLvlText)
            yield return new WaitForSecondsRealtime(0.25f);

        if (screenOrientationLandscape)
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        else
            Screen.orientation = ScreenOrientation.Portrait;


        //GetDeviceSDK(); // Get Device       нужно включить
        //GetDeviceSimple();

        //GetDomainSite(); //                 нужно включить

        SetPlayerInfo();
        //GetLang(); //                 нужно включить

        //LoadExtern();

        if (advertTimerGameObject)
        {
            advertTimerGameObject.SetActive(false);
            advertTimerActive = false;
        }

        if (showAdvOnStart)
            ShowAdvert();
    }


    void GetDeviceSimple()
    {
        string isMob = Application.isMobilePlatform ? "true" : "false";
        //string isMob = "true";

        GetDevice(isMob);
    }

    public void GetDevice(string data)
    {
        if (data == "true")
        {
            isMobile = true;
        }
        else
        {
            isMobile = false;
        }

        //if (isMobile)
        //    Application.targetFrameRate = 60;

        if (isMobile && openLink)
        {
            openLink.DisableIcons();
        }

        if (playerRespawn)
            playerRespawn.UpdateJoystickControls(isMobile);
    }

    public void GetDomain(string domainStr)
    {
        if (openLink)
            openLink.domainName = domainStr;
    }





    public void Save(int scoreNum)
    {
        PlayerPrefs.SetInt("Score", scoreNum);
        PlayerPrefs.Save();

        savedRecord = scoreNum;
    }
    public void SaveNumberRun(int lvl, int bal, int upgrade_1, float upgrade_2, int _spins)
    {
        PlayerPrefs.SetInt("Score", lvl);
        PlayerPrefs.SetInt("Money", bal);
        PlayerPrefs.SetInt("Upgrade_1", upgrade_1);
        PlayerPrefs.SetFloat("Upgrade_2", upgrade_2);
        PlayerPrefs.SetInt("Spins", _spins);

        PlayerPrefs.Save();
    }

    public void SaveWithCheck(int scoreNum)
    {
        if (scoreNum > savedRecord)
        {
            PlayerPrefs.SetInt("Score", scoreNum);
            PlayerPrefs.Save();

            savedRecord = scoreNum;

            if (recordValue)
            {
                recordValue.SetValue(scoreNum);
            }
        }

        nowScoreValue = scoreNum;
    }

    public void SaveDifficulty(int difficulty)
    {
        PlayerPrefs.SetInt("Difficulty", difficulty);
        PlayerPrefs.Save();
    }


    public void SetPlayerInfo()
    {
        savedRecord = PlayerPrefs.GetInt("Score");
        savedMoney = PlayerPrefs.GetInt("Money");
        savedUpgrade_1 = PlayerPrefs.GetInt("Upgrade_1");
        savedUpgrade_2 = PlayerPrefs.GetFloat("Upgrade_2");
        savedDifficulty = PlayerPrefs.GetInt("Difficulty");
        savedSpins = PlayerPrefs.GetInt("Spins");

        if (recordValue)
        {
            if (setNowLvlText)
            {
                StartCoroutine(SetLevelName());
            }
            else
            {
                recordValue.SetValue(savedRecord);
            }
        }
        if (playerRespawn && haveDifficulty)
            playerRespawn.UpdateDifficulty(savedDifficulty);

        if (health && haveCheckpointsSave)
            health.LoadCheckpoint(savedRecord);

        if (runnerNumberRun)
        {
            runnerNumberRun.UpdateStat(savedRecord, savedMoney, savedUpgrade_1, savedUpgrade_2, savedSpins);
        }

        if (levelsManager)
            levelsManager.UpdateLevels(savedRecord);

        if (setupArena)
        {
            setupArena.level = savedRecord;
            setupArena.Setup();
        }
    }
	
	
	
	public void SetLang(string lang)
	{
		if (leanLocalization)
		{
			if (shortLang.Contains(lang))
				leanLocalization.SetCurrentLanguage(longName[shortLang.IndexOf(lang)]);
		}
        DialogueManager.SetLanguage(lang);
        if (openLink)
            openLink.langName = lang;
    }



    void ShowAdvertSimple()
    {
        if (Time.time >= nextAdv)
        {
            nextAdv = Time.time + 90;
            if (playerRespawn)
                playerRespawn.Pause();

            ShowAdv();
        }
    }
    public void ShowAdvert()
    {
        if (advertTimerActive)
            return;

        ShowAdvertSimple();
    }
    public void ShowAdvertTimer()
    {
        if (advertTimerGameObject == null)
            return;

        if (advertTimerActive)
            return;

        if (nowAdShow)
            return;

        if (Time.time < nextAdv)
            return;

        advertTimerActive = true;

        StartCoroutine(StartAdvertTimer());
    }
    IEnumerator StartAdvertTimer()
    {
        if (playerRespawn)
            playerRespawn.PauseSimple();
        advertTimerGameObject.SetActive(true);
        advertTimer.text = "3";
        yield return new WaitForSecondsRealtime(0.3f);
        advertTimer.text = "3.";
        yield return new WaitForSecondsRealtime(0.3f);
        advertTimer.text = "3..";
        yield return new WaitForSecondsRealtime(0.3f);
        advertTimer.text = "2";
        yield return new WaitForSecondsRealtime(0.3f);
        advertTimer.text = "2.";
        yield return new WaitForSecondsRealtime(0.3f);
        advertTimer.text = "2..";
        yield return new WaitForSecondsRealtime(0.3f);
        advertTimer.text = "1";
        yield return new WaitForSecondsRealtime(0.3f);
        advertTimer.text = "1.";
        yield return new WaitForSecondsRealtime(0.3f);
        advertTimer.text = "1..";
        yield return new WaitForSecondsRealtime(0.3f);
        advertTimerGameObject.SetActive(false);
        ShowAdvertSimple();
        advertTimerActive = false;
    }

    public void ButtonAdvWeapon()
    {
        AdvWeapon();
    }

    public void UnlockAdvWeapon(int value)
    {
        if (setupArena)
            setupArena.UnlockWeapon();
    }

    IEnumerator SetLevelName()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        recordValue.SetValue(sceneName);
        recordValue.gameObject.SetActive(false);
        recordValue.gameObject.SetActive(true);


        yield return new WaitForSecondsRealtime(5f);
        nowScoreValue = int.Parse(sceneName);
        TrySendAnalyticsEvent("LevelStart",
                new Dictionary<string, object>
                {
                    { "Level", nowScoreValue }
                }
            );
    }



    public void SetListenerVolume(int x)
    {
        nowAdShow = x == 0 ? true : false;
        if (nowAdShow == false)
            advertTimerActive = false;
        AudioListener.volume = x;

        if (x > 0 && playerRespawn.GameIsPaused)
            return;

        Time.timeScale = x == 0 ? 0.00000001f : x;
    }



    public void TrySendAnalyticsEvent(string Name, Dictionary<string, object> Dict)
    {
        if (enableAnalytics)
        {
            analyticsInit.SendAnalyticsEvent(Name, Dict);
        }
    }


    // Сохранения через яндекс

    //public void Save()
    //{
    //    string jsonString = JsonUtility.ToJson(PlayerInfo);
    //    SaveExtern(jsonString);
    //}

    //public void SetPlayerInfo(string value)
    //{
    //    PlayerInfo = JsonUtility.FromJson<PlayerInfo>(value);
    //    savedRecord = PlayerInfo.Score;

    //    if (levelsManager)
    //        levelsManager.UpdateLevels(savedRecord);

    //    if (setupArena)
    //    {
    //        setupArena.level = savedRecord;
    //        setupArena.Setup();
    //    }
    //}
}
