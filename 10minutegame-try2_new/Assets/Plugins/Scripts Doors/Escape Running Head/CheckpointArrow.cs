using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointArrow : MonoBehaviour
{

    public ArrayCheckpoint arrayCheckpoint;
    public Renderer objectSwitchColor;
    public Renderer objectSwitchPicture;

    public bool used;

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.CompareTag("Player"))
        {
            if (used)
                return;


            PlayerRespawn playerRespawn = c.gameObject.GetComponent<PlayerRespawn>();
            playerRespawn.SetCheckpoint(transform.position, gameObject.name);


            Activate();
        }
    }

    public void Activate()
    {
        used = true;

        int index = arrayCheckpoint.checkpoints.IndexOf(this);

        for (int i = arrayCheckpoint.nowCheckpoints; i <= index; i++)
        {
            arrayCheckpoint.checkpoints[i].used = true;
            arrayCheckpoint.checkpoints[i].gameObject.SetActive(false);
            arrayCheckpoint.checkpoints[i].objectSwitchColor.material.SetColor("_Color", Color.green);
            arrayCheckpoint.checkpoints[i].objectSwitchPicture.material = arrayCheckpoint.newPicture;
            arrayCheckpoint.nowCheckpoints++;
        }

        objectSwitchColor.material.SetColor("_Color", Color.green);
        objectSwitchPicture.material = arrayCheckpoint.newPicture;
        arrayCheckpoint.UpdateArrow(true);

        gameObject.SetActive(false);

    }


}
