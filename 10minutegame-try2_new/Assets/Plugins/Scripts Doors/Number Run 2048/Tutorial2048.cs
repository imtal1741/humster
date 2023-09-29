using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial2048 : MonoBehaviour
{

    public RunnerNumberRun runnerNumberRun;
    public bool needMergeState;

    bool isTriggeredEnter;
    bool isTriggeredExit;

    void OnCollisionEnter(Collision other)
    {
        if (isTriggeredEnter)
            return;

        if (other.gameObject.CompareTag("Player"))
        {
            if (runnerNumberRun.isMerge != needMergeState)
            {
                if (runnerNumberRun.isAnimate)
                {
                    if (needMergeState && runnerNumberRun.anim.GetCurrentAnimatorStateInfo(0).IsName("Merge"))
                        return;

                    if (!needMergeState && runnerNumberRun.anim.GetCurrentAnimatorStateInfo(0).IsName("Split"))
                        return;
                }

                isTriggeredEnter = true;
                runnerNumberRun.EnableTutorial();
            }
        }

    }

    void OnCollisionExit(Collision other)
    {
        if (isTriggeredExit)
            return;

        if (other.gameObject.CompareTag("Player"))
        {
            isTriggeredExit = true;
            runnerNumberRun.DisableTutorial();
            Destroy(gameObject);
        }

    }

}
