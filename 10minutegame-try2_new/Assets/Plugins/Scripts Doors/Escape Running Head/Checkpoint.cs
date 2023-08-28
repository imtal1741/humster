using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    public TimerDestroy timerDestroy;


    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.CompareTag("Player"))
        {
            PlayerRespawn playerRespawn = c.gameObject.GetComponent<PlayerRespawn>();

            playerRespawn.SetCheckpoint(transform.position, gameObject.name);

            if (timerDestroy)
                timerDestroy.ActivateDestroy();
        }

        Destroy(gameObject);
    }


}
