using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerNumber : MonoBehaviour
{

    public RunnerNumberRun runnerNumberRun;

    public int lineNumber;
    // 1 - Left, 2 - Mid, 3 - Right

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Interaction"))
        {// && runnerNumberRun.anim.GetCurrentAnimatorStateInfo(0).IsName("Merge")

            NumberSumTrigger tempNumberSum = other.gameObject.GetComponent<NumberSumTrigger>();
            NumberState tempNumberState = other.gameObject.GetComponent<NumberState>();
            int tempState = tempNumberState.state;

            if (runnerNumberRun.isAnimate && tempState >= 0)
            {
                return;
            }

            if (tempState >= 0 && tempState < 100)
            {
                switch (lineNumber)
                {
                    case 1:
                        if (runnerNumberRun.nowStateLeft != tempState)
                        {
                            return;
                        }
                        break;
                    case 2:
                        if (runnerNumberRun.nowStateMid != tempState)
                        {
                            return;
                        }
                        break;
                    case 3:
                        if (runnerNumberRun.nowStateRight != tempState)
                        {
                            return;
                        }
                        break;
                }
            }

            runnerNumberRun.UpdateState(lineNumber, tempState);

            if (tempState >= 0 && tempState < 100)
                tempNumberSum.DestroyNumber(tempNumberSum);
            else if (tempState == 100)
                other.collider.enabled = false;
            else if (tempState == 101)
                Destroy(other.gameObject);
            else
                tempNumberState.DestroyIt();
        }
        else if (other.gameObject.CompareTag("Finish"))
        {
            runnerNumberRun.Finish();
        }
    }


}
