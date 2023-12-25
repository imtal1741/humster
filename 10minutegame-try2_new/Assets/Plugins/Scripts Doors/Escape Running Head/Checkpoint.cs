using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    public TimerDestroy timerDestroy;

    public bool deactiveOnTrigger;


    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.CompareTag("Foot"))
        {
            PlayerRespawn playerRespawn = c.GetComponentInParent<PlayerRespawn>();

            if (!playerRespawn)
                return;

            playerRespawn.SetCheckpoint(transform.position, gameObject.name);

            if (timerDestroy)
                timerDestroy.ActivateDestroy();

            if (deactiveOnTrigger)
                gameObject.SetActive(false);
            else
                Destroy(gameObject);
        }
    }


}
