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
    bool death;
    [HideInInspector] public int countDeaths;

    [Header("Sound")]
    public AudioClip impact;
    public AudioClip deathSound;
    public AudioSource audioSource;

    [Header("Timeline")]
    public GameObject deathTimeline;

    [Header("Links")]
    public YandexSDK yandexSDK;

    [HideInInspector] public PlayerRespawn playerRespawn;
    AdvancedWalkerController advancedWalkerController;
    Rigidbody rb;

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
            StartCoroutine(ShowRestartMenu());
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

    IEnumerator ShowRestartMenu()
    {

        if (!playerRespawn.BlackBorders_anim.GetBool("Close"))
        {
            playerRespawn.BlackBorders_anim.SetBool("Close", true);
            yield return new WaitForSeconds(0.6f);

            yandexSDK.ShowAdvert();



            playerRespawn.restartMenuUI.SetActive(true);


            playerRespawn.GameIsPaused = true;

            Time.timeScale = 0f;

            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;



            if (setupArena)
            {
                setupArena.ClearArena(true);
            }
        }

    }

    public void Restart()
    {
        playerRespawn.restartMenuUI.SetActive(false);


        if (setupArena)
        {
            ResetPlayer();

            setupArena.Setup();
        }

        playerRespawn.Resume();

        playerRespawn.BlackBorders_anim.SetBool("Close", false);
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
        Animator tempAnim = playerRespawn.BlackBorders_anim;

        if (!tempAnim.GetBool("Close"))
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

            if (audioSource && deathSound)
                audioSource.PlayOneShot(deathSound, 0.5f);

            tempAnim.SetBool("Close", true);
            yield return new WaitForSeconds(0.5f);

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
                advancedWalkerController.momentum = Vector3.zero;
                transform.position = playerRespawn.startPos;
                transform.rotation = Quaternion.identity;

                yield return new WaitForSeconds(0.1f);

                rb.interpolation = RigidbodyInterpolation.Interpolate;

                yandexSDK.ShowAdvert();

                if (deathTimeline)
                    deathTimeline.SetActive(false);

                tempAnim.SetBool("Close", false);
            }
        }

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
