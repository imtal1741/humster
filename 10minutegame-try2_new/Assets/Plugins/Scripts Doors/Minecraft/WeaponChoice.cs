using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponChoice : MonoBehaviour
{

    public Animator anim;
    public GameObject[] Weapons;

    public bool advWeapon;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ResetInventory();

            Weapons[0].SetActive(true);

        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ResetInventory();

            Weapons[1].SetActive(true);

        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && advWeapon)
        {
            ResetInventory();

            Weapons[2].SetActive(true);

        }
    }

    public void ResetInventory()
    {
        for (int i = 0; i < Weapons.Length; i++)
        {
            Weapons[i].SetActive(false);
        }

        anim.Play("GetUpWeapon", -1, 0f);
    }
}
