using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EmojiFace : MonoBehaviour
{
    public AudioSource source;

    public Transform head;
    public Transform[] eyes;
    public Transform[] eyesBlack;
    public Transform[] eyebrows;

    Vector3 start_headRot;
    Vector3[] start_eyesScale;
    Vector3[] start_eyebrowsPos;
    Vector3[] start_eyebrowsRot;

    void Start()
    {
        start_headRot = head.rotation.eulerAngles;

        start_eyesScale = new Vector3[eyes.Length];
        for (int i = 0; i < eyes.Length; i++)
        {
            start_eyesScale[i] = eyes[i].localScale;
        }

        start_eyebrowsPos = new Vector3[eyebrows.Length];
        start_eyebrowsRot = new Vector3[eyebrows.Length];
        for (int i = 0; i < eyebrows.Length; i++)
        {
            start_eyebrowsPos[i] = eyebrows[i].localPosition;
            start_eyebrowsRot[i] = eyebrows[i].localEulerAngles;
        }
    }

    public void LookAtObject(Transform target)
    {
        head.DOLookAt(target.position, 0.5f);
    }


    // Прищуривание (0% - 100%)
    public void Squint(int power)
    {
        for (int i = 0; i < eyes.Length; i++)
        {
            eyes[i].DOScale(new Vector3(start_eyesScale[i].x, start_eyesScale[i].y * (power / 100f), start_eyesScale[i].z), 0.5f);
        }
    }
    public void ResetSquint()
    {
        for (int i = 0; i < eyes.Length; i++)
        {
            eyes[i].DOScale(start_eyesScale[i], 1);
        }
    }
    // Брови Вверх (0% - 100%)
    public void EyebrowsUp(float power)
    {
        for (int i = 0; i < eyebrows.Length; i++)
        {
            eyebrows[i].DOLocalMoveY(start_eyebrowsPos[i].y + (0.03f * (power / 100f)), 0.5f);
        }
    }
    public void ResetEyebrowsUp()
    {
        for (int i = 0; i < eyebrows.Length; i++)
        {
            eyebrows[i].DOLocalMoveY(start_eyebrowsPos[i].y, 1);
        }
    }
    // Брови Поворот (0 - 100 градусов)
    public void EyebrowsRot(float power)
    {
        for (int i = 0; i < eyebrows.Length; i++)
        {
            eyebrows[i].DOLocalRotate(start_eyebrowsRot[i] + new Vector3(power, 0, 0), 0.5f);
        }
    }
    public void ResetEyebrowsRot()
    {
        for (int i = 0; i < eyebrows.Length; i++)
        {
            eyebrows[i].DOLocalRotate(start_eyebrowsRot[i], 1);
        }
    }





    public void ResetAll()
    {
        head.DORotate(start_headRot, 1);
        ResetSquint();
        ResetEyebrowsUp();
        ResetEyebrowsRot();
    }

    public void PlayAudio(AudioClip clip)
    {
        source.PlayOneShot(clip, 0.5f);
    }

}
