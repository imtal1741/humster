using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishTrigger : MonoBehaviour
{

    public float delay;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerRespawn playerRespawn = other.gameObject.GetComponent<PlayerRespawn>();
            YandexSDK yandexSDK = playerRespawn.health.yandexSDK;
            StartCoroutine(NextLevel(playerRespawn, yandexSDK));
        }
    }



    IEnumerator NextLevel(PlayerRespawn playerRespawn, YandexSDK yandexSDK)
    {
        yield return new WaitForSeconds(delay);

        if (int.Parse(SceneManager.GetActiveScene().name) > yandexSDK.savedRecord)
        {
            yandexSDK.Save(int.Parse(SceneManager.GetActiveScene().name));
        }

        playerRespawn.PlaySoundComplete();
        yield return new WaitForSeconds(0.1f);

        playerRespawn.BlackBorders_anim.SetBool("Close", true);
        yield return new WaitForSeconds(0.6f);

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (SceneManager.sceneCountInBuildSettings > nextSceneIndex)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            SceneManager.LoadScene("Menu");
        }

    }


}
