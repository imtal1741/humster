using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBonus : MonoBehaviour
{

    public int state;
    public GameObject objectFirst;
    public GameObject objectSecond;

    bool wasTriggered;

    void OnTriggerEnter(Collider other)
    {
        if (wasTriggered)
            return;

        if (other.CompareTag("Player"))
        {
            wasTriggered = true;

            RunnerNumberRun runnerNumberRun = other.GetComponent<TriggerNumber>().runnerNumberRun;

            if (state < runnerNumberRun.nowStateMid)
            {
                objectFirst.SetActive(false);
                objectSecond.SetActive(true);

                if (state <= 4)
                {
                    runnerNumberRun.moveDownAddSpeed += 2f;
                }

                runnerNumberRun.audioEffects.PlaySoundPitch(runnerNumberRun.audioEffects.beat, state == 0);
            }
            else
            {
                runnerNumberRun.EndGame();
                // end game
            }


        }
    }



}
