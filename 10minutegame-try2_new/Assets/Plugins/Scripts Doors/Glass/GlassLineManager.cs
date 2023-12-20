using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassLineManager : MonoBehaviour
{
    public List<GlassLine> glassLines = new List<GlassLine>();

    public bool needBots;
    public GameObject botPrefab;
    public int countBots;
    public Transform startPoint;

    void Start()
    {
        if (needBots)
        {
            int maxGlasses = glassLines.Count < 4 ? glassLines.Count : 4;
            Vector3 pos = Vector3.zero;

            for (int i = 0; i < countBots; i++)
            {
                if (Random.Range(0, 100) < 25)
                {
                    GlassLine gl = glassLines[Random.Range(0, maxGlasses)];
                    Vector2 randomOffset = Random.insideUnitCircle * 1;

                    pos = gl.glasses[gl.trueGlass].position + new Vector3(randomOffset.x, 0, randomOffset.y);
                }
                else
                {
                    Vector2 randomOffset = Random.insideUnitCircle * 2;
                    pos = startPoint.position + new Vector3(randomOffset.x, 0, randomOffset.y);
                }

                Instantiate(botPrefab, pos, Quaternion.identity);
            }
        }
    }


}
