using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CMF;

public class Health : MonoBehaviour
{
    [Header("Main")]
    public GameObject[] EnemyBullets;
    public int health;
    int healthStart;
    public Slider sliderHP;
    [HideInInspector] public bool death;
    [HideInInspector] public int countDeaths;
    public GameObject VisualModel;

    [Header("Sound")]
    public AudioClip impact;
    public AudioClip deathSound;
    public AudioSource audioSource;

    [Header("Timeline")]
    public GameObject deathTimeline;

    [Header("Links")]
    public YandexSDK yandexSDK;

    [HideInInspector] public PlayerRespawn playerRespawn;
    [HideInInspector] public AdvancedWalkerController advancedWalkerController;
    [HideInInspector] public Rigidbody rb;

    public SetupArena setupArena;

    void Start()
    {
        healthStart = health;

        playerRespawn = GetComponent<PlayerRespawn>();
        advancedWalkerController = GetComponent<AdvancedWalkerController>();
        rb = GetComponent<Rigidbody>();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (sliderHP)
            sliderHP.value = health * 100 / healthStart;

        if (audioSource)
            audioSource.PlayOneShot(impact, 0.25f);

        if (health <= 0 && !death)
        {
            death = true;
            ShowRestartMenu();
        }
    }

    public IEnumerator ShowEnemyBullet()
    {
        int index = Random.Range(0, EnemyBullets.Length);

        if (EnemyBullets[index].activeSelf == false)
        {
            EnemyBullets[index].SetActive(true);

            yield return new WaitForSeconds(2.5f);

            EnemyBullets[index].SetActive(false);
        }
    }

    void ShowRestartMenu()
    {
        if (VisualModel)
            VisualModel.SetActive(false);

        playerRespawn.Resume();
        playerRespawn.restartMenuUI.SetActive(true);


        playerRespawn.GameIsPaused = true;
        if (playerRespawn.isMobile)
        {
            foreach (GameObject obj in playerRespawn.MobileInputs)
            {
                obj.SetActive(false);
            }
        }
        Time.timeScale = 0.00000001f;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;



        if (setupArena)
        {
            setupArena.ClearArena(true);
        }
    }

    public void Restart()
    {
        playerRespawn.restartMenuUI.SetActive(false);

        #if !UNITY_EDITOR
            yandexSDK.ShowAdvert();
        #endif


        if (setupArena)
        {
            ResetPlayer();

            setupArena.Setup();
        }
        else
        {
            Respawn(false, "-");
        }

        playerRespawn.Resume();

        if (VisualModel)
            VisualModel.SetActive(true);
    }

    public void ResetPlayer()
    {
        health = healthStart;
        death = false;
        sliderHP.value = health * 100 / healthStart;

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        advancedWalkerController.momentum = Vector3.zero;



        setupArena.weaponChoice.ResetInventory();
        setupArena.weaponChoice.Weapons[1].SetActive(true);

        for (int i = 0; i < EnemyBullets.Length; i++)
        {
            EnemyBullets[i].SetActive(false);
        }
    }


    public void Respawn(bool reloadScene, string name)
    {
        StartCoroutine(RespawnIE(reloadScene, name));
    }

    IEnumerator RespawnIE(bool reloadScene, string name)
    {

        if (yandexSDK)
        {
            yandexSDK.TrySendAnalyticsEvent("Death",
                new Dictionary<string, object>
                {
                        { "Level", yandexSDK.nowScoreValue },
                        { "EnemyName", name },
                        { "LastCheckpoint", playerRespawn.lastCheckpoint },
                        { "LastObject", advancedWalkerController.lastTransform.name }
                }
            );
        }

        if (deathTimeline)
        {
            deathTimeline.SetActive(true);

            yield return new WaitForSeconds(2.3f);
        }

        if (reloadScene)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            countDeaths++;

            rb.interpolation = RigidbodyInterpolation.None;
            advancedWalkerController.smoothDampVel = Vector3.zero;
            advancedWalkerController.momentum = Vector3.zero;
            advancedWalkerController.savedVelocity = Vector3.zero;
            advancedWalkerController.savedMovementVelocity = Vector3.zero;
            transform.position = playerRespawn.startPos;
            transform.rotation = Quaternion.identity;

            yield return new WaitForSeconds(0.1f);

            rb.interpolation = RigidbodyInterpolation.Interpolate;


            if (deathTimeline)
                deathTimeline.SetActive(false);

            #if !UNITY_EDITOR
                yandexSDK.ShowAdvert();
            #endif

            death = false;
            health = healthStart;
        }

    }

    public void LoadCheckpoint(int x)
    {
        if (x <= 0)
            return;

        ArrayCheckpoint arrayCheckpoint = GetComponent<ArrayCheckpoint>();
        Transform teleportTr;
        if (PlayerPrefs.GetInt("Upgrade_1") == 1 && arrayCheckpoint.checkpoints.Count == x)
        {
            arrayCheckpoint.checkpoints[x - 1].Activate();
            teleportTr = arrayCheckpoint.FinishPoint;
            GetComponent<PlayerRespawn>().SetCheckpoint(teleportTr.position, "FinishPoint");
        }
        else
        {
            teleportTr = arrayCheckpoint.checkpoints[x - 1].transform;
        }

        StartCoroutine(TeleportIE(teleportTr));
    }
    public IEnumerator TeleportIE(Transform point)
    {
        // Try wait Initilization
        if (rb == null || advancedWalkerController == null)
            yield return new WaitForSecondsRealtime(0.5f);

        if (rb == null || advancedWalkerController == null)
            yield return new WaitForSecondsRealtime(0.5f);


        rb.interpolation = RigidbodyInterpolation.None;
        advancedWalkerController.momentum = Vector3.zero;
        transform.position = point.position;
        transform.rotation = point.rotation;

        yield return new WaitForSeconds(0.1f);

        rb.interpolation = RigidbodyInterpolation.Interpolate;

        FindObjectOfType<SetGetPrefsState>().TeleportState(transform);
    }





    void OnDestroy()
    {
        if (yandexSDK)
        {
            yandexSDK.TrySendAnalyticsEvent("Quit",
                new Dictionary<string, object>
                {
                    { "Level", yandexSDK.nowScoreValue },
                    { "countDeaths", countDeaths },
                    { "LastCheckpoint", playerRespawn.lastCheckpoint },
                    { "LastObject", advancedWalkerController.lastTransform.name }
                }
            );
        }
    }

}
