using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using CMF;
using EZCameraShake;

public class Interaction : MonoBehaviour
{
    public AudioSource audioSource;

    public bool isUsed;
    public int materialNumber;

    [Header("If is Closet")]
    public Animator animCloset;

    [Header("If is Drawer")]
    public Transform propsPoint;
    public PropSet propSet;

    [Header("If is Hide")]
    public bool isHide;
    public Transform hidePoint;
    public Transform exitPoint;

    [Header("If is Buy")]
    public bool isBuy;
    public DoorElements doorElem;

    [Header("If is Lever")]
    public bool isLever;
    public Transform ObjectToRotate;
    public Vector3 NewRotation;

    bool isOpen;
    [HideInInspector] public bool isAnimate;
    [HideInInspector] public bool isKey;
    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    public void Interact(CameraController cameraController, AdvancedWalkerController advancedWalkerController)
    {
        if (isAnimate) return;

        isAnimate = true;

        if (isLever)
        {
            transform.DOLocalRotate(new Vector3(-140f, 0f, 0f), 1f);
            ObjectToRotate.localEulerAngles = NewRotation;

            cameraController.audioEffects.PlaySound(audioSource, cameraController.audioEffects.key);

            return;
        }

        if (isBuy)
        {
            if (cameraController.coinsManager.Coins >= 50)
            {
                cameraController.coinsManager.Coins -= 50;
                cameraController.chunksPlacer.propWithKey.DeleteKey();
                cameraController.OpenDoor(doorElem, true);
            }
            else
            {
                Invoke("ResetIsAnimate", 0.5f);
            }

            return;
        }

        if (isHide)
        {
            if (cameraController.audioEffects)
                cameraController.audioEffects.PlaySound(audioSource, cameraController.audioEffects.closetOpen);

            cameraController.enabled = false;
            advancedWalkerController.enabled = false;
            cameraController.isHide = true;

            cameraController.UI_hand.SetActive(false);

            advancedWalkerController.GetComponent<Rigidbody>().isKinematic = true;
            advancedWalkerController.limitedInputX = 0f;
            advancedWalkerController.limitedInputY = 0f;
            cameraController.VisualTr.GetComponent<PosFollow>().enabled = false;
            Animator anim = cameraController.VisualTr.GetComponent<Animator>();
            anim.SetFloat("horizontal", 0);
            anim.SetFloat("vertical", 0);

            //ShakeCamera
            CameraShaker.Instance.ShakeOnce(1.35f, 1f, 0.5f, 2.5f);

            //Closet Animation
            if (animCloset)
                animCloset.CrossFade("Closet Open", 0.001f);

            cameraController.transform.DOLocalRotateQuaternion(Quaternion.identity, 1f).SetEase(Ease.InOutQuad);
            cameraController.VisualTr.DOLocalRotateQuaternion(hidePoint.rotation, 1f).SetEase(Ease.InOutQuad);
            cameraController.VisualTr.DOMove(hidePoint.position, 1f)
                    .SetEase(Ease.InOutQuad)
                    .OnComplete(() => {
                        cameraController.cameraHideOffset = hidePoint.rotation.eulerAngles.y;
                        cameraController.currentXAngle = 0f;
                        cameraController.currentYAngle = hidePoint.rotation.eulerAngles.y;
                        cameraController.enabled = true;
                    });
        }
        else
        {
            if (!isUsed)
            {
                isUsed = true;

                //Spawn prop
                if (isKey)
                {
                    GameObject m = Instantiate(propSet.key,
                        propsPoint.position + new Vector3(Random.Range(-0.075f, 0.075f), 0, Random.Range(-0.075f, 0.075f)),
                        Quaternion.Euler(0, Random.Range(0, 360), 0));

                    m.transform.SetParent(propsPoint);
                }
                else
                {
                    for (int i = 0; i < propSet.props.Length; i++)
                    {
                        int rand = Random.Range(0, 100);
                        if (rand <= propSet.props[i].chance)
                        {
                            GameObject m = Instantiate(propSet.props[i].propPrefab[Random.Range(0, propSet.props[i].propPrefab.Length)],
                                propsPoint.position + new Vector3(Random.Range(-0.075f, 0.075f), 0, Random.Range(-0.075f, 0.075f)),
                                Quaternion.Euler(0, Random.Range(0, 360), 0));

                            m.transform.SetParent(propsPoint);

                            break;
                        }
                    }
                }
                //
            }


            isOpen = !isOpen;

            if (cameraController.audioEffects)
            {
                if (isOpen)
                    cameraController.audioEffects.PlaySound(audioSource, cameraController.audioEffects.drawerOpen);
                else
                    cameraController.audioEffects.PlaySound(audioSource, cameraController.audioEffects.drawerClose);
            }


            transform.DOMove(startPos + (isOpen ? transform.forward * 0.4f : Vector3.zero), 0.3f)
                    .SetEase(Ease.InOutQuad)
                    .OnComplete(() => {
                        isAnimate = false;
                    });
        }
    }

    public void hideExit(CameraController cameraController, AdvancedWalkerController advancedWalkerController)
    {
        if (cameraController.audioEffects)
            cameraController.audioEffects.PlaySound(audioSource, cameraController.audioEffects.closetClose);

        cameraController.enabled = false;
        advancedWalkerController.enabled = false;

        //ShakeCamera
        CameraShaker.Instance.ShakeOnce(1.35f, 1f, 0.5f, 2.5f);

        //Closet Animation
        if (animCloset)
            animCloset.CrossFade("Closet Open", 0.001f);

        cameraController.transform.DOLocalRotateQuaternion(Quaternion.identity, 1f).SetEase(Ease.InOutQuad);
        cameraController.VisualTr.DOLocalRotateQuaternion(exitPoint.rotation, 1f).SetEase(Ease.InOutQuad);
        cameraController.VisualTr.DOMove(exitPoint.position, 1f)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() => {
                    advancedWalkerController.transform.position = exitPoint.position;

                    advancedWalkerController.GetComponent<Rigidbody>().isKinematic = false;
                    cameraController.VisualTr.GetComponent<PosFollow>().enabled = true;
                    cameraController.currentXAngle = 0f;
                    cameraController.currentYAngle = exitPoint.rotation.eulerAngles.y;
                    cameraController.isHide = false;
                    cameraController.enabled = true;
                    advancedWalkerController.enabled = true;

                    isAnimate = false;
                });
    }

    void ResetIsAnimate()
    {
        isAnimate = false;
    }

}
