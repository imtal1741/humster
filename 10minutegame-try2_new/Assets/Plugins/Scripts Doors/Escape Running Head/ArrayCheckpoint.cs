using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ArrayCheckpoint : MonoBehaviour
{

    public List<CheckpointArrow> checkpoints = new List<CheckpointArrow>();
    public Transform FinishPoint;
    public Material oldPicture;
    public Material newPicture;
    public Transform Arrow;
    public GameObject StartCheckpoint;

    public Slider progressBar;
    public TextMeshProUGUI progressText;

    public YandexSDK yandexSDK;

    [HideInInspector] public int nowCheckpoints;
    int maxCheckpoints;

    void Awake()
    {
        ResetCheckpoints();
    }

    public void UpdateArrow(bool withSave)
    {

        if (nowCheckpoints < maxCheckpoints)
            Arrow.DOMove(checkpoints[nowCheckpoints].transform.position, 1f);
        else
            Arrow.gameObject.SetActive(false);


        SetProgressBar();

        if (withSave)
        {
            if (PlayerPrefs.GetInt("Upgrade_1") == 0)
                if (PlayerPrefs.GetInt("Score") != nowCheckpoints)
                    Debug.Log(nowCheckpoints);
            PlayerPrefs.SetInt("Score", nowCheckpoints);
            PlayerPrefs.Save();
        }

        yandexSDK.ShowAdvertTimer();
    }

    public void ResetCheckpoints()
    {
        StartCheckpoint.SetActive(true);

        for (int i = 0; i < maxCheckpoints; i++)
        {
            checkpoints[i].used = false;
            checkpoints[i].gameObject.SetActive(true);
            checkpoints[i].objectSwitchColor.material.SetColor("_Color", Color.red);
            checkpoints[i].objectSwitchPicture.material = oldPicture;
        }

        nowCheckpoints = 0;
        SetProgressBar();

        Arrow.gameObject.SetActive(true);
        Arrow.position = checkpoints[0].transform.position;
    }

    void SetProgressBar()
    {
        if (maxCheckpoints == 0)
        {
            maxCheckpoints = checkpoints.Count;
            progressBar.maxValue = maxCheckpoints;
        }

        int x = (int)(nowCheckpoints / (maxCheckpoints / 10f)) + 1;

        if (x > 10)
            x = 10;

        string smileType = " <sprite name=\"smiles_" + x + "\"> ";


        progressBar.value = nowCheckpoints;
        progressText.text = smileType + nowCheckpoints.ToString() + "/" + maxCheckpoints + " (" + Mathf.Floor(100 * nowCheckpoints / maxCheckpoints).ToString() + "%)" + smileType;

    }

}
