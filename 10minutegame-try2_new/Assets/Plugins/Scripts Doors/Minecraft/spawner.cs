using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class spawner : MonoBehaviour
{

    public GameObject goSpawn;
    public float fDifficulty = 40f;
    public float Score;
    float fSpawn;

    public Text ScoreText;

    public bool canStart;
	
	void Update () {

        if (canStart)
        {
            fSpawn += fDifficulty * Time.deltaTime;
            Score += 4f * Time.deltaTime * 10;
            fDifficulty += 4f * Time.deltaTime;
            fDifficulty = Mathf.Clamp(fDifficulty, 0, 400);

            ScoreText.text = ((int)Score).ToString();

            while (fSpawn > 0)
            {
                fSpawn -= 1;
                Vector3 v3Pos = transform.position + new Vector3(Random.value * 50 - 20f, 0, Random.value * 50 - 20f);
                Quaternion qRot = Quaternion.Euler(0, Random.value * 360f, Random.value * 30f);
                Vector3 v3Scale = new Vector3(Random.value + 0.3f, 10f, Random.value + 0.3f);
                GameObject goCreated = Instantiate(goSpawn, v3Pos, qRot);
                goCreated.transform.localScale = v3Scale;
            }
        }
	}
}
