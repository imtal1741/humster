using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public class LevelSaveLoad : MonoBehaviour
{

    public RunnerNumberRun runnerNumberRun;
    public Transform Enviroments;
    public int nowLevel;
    [HideInInspector] public int upgradeLvl;
    [HideInInspector] public List<NumberSumTrigger> spawnedNumbers = new List<NumberSumTrigger>();
    public Color32 BlueColor;
    public Color32 GrayColor;

    public Saves saves;

    public GameObject[] m_numberPrefab;
    public GameObject[] m_obstaclePrefab;
    public GameObject[] m_otherPrefab;
    [HideInInspector] public Transform m_activeObject;
		

    public void CreateObjectNumber(int i)
    {
        Vector3 pos = Vector3.zero;
        pos = freePos(m_activeObject ? m_activeObject.localPosition : pos);

        m_activeObject = Instantiate(m_numberPrefab[i], pos + new Vector3(0, 0.13f, 0), Quaternion.Euler(10, 0, 0)).transform;
        m_activeObject.SetParent(Enviroments, false);
        InsertOneObject(m_activeObject, false);
    }
    public void CreateObjectObstacle(int i)
    {
        Vector3 pos = Vector3.zero;
        pos = freePos(m_activeObject ? m_activeObject.localPosition : pos);

        m_activeObject = Instantiate(m_obstaclePrefab[i], pos, Quaternion.Euler(0, 0, 0)).transform;
        m_activeObject.SetParent(Enviroments, false);
        InsertOneObject(m_activeObject, false);
    }
    public void CreateObjectOther(int i)
    {
        Vector3 pos = Vector3.zero;
        pos = freePos(m_activeObject ? m_activeObject.localPosition : pos);

        m_activeObject = Instantiate(m_otherPrefab[i], pos, Quaternion.Euler(0, 0, 0)).transform;
        m_activeObject.SetParent(Enviroments, false);
        InsertOneObject(m_activeObject, false);
    }
    Vector3 freePos(Vector3 pos)
    {
        pos = new Vector3(pos.x, 0, pos.z);
        while (saves.level[nowLevel - 1].OtherPos.Contains(pos) || saves.level[nowLevel - 1].ObstaclePos.Contains(pos) || saves.level[nowLevel - 1].NumberPos.Contains(pos + new Vector3(0, 0.13f, 0)))
        {
            pos += new Vector3(0, 0, 3.5f);
        }

        return pos;
    }
    public void MoveObject(Vector2 vec)
    {
        if (m_activeObject)
        {
            m_activeObject.localPosition += new Vector3(vec.x, 0, vec.y) * 3.5f;

            m_activeObject.localPosition = new Vector3(Mathf.Clamp(m_activeObject.localPosition.x, -3.5f, 3.5f), m_activeObject.localPosition.y, m_activeObject.localPosition.z);
            InsertOneObject(m_activeObject, true);
        }
    }
    public void RotateObject(int vec)
    {
        if (m_activeObject)
        {
            if (m_activeObject.GetComponent<NumberState>().state < 0)
            {
                m_activeObject.Rotate(new Vector3(0, 30 * vec, 0));
            }
        }
    }
    public void DeleteObject()
    {
        if (m_activeObject)
            GameObject.DestroyImmediate(m_activeObject.gameObject);
    }



    public void UpdateColors()
    {
        int stateNow = 0;
        if (runnerNumberRun.isMerge)
            stateNow = runnerNumberRun.nowStateMid;
        else
            stateNow = runnerNumberRun.nowStateLeft;

        for (int i = 0; i < spawnedNumbers.Count; i++)
        {
            Material mat = spawnedNumbers[i].GetComponent<Renderer>().material;
            int num = spawnedNumbers[i].numberState;

            if (stateNow == num || stateNow == num - 1 || stateNow == num + 1 || stateNow == num + 2)
                mat.SetColor("_Color", BlueColor);
            else
                mat.SetColor("_Color", GrayColor);
        }
    }

    [ContextMenu("Load")]
	public void LoadField()
    {
		StartCoroutine(LoadFieldIE());
	}
	
    IEnumerator LoadFieldIE()
    {
        Clear();
        spawnedNumbers.Clear();

        //saves = JsonUtility.FromJson<Saves>(File.ReadAllText(Application.streamingAssetsPath + "/levels.json"));

		
		string path = Path.Combine(Application.streamingAssetsPath, "levels.json");

		using(var request = UnityWebRequest.Get(path))
		{
			yield return request.SendWebRequest();

			if(request.result != UnityWebRequest.Result.Success)
			{
				yield break;
			}
		   
			saves = JsonUtility.FromJson<Saves>(request.downloadHandler.text);
		}
		
		
		
		
        for (int i = 0; i < saves.level[nowLevel - 1].OtherType.Count; i++)
        {
            Transform GameObj = Instantiate(m_otherPrefab[saves.level[nowLevel - 1].OtherType[i] - 100], saves.level[nowLevel - 1].OtherPos[i], Quaternion.Euler(0, 0, 0)).transform;
            GameObj.SetParent(Enviroments, false);
        }
        for (int i = 0; i < saves.level[nowLevel - 1].ObstacleType.Count; i++)
        {
            Transform GameObj = Instantiate(m_obstaclePrefab[saves.level[nowLevel - 1].ObstacleType[i] * -1 - 1], saves.level[nowLevel - 1].ObstaclePos[i], saves.level[nowLevel - 1].ObstacleRot[i]).transform;
            GameObj.SetParent(Enviroments, false);
        }
        for (int i = 0; i < saves.level[nowLevel - 1].NumberType.Count; i++)
        {
            Transform GameObj = Instantiate(m_numberPrefab[saves.level[nowLevel - 1].NumberType[i] + upgradeLvl], saves.level[nowLevel - 1].NumberPos[i], saves.level[nowLevel - 1].NumberRot[i]).transform;
            GameObj.SetParent(Enviroments, false);

            NumberSumTrigger GameObjNumberSumTrigger = GameObj.GetComponent<NumberSumTrigger>();
            GameObjNumberSumTrigger.numberState = GameObj.GetComponent<NumberState>().state;
            GameObjNumberSumTrigger.levelSaveLoad = this;

            spawnedNumbers.Add(GameObjNumberSumTrigger);
        }

        UpdateColors();
    }

    [ContextMenu("Save")]
    public void SaveField()
    {
        InsertObjects();
        File.WriteAllText(Application.streamingAssetsPath + "/levels.json", JsonUtility.ToJson(saves));
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        saves.level[nowLevel - 1].OtherType.Clear();
        saves.level[nowLevel - 1].OtherPos.Clear();

        saves.level[nowLevel - 1].ObstacleType.Clear();
        saves.level[nowLevel - 1].ObstaclePos.Clear();
        saves.level[nowLevel - 1].ObstacleRot.Clear();

        saves.level[nowLevel - 1].NumberType.Clear();
        saves.level[nowLevel - 1].NumberPos.Clear();
        saves.level[nowLevel - 1].NumberRot.Clear();

        while (Enviroments.childCount > 0)
        {
            DestroyImmediate(Enviroments.GetChild(0).gameObject);
        }
    }


    [ContextMenu("Insert Objects")]
    public void InsertObjects()
    {
        saves.level[nowLevel - 1].OtherType.Clear();
        saves.level[nowLevel - 1].OtherPos.Clear();

        saves.level[nowLevel - 1].ObstacleType.Clear();
        saves.level[nowLevel - 1].ObstaclePos.Clear();
        saves.level[nowLevel - 1].ObstacleRot.Clear();

        saves.level[nowLevel - 1].NumberType.Clear();
        saves.level[nowLevel - 1].NumberPos.Clear();
        saves.level[nowLevel - 1].NumberRot.Clear();

        for (int i = 0; i < Enviroments.childCount; i++)
        {
            Transform obj = Enviroments.GetChild(i);
            NumberState objNumberState = obj.GetComponent<NumberState>();

            if (objNumberState.state < 0)
            {
                saves.level[nowLevel - 1].ObstacleType.Add(objNumberState.state);
                saves.level[nowLevel - 1].ObstaclePos.Add(obj.localPosition);
                saves.level[nowLevel - 1].ObstacleRot.Add(obj.localRotation);
            }
            else
            {
                if (objNumberState.state >= 100)
                {
                    saves.level[nowLevel - 1].OtherType.Add(objNumberState.state);
                    saves.level[nowLevel - 1].OtherPos.Add(obj.localPosition);
                }
                else
                {
                    saves.level[nowLevel - 1].NumberType.Add(objNumberState.state);
                    saves.level[nowLevel - 1].NumberPos.Add(obj.localPosition);
                    saves.level[nowLevel - 1].NumberRot.Add(obj.localRotation);
                }
            }
        }
    }
    void InsertOneObject(Transform obj, bool justUpdateTr)
    {
        NumberState objNumberState = obj.GetComponent<NumberState>();

        if (objNumberState.state < 0)
        {
            if (justUpdateTr)
            {
                int i = saves.level[nowLevel - 1].ObstaclePos.Count - 1;
                saves.level[nowLevel - 1].ObstaclePos[i] = obj.localPosition;
                saves.level[nowLevel - 1].ObstacleRot[i] = obj.localRotation;
            }
            else
            {
                saves.level[nowLevel - 1].ObstacleType.Add(objNumberState.state);
                saves.level[nowLevel - 1].ObstaclePos.Add(obj.localPosition);
                saves.level[nowLevel - 1].ObstacleRot.Add(obj.localRotation);
            }
        }
        else
        {
            if (objNumberState.state >= 100)
            {
                if (justUpdateTr)
                {
                    int i = saves.level[nowLevel - 1].OtherPos.Count - 1;
                    saves.level[nowLevel - 1].OtherPos[i] = obj.localPosition;
                }
                else
                {
                    saves.level[nowLevel - 1].OtherType.Add(objNumberState.state);
                    saves.level[nowLevel - 1].OtherPos.Add(obj.localPosition);
                }
            }
            else
            {
                if (justUpdateTr)
                {
                    int i = saves.level[nowLevel - 1].NumberPos.Count - 1;
                    saves.level[nowLevel - 1].NumberPos[i] = obj.localPosition;
                    saves.level[nowLevel - 1].NumberRot[i] = obj.localRotation;
                }
                else
                {
                    saves.level[nowLevel - 1].NumberType.Add(objNumberState.state);
                    saves.level[nowLevel - 1].NumberPos.Add(obj.localPosition);
                    saves.level[nowLevel - 1].NumberRot.Add(obj.localRotation);
                }
            }
        }
    }

    public void Repair()
    {
        for (int i = 0; i < saves.level.Count; i++)
        {
            for (int p = saves.level[i].NumberType.Count - 1; p >= 0; p--)
            {
                if (saves.level[i].NumberType[p] == 29)
                {
                    saves.level[i].OtherType.Add(101);
                    saves.level[i].OtherPos.Add(saves.level[i].NumberPos[p]);

                    saves.level[i].NumberType.RemoveAt(p);
                    saves.level[i].NumberPos.RemoveAt(p);
                    saves.level[i].NumberRot.RemoveAt(p);
                }
            }
        }
    }


    [System.Serializable]
    public class Saves
    {
        public List<Level> level = new List<Level>();
    }

    [System.Serializable]
    public class Level
    {
        public List<int> OtherType = new List<int>();
        public List<Vector3> OtherPos = new List<Vector3>();

        public List<int> ObstacleType = new List<int>();
        public List<Vector3> ObstaclePos = new List<Vector3>();
        public List<Quaternion> ObstacleRot = new List<Quaternion>();

        public List<int> NumberType = new List<int>();
        public List<Vector3> NumberPos = new List<Vector3>();
        public List<Quaternion> NumberRot = new List<Quaternion>();
    }




}