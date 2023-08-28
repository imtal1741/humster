using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public float moveSpeed;
    public bool isEnemyBullet;
    [HideInInspector] public int damage;


    void OnTriggerEnter(Collider c)
    {
        if (isEnemyBullet)
        {
            if (c.gameObject.CompareTag("Player"))
            {
                Health health = c.gameObject.GetComponent<Health>();

                health.TakeDamage(3);
                health.StartCoroutine(health.ShowEnemyBullet());
            }
        }
        else
        {
            if (c.gameObject.CompareTag("Enemy"))
            {
                Enemy enemy = c.gameObject.GetComponent<Enemy>();
                Hitbox hitbox = c.gameObject.GetComponent<Hitbox>();

                if (enemy)
                {
                    c.gameObject.GetComponent<Enemy>().TakeDamage(damage, 1, enemy.material);
                }
                else if (hitbox)
                {
                    hitbox.enemy.TakeDamage(damage, hitbox.multiplier, hitbox.renderer.material);
                }

            }
        }


        Destroy(gameObject);
    }


    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);
    }
}
