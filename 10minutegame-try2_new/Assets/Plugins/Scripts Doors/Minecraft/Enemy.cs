using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class Enemy : MonoBehaviour
{
    [Header("Main Components")]
    public Renderer renderer;
    [HideInInspector] public Material material;
    public Rigidbody[] rb;


    public bool isDistance;
    [Header("For Distance")]
    public GameObject bulletObject;
    public Transform attackPoint;
    public GameObject visibleBulletInHand;

    [Header("Other")]
    public int health;
    bool death;
    bool isAttackingNow;

    public NavMeshAgent agent;
    public Animator anim;
    public Transform target;
    Health playerHP;
    [HideInInspector] public SetupArena setupArena;

    [Header("Sound")]
    public AudioClip impact;
    public AudioClip deathImpact;
    public AudioSource audioSource; // Also is object for rotation (LookAt)

    void Start()
    {
        if (renderer)
            material = renderer.material;
        playerHP = target.GetComponent<Health>();

        StartCoroutine(CheckDistance());
    }

    public void Attack()
    {
        if ((isDistance == false && Vector3.Distance(transform.position, target.position) > 4f) || isAttackingNow == false)
            return;

        if (isDistance)
        {
            visibleBulletInHand.SetActive(false);

            GameObject tempBullet = Instantiate(bulletObject, attackPoint.position, Quaternion.identity);
            tempBullet.GetComponent<Bullet>().damage = 3;
            tempBullet.transform.LookAt(target.position + new Vector3(0, 0.9f, 0));
        }
        else
        {
            playerHP.TakeDamage(3);
        }

        isAttackingNow = false;
    }

    public void TakeDamage(int damage, int multiplier, Material material)
    {
        health -= damage * multiplier;
        if (health <= 0 && !death)
        {
            audioSource.PlayOneShot(deathImpact, 0.25f);

            StartCoroutine(DoDeath());
        }
        else if (health > 0)
        {
            audioSource.PlayOneShot(impact, 0.25f);


            var s = DOTween.Sequence();
            s.Append(material.DOFloat(1, "_ColorIntenseAdd", 0.075f));
            s.Append(material.DOFloat(0, "_ColorIntenseAdd", 0.2f));

        }
    }


    IEnumerator CheckDistance()
    {

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        float rand = Random.Range(0.3f, 0.6f);

        yield return new WaitForSeconds(rand);

        if (!death)
        {
            if (!isAttackingNow)
            {
                if (distanceToTarget < 35f)
                {
                    if (isDistance == false)
                    {
                        if (distanceToTarget < 2f)
                        {
                            // take damage
                            isAttackingNow = true;

                            agent.ResetPath();
                            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") == false)
                                anim.CrossFade("Attack", 0.1f);
                        }
                        else
                        {
                            agent.destination = target.position;
                            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Run") == false)
                                anim.CrossFade("Run", 0.1f);
                        }
                    }
                    else
                    {
                        isAttackingNow = true;

                        visibleBulletInHand.SetActive(true);

                        audioSource.transform.DOLookAt(new Vector3(target.position.x, transform.position.y, target.position.z), 1f);
                        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") == false)
                            anim.CrossFade("Attack", 0.1f);
                    }
                }
                else
                {
                    if (isDistance == false)
                        agent.ResetPath();
                    if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") == false)
                        anim.CrossFade("Idle", 0.1f);
                }
            }

            if (isAttackingNow && isDistance == false && distanceToTarget > 3f)
            {
                isAttackingNow = false;

                agent.destination = target.position;
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Run") == false)
                    anim.CrossFade("Run", 0.1f);
            }

            StartCoroutine(CheckDistance());
        }

    }

    IEnumerator DoDeath()
    {
        death = true;
        setupArena.StartCoroutine(setupArena.UpdateKillStat());

        if (agent)
        {
            agent.Stop();
            agent.enabled = false;
        }

        if (renderer)
        {

            GetComponent<BoxCollider>().enabled = false;

            foreach (Rigidbody rigid in rb)
            {
                rigid.isKinematic = false;
                rigid.GetComponent<BoxCollider>().enabled = true;
            }
            anim.enabled = false;

            rb[0].AddForce((transform.position - target.position).normalized * 40000);

            yield return new WaitForSeconds(5);

            foreach (Rigidbody rigid in rb)
            {
                rigid.isKinematic = true;
                rigid.GetComponent<BoxCollider>().enabled = false;
            }

            rb[0].transform.DOScale(Vector3.zero, 1).SetEase(Ease.InBack);
        }
        else
        {
            anim.CrossFade("Die", 0.3f);

            yield return new WaitForSeconds(5);

            transform.DOScale(Vector3.zero, 1).SetEase(Ease.InBack);
        }


        Destroy(gameObject, 6);
    }

}
