using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NumberState : MonoBehaviour
{

    public int state;


    public void DestroyIt()
    {
        BoxCollider collider = GetComponent<BoxCollider>();
        if (collider)
            collider.enabled = false;

        GameObject particles = Instantiate(Resources.Load("ParticlePunch", typeof(GameObject)), transform.position + new Vector3(0, 0.2f, 0), Quaternion.identity) as GameObject;

        transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() => Destroy(gameObject));
    }
}
