using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
using System.Runtime.InteropServices;
using CMF;

public class PlayerRespawn : MonoBehaviour
{

    public Animator BlackBorders_anim;


    [HideInInspector] public Vector3 startPos;
    [HideInInspector] public string lastCheckpoint = "null";



    [HideInInspector] public bool GameIsPaused = false;
    [HideInInspector] public bool isMobile = false;

    [Header("Pause Menu")]
    public GameObject moreGamesUI;
    public GameObject difficultyMenuUI;
    public GameObject languageMenuUI;
    public GameObject pauseMenuUI;
    public GameObject restartMenuUI;

    [Header("Difficulty UI")]
    public GameObject[] Checks;

    [Header("Additional Difficulty UI")]
    public GameObject ChangeDiffUI;
    public GameObject HintDiffUI;
    public GameObject[] AdditionalChecks;
    bool doneClicked;
    bool AddDiffShowed;

    [Header("For Mobile")]
    public bool haveJoystick = false;
    public List<GameObject> MobileInputs = new List<GameObject>();
    public PostProcessLayer postProcessLayer;
    public GameObject postProcessVolume;

    [Header("Push objects if need show play button on start")]
    public GameObject resumeButton;
    public GameObject playButton;

    [Header("If Have Tutorial")]
    public GameObject tutorialObject;

    [Header("Sounds")]
    public AudioSource source;
    public AudioClip checkpointSound;
    public AudioClip completeSound;

    [HideInInspector] public Health health;
    [HideInInspector] public AdvancedWalkerController advancedWalkerController;

    float startSpeed;
    List<GameObject> sizeBlocks = new List<GameObject>();
    List<Vector3> startSizeBlocks = new List<Vector3>();

    void Start()
    {
        health = GetComponent<Health>();
        advancedWalkerController = GetComponent<AdvancedWalkerController>();
        startSpeed = advancedWalkerController.movementSpeed;

        startPos = transform.position;

        if (playButton)
        {
            Pause();
        }
        else
        {
            Resume();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && health.health > 0)
        {
            if (playButton.activeSelf) return;

            if (health.yandexSDK.nowAdShow) return;

            if (restartMenuUI)
            {
                if (restartMenuUI.activeSelf)
                {
                    return;
                }
            }

            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }


    public void UpdateJoystickControls(bool isMob)
    {
        if (haveJoystick == false)
            return;

        isMobile = isMob;

        if (isMobile)
        {
            if (postProcessLayer)
                postProcessLayer.enabled = false;
            if (postProcessVolume)
                postProcessVolume.SetActive(false);
        }
        else
        {
            if (postProcessLayer)
                postProcessLayer.enabled = true;
            if (postProcessVolume)
                postProcessVolume.SetActive(true);
        }

        foreach (GameObject obj in MobileInputs)
        {
            obj.SetActive(false);
        }
    }

    public void UpdateDifficulty(int difficulty)
    {

        //Show Difficult Menu
        if (ChangeDiffUI && AddDiffShowed == false)
        {
            AddDiffShowed = true;
            StartCoroutine(ShowDifficultMenu());
        }





        bool isFirstTime = false;
        float scaleFactor = 1f;


        for (int c = 0; c < Checks.Length; c++)
        {
            if (c == difficulty)
                Checks[c].SetActive(true);
            else
                Checks[c].SetActive(false);

            if (AdditionalChecks.Length > 0)
            {
                if (c == difficulty)
                    AdditionalChecks[c].SetActive(true);
                else
                    AdditionalChecks[c].SetActive(false);
            }
        }


        if (sizeBlocks.Count <= 0)
        {
            if (difficulty == 0)
            {
                return;
            }
            else
            {
                isFirstTime = true;
                sizeBlocks.AddRange(GameObject.FindGameObjectsWithTag("sizeBlock"));
            }
        }



        switch (difficulty)
        {
            case 0:
                advancedWalkerController.movementSpeed = startSpeed;
                scaleFactor = 1f;
                break;
            case 1:
                scaleFactor = 0.65f;
                advancedWalkerController.movementSpeed = startSpeed + 1;
                break;
            default:
                break;
        }

        for (int i = 0; i < sizeBlocks.Count; i++)
        {
            if (isFirstTime)
            {
                startSizeBlocks.Add(sizeBlocks[i].transform.localScale);
            }

            sizeBlocks[i].transform.localScale = new Vector3(startSizeBlocks[i].x * scaleFactor, sizeBlocks[i].transform.localScale.y, startSizeBlocks[i].z * scaleFactor);
        }



        health.yandexSDK.SaveDifficulty(difficulty);
    }



    public void Resume()
    {
        GameIsPaused = false;
        pauseMenuUI.SetActive(false);
        if (languageMenuUI)
            languageMenuUI.SetActive(false);
        if (difficultyMenuUI)
            difficultyMenuUI.SetActive(false);
        if (moreGamesUI)
            moreGamesUI.SetActive(false);

        if (isMobile)
        {
            foreach (GameObject obj in MobileInputs)
            {
                obj.SetActive(true);
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }


        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    public void Pause()
    {
        GameIsPaused = true;
        pauseMenuUI.SetActive(true);
        if (languageMenuUI)
            languageMenuUI.SetActive(true);
        if (difficultyMenuUI)
        {
            if (health.yandexSDK.savedRecord >= 9)
                difficultyMenuUI.SetActive(true);
            else
                difficultyMenuUI.SetActive(false);
        }
        if (moreGamesUI)
            moreGamesUI.SetActive(true);

        if (isMobile)
        {
            foreach (GameObject obj in MobileInputs)
            {
                obj.SetActive(false);
            }
        }

        Time.timeScale = 0.00000001f;
        AudioListener.pause = true;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void PauseSimple()
    {
        GameIsPaused = true;

        Time.timeScale = 0.00000001f;
        AudioListener.pause = true;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void Play()
    {
        playButton.SetActive(false);
        resumeButton.SetActive(true);

        if (tutorialObject)
            tutorialObject.SetActive(true);

        Resume();
    }
    public void ShowPlay()
    {
        playButton.SetActive(true);
        resumeButton.SetActive(false);
        Pause();
    }


    public void LoadMenu_IE()
    {
        StartCoroutine(LoadMenu());
    }

    IEnumerator LoadMenu()
    {
        if (!BlackBorders_anim.GetBool("Close"))
        {
            health.death = true;
            Time.timeScale = 1f;
            BlackBorders_anim.SetBool("Close", true);
            yield return new WaitForSeconds(0.6f);
            SceneManager.LoadScene("Menu");
        }

    }

    public void SetCheckpoint(Vector3 pos, string name)
    {
        startPos = pos;
        lastCheckpoint = name;

        if (checkpointSound)
            source.PlayOneShot(checkpointSound, 0.5f);
    }




    IEnumerator ShowDifficultMenu()
    {
        if (health.yandexSDK.savedRecord > 9)
            yield break;

        ChangeDiffUI.SetActive(true);

        doneClicked = false;
        while (!doneClicked)
        {
            yield return null;
        }


        Resume();
        ChangeDiffUI.SetActive(false);
        Pause();
        HintDiffUI.SetActive(true);
        difficultyMenuUI.SetActive(true);

        doneClicked = false;
        while (!doneClicked)
        {
            yield return null;
        }

        HintDiffUI.SetActive(false);

    }

    public void PlaySoundComplete()
    {
        if (completeSound)
            source.PlayOneShot(completeSound, 0.5f);
    }

    public void DoneClick()
    {
        doneClicked = true;
    }
}
