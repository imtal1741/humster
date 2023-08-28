using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberSumTrigger : MonoBehaviour
{

    public int numberState;
    [HideInInspector] public LevelSaveLoad levelSaveLoad;
    [HideInInspector] public bool isTriggered;

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Interaction"))
        {
            NumberSumTrigger numberSumTrigger = other.gameObject.GetComponent<NumberSumTrigger>();
            if (isTriggered)
                return;

            if (numberSumTrigger)
            {
                if (numberSumTrigger.isTriggered)
                {
                    return;
                }
            }

            NumberState tempNumberState = other.gameObject.GetComponent<NumberState>();
            int tempState = tempNumberState.state;

            if (tempState == numberState || tempState == 101)
            {
                isTriggered = true;

                if (levelSaveLoad)
                {
                    Transform obj = Instantiate(levelSaveLoad.m_numberPrefab[numberState + 1], transform.localPosition, Quaternion.Euler(10, 0, 0)).transform;
                    obj.SetParent(levelSaveLoad.Enviroments, false);

                    NumberSumTrigger GameObjNumberSumTrigger = obj.GetComponent<NumberSumTrigger>();
                    GameObjNumberSumTrigger.numberState = obj.GetComponent<NumberState>().state;
                    GameObjNumberSumTrigger.levelSaveLoad = levelSaveLoad;

                    levelSaveLoad.spawnedNumbers.Add(GameObjNumberSumTrigger);
                }

                if (numberSumTrigger)
                    DestroyNumber(numberSumTrigger);
                else
                    Destroy(other.gameObject);
                DestroyNumber(this);
            }
            else if (tempState < 0)
            {
                isTriggered = true;




                tempNumberState.DestroyIt();

                if (numberState > 0)
                {
                    Transform obj = Instantiate(levelSaveLoad.m_numberPrefab[numberState - 1], transform.localPosition, Quaternion.Euler(10, 0, 0)).transform;
                    obj.SetParent(levelSaveLoad.Enviroments, false);

                    NumberSumTrigger GameObjNumberSumTrigger = obj.GetComponent<NumberSumTrigger>();
                    GameObjNumberSumTrigger.numberState = obj.GetComponent<NumberState>().state;
                    GameObjNumberSumTrigger.levelSaveLoad = levelSaveLoad;

                    levelSaveLoad.spawnedNumbers.Add(GameObjNumberSumTrigger);
                }

                DestroyNumber(this);
            }
        }

    }

    public void DestroyNumber(NumberSumTrigger obj)
    {
        levelSaveLoad.spawnedNumbers.Remove(obj);
        Destroy(obj.gameObject);
    }

}
