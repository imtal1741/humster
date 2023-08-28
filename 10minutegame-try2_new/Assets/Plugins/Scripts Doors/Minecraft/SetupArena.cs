using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using Lean.Localization;

public class SetupArena : MonoBehaviour
{
    public NavMeshSurface surface;
    public Health health;
    public WeaponChoice weaponChoice;

    public float spawnRange = 40f;

    public GameObject[] levels;
    public MobSet[] mobSets;

    public GameObject mobRoot;

    public Transform player;

    public LeanToken leanTokenLevel;
    string isMaxText;
    public int level;

    int enemyKilled;
    int enemyGoal;
    public TextMeshProUGUI enemyGoalText;

    List<Vector3> tempPos = new List<Vector3>();

    public GameObject AdvertWeaponMenu;



    //public int nowLVL;

    ////Need to delete
    //void Start()
    //{
    //    level = nowLVL;
    //    Setup();
    //}


    public void ClearArena(bool clearMobsOnly)
    {
        if (clearMobsOnly == false)
        {
            for (int i = 0; i < levels.Length; i++)
            {
                levels[i].SetActive(false);
            }
        }

        foreach (Transform child in mobRoot.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void Setup()
    {
        if (level == 0)
            level = 1;

        UpdateLevelText();

        AdvertWeaponMenu.SetActive(true);
        weaponChoice.advWeapon = false;

        //Clear

        ClearArena(false);



        //Create

        levels[level - 1].SetActive(true);
        surface.BuildNavMesh();

        enemyKilled = 0;
        enemyGoal = 0;
        MobSet set = mobSets[level - 1];
        for (int i = 0; i < set.mobs.Length; i++)
        {
            for (int j = 0; j < set.mobs[i].count; j++)
            {

                Vector3 point;
                if (RandomPoint(transform.position, spawnRange, set.mobs[i].isDistant, out point))
                {
                    GameObject m = Instantiate(set.mobs[i].mobprefab, point, Quaternion.identity);
                    Enemy enemy = m.GetComponent<Enemy>();
                    if (!enemy)
                    {
                        enemy = m.transform.GetChild(0).GetComponent<Enemy>();
                    }
                    enemy.target = player;
                    enemy.setupArena = this;
                    m.transform.SetParent(mobRoot.transform);

                    enemyGoal++;
                }

            }
        }
        tempPos.Clear();

        enemyGoalText.text = enemyKilled + " / " + enemyGoal;

    }


    public IEnumerator UpdateKillStat()
    {
        enemyKilled++;
        enemyGoalText.text = enemyKilled + " / " + enemyGoal;

        if (enemyKilled >= enemyGoal)
        {
            if (level < levels.Length)
            {
                level++;
            }
            health.yandexSDK.Save(level);

            yield return new WaitForSeconds(1f);
            health.playerRespawn.BlackBorders_anim.SetBool("Close", true);
            yield return new WaitForSeconds(0.6f);

            health.ResetPlayer();

            yield return new WaitForSeconds(0.2f);

            Setup();

            health.playerRespawn.ShowPlay();
            health.yandexSDK.ShowAdvert();

            health.playerRespawn.BlackBorders_anim.SetBool("Close", false);
        }
    }

    public void UpdateLevelText()
    {
        if (level == levels.Length)
            isMaxText = Lean.Localization.LeanLocalization.GetTranslationText("MAX");
        else
            isMaxText = "";

        leanTokenLevel.SetValue(level + " " + isMaxText);
    }






    bool RandomPoint(Vector3 center, float range, bool isDistant, out Vector3 result)
    {
        int tempArea;
        if (isDistant)
            tempArea = 262144;
        else
            tempArea = 512;

        for (int i = 0; i < 10; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1000f, tempArea))
            {
                int x = 0;
                for (int j = 0; j < tempPos.Count; j++)
                {
                    if (Vector3.Distance(tempPos[j], hit.position) <= 1.5f)
                    {
                        x++;
                    }
                }

                if (x <= 0)
                {
                    result = hit.position;
                    tempPos.Add(result);
                    return true;
                }
            }
        }
        result = Vector3.zero;
        return false;
    }



    public void UnlockWeapon()
    {
        AdvertWeaponMenu.SetActive(false);
        weaponChoice.advWeapon = true;

        weaponChoice.ResetInventory();
        weaponChoice.Weapons[2].SetActive(true);
    }

    //void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, spawnRange);
    //}
}
