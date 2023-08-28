using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using TMPro;
using CMF;

public class ChunksPlacer : MonoBehaviour
{
    public YandexSDK yandexSDK;

    public MapSet ChunkPrefabs;
    public MapSet ChunkPrefabsSeek;
    public Chunk FirstChunk;
    GameObject lastTimeline;

    public Material[] wallMaterials;
    public Material[] carpetMaterials;

    public List<GameObject> HighProps = new List<GameObject>();
    public List<GameObject> LowProps = new List<GameObject>();

    public CameraController cameraController;
    public AdvancedWalkerController advancedWalkerController;
    [HideInInspector] public float startBobIntensity;
    [HideInInspector] public float startMovementSpeed;
    public GameObject bossRush;
    public GameObject bossSeek;
    [HideInInspector] public WaypointsMove waypointsMoveBoss; // Last Boss

    public GameObject hintRush;

    Chunk newChunk;
    private List<GameObject> AllProps = new List<GameObject>();
    [HideInInspector] public List<Chunk> spawnedChunks = new List<Chunk>();
    [HideInInspector] public SpawnProps propWithKey;
    [HideInInspector] public int chunkNum = 0;
    int chunkNumSeek = 0;
    int lowInteractCount = 0;
    bool isKeyChunk;
    [HideInInspector] public bool isRushNow;
    bool isRushNowSaveLast;
    [HideInInspector] public bool isSeekNow;
    [HideInInspector] public bool canSpawnSeekRooms;

    private void Start()
    {
        startBobIntensity = cameraController.bobIntensity;
        startMovementSpeed = advancedWalkerController.movementSpeed;

        AllProps.AddRange(HighProps);
        AllProps.AddRange(LowProps);

        spawnedChunks.Add(FirstChunk);
        GenChunk();
    }

    public void SpawnChunk()
    {
        chunkNum++;
        lowInteractCount = 0;
        newChunk = null;
        int saveWallMaterialNumber = 0;

        float rand = Random.Range(0f, 100f);
        for (int i = 0; i < ChunkPrefabs.maps.Length; i++)
        {
            if (rand >= ChunkPrefabs.maps[i].chanceMin && rand <= ChunkPrefabs.maps[i].chanceMax)
            {
                newChunk = Instantiate(ChunkPrefabs.maps[i].mapPrefab[Random.Range(0, ChunkPrefabs.maps[i].mapPrefab.Length)]).GetComponent<Chunk>();
                newChunk.transform.rotation = spawnedChunks[spawnedChunks.Count - 1].End.rotation;
                newChunk.transform.position = spawnedChunks[spawnedChunks.Count - 1].End.position - newChunk.Begin.localPosition;
                spawnedChunks.Add(newChunk);

                break;
            }
        }

        if (newChunk.wallMaterialNumber >= 0)
        {
            Material[] rendMats;
            MeshRenderer newChunkRenderer = newChunk.GetComponent<MeshRenderer>();
            rendMats = newChunkRenderer.materials;
            saveWallMaterialNumber = Random.Range(0, wallMaterials.Length);
            rendMats[newChunk.wallMaterialNumber] = wallMaterials[saveWallMaterialNumber];
            newChunkRenderer.materials = rendMats;
        }

        if (Random.Range(0, 100) < 70)
        {
            int tempRandCarpet = Random.Range(0, 2) * 2; // (0 or 1) * 2 = 0 or 2
            Material[] rendMatsCarpet = new Material[2];
            for (int i = 0; i < newChunk.Carpets.Count; i++)
            {
                rendMatsCarpet[0] = carpetMaterials[0 + tempRandCarpet];
                rendMatsCarpet[1] = carpetMaterials[1 + tempRandCarpet];
                newChunk.Carpets[i].materials = rendMatsCarpet;
            }
        }
        else
        {
            for (int i = 0; i < newChunk.Carpets.Count; i++)
            {
                newChunk.Carpets[i].gameObject.SetActive(false);
            }
        }

        DoorElements doorElem = newChunk.animDoor.GetComponent<DoorElements>();
        doorElem.TMPtext.text = "00" + (chunkNum + 1).ToString();
        if ((newChunk.HighPoints.Count > 0 || newChunk.LowPoints.Count > 0) && Random.Range(0, 100) < 30)
        {
            isKeyChunk = true;
            doorElem.isKey = true;
            doorElem.padlock.SetActive(true);
        }
        else
        {
            isKeyChunk = false;
            doorElem.isKey = false;
            doorElem.padlock.SetActive(false);
        }


        bool closetWasSpawned = false;
        for (int i = 0; i < newChunk.HighPoints.Count; i++)
        {
            if (i == newChunk.HighPoints.Count - 1 && isKeyChunk && newChunk.LowPoints.Count == 0)
            {
                SpawnKeyProp(newChunk.HighPoints[i]);
                break;
            }
            if (i == newChunk.HighPoints.Count - 2 && closetWasSpawned == false)
            {
                // Spawn Closet
                GameObject newProp = Instantiate(HighProps[0]);
                newProp.transform.rotation = newChunk.HighPoints[i].rotation;
                newProp.transform.position = newChunk.HighPoints[i].position;
                newProp.transform.parent = newChunk.transform;

                closetWasSpawned = true;
                continue;
            }

            if (Random.Range(0, 100) < 50)
            {
                GameObject newProp = null;
                if (lowInteractCount < 5)
                {
                    newProp = Instantiate(AllProps[Random.Range(0, AllProps.Count)]);
                }
                else
                {
                    newProp = Instantiate(HighProps[Random.Range(0, HighProps.Count)]);
                }
                newProp.transform.rotation = newChunk.HighPoints[i].rotation;
                newProp.transform.position = newChunk.HighPoints[i].position;
                newProp.transform.parent = newChunk.transform;

                if (LowProps.Contains(newProp))
                {
                    lowInteractCount++;

                    SpawnProps spawnProps = newProp.transform.GetChild(0).GetComponent<SpawnProps>();
                    if (isKeyChunk && Random.Range(0, 100) < 75)
                    {
                        spawnProps.DoSpawn(true);

                        propWithKey = spawnProps;
                        isKeyChunk = false;
                    }
                    else
                    {
                        spawnProps.DoSpawn(false);
                    }
                }
                else
                {
                    // Spawn Bookshelf or Chair
                    if (newProp.CompareTag("Chair"))
                    {
                        Material[] rendMats;
                        MeshRenderer newPropRenderer = newProp.GetComponent<MeshRenderer>();
                        rendMats = newPropRenderer.materials;
                        rendMats[0] = wallMaterials[saveWallMaterialNumber];
                        newPropRenderer.materials = rendMats;
                    }
                }
            }
            else if(Random.Range(0, 100) < 25)
            {
                // Spawn Closet
                GameObject newProp = Instantiate(HighProps[0]);
                newProp.transform.rotation = newChunk.HighPoints[i].rotation;
                newProp.transform.position = newChunk.HighPoints[i].position;
                newProp.transform.parent = newChunk.transform;

                closetWasSpawned = true;
            }
        }

        for (int i = 0; i < newChunk.LowPoints.Count; i++)
        {
            if (i == newChunk.LowPoints.Count - 1 && isKeyChunk)
            {
                SpawnKeyProp(newChunk.LowPoints[i]);
                break;
            }

            if (Random.Range(0, 100) < 50 && lowInteractCount < 5)
            {
                lowInteractCount++;

                GameObject newProp = Instantiate(LowProps[Random.Range(0, LowProps.Count)]);
                newProp.transform.rotation = newChunk.LowPoints[i].rotation;
                newProp.transform.position = newChunk.LowPoints[i].position;
                newProp.transform.parent = newChunk.transform;

                SpawnProps spawnProps = newProp.transform.GetChild(0).GetComponent<SpawnProps>();
                if (isKeyChunk && Random.Range(0, 100) < 50)
                {
                    spawnProps.DoSpawn(true);

                    propWithKey = spawnProps;
                    isKeyChunk = false;
                }
                else
                {
                    spawnProps.DoSpawn(false);
                }

            }
        }

    }


    void SpawnKeyProp(Transform point)
    {
        GameObject newProp = Instantiate(LowProps[0]);
        newProp.transform.rotation = point.rotation;
        newProp.transform.position = point.position;
        newProp.transform.parent = newChunk.transform;

        SpawnProps spawnProps = newProp.transform.GetChild(0).GetComponent<SpawnProps>();
        spawnProps.DoSpawn(true);

        propWithKey = spawnProps;
        isKeyChunk = false;
    }

    public void SpawnChunkSeek()
    {
        chunkNum++;
        newChunk = null;

        newChunk = Instantiate(ChunkPrefabsSeek.maps[chunkNumSeek].mapPrefab[Random.Range(0, ChunkPrefabsSeek.maps[chunkNumSeek].mapPrefab.Length)]).GetComponent<Chunk>();
        newChunk.transform.rotation = spawnedChunks[spawnedChunks.Count - 1].End.rotation;
        newChunk.transform.position = spawnedChunks[spawnedChunks.Count - 1].End.position - newChunk.Begin.localPosition;
        spawnedChunks.Add(newChunk);


        DoorElements doorElem = newChunk.animDoor.GetComponent<DoorElements>();
        doorElem.TMPtext.text = "00" + (chunkNum + 1).ToString();

        isKeyChunk = false;
        doorElem.isKey = false;
        doorElem.padlock.SetActive(false);


        if (newChunk.Timeline)
        {
            lastTimeline = newChunk.Timeline;
        }


        if (chunkNumSeek == 2)
        {
            if (lastTimeline)
            {
                StartCoroutine(PlayCutScene(8.6f));
            }
        }

        if (chunkNumSeek == ChunkPrefabsSeek.maps.Length - 1)
        {
            canSpawnSeekRooms = false;

            return;
        }

        chunkNumSeek++;
    }

    IEnumerator PlayCutScene(float sec)
    {
        lastTimeline.SetActive(true);

        cameraController.audioEffects.audioSource.clip = cameraController.audioEffects.music;
        cameraController.audioEffects.audioSource.Play();

        GameObject tempPhysics = cameraController.PlayerTr.gameObject;
        GameObject tempVisual = cameraController.VisualModel;

        tempPhysics.SetActive(false);
        tempVisual.SetActive(false);
        cameraController.enabled = false;

        yield return new WaitForSeconds(sec);

        tempPhysics.SetActive(true);
        tempVisual.SetActive(true);
        cameraController.enabled = true;

        lastTimeline.SetActive(false);

        SpawnBoss(bossSeek);

        cameraController.bobIntensity = 0.65f;
        advancedWalkerController.movementSpeed = 4.25f;
    }

    public void GenChunk()
    {
        StartCoroutine(GenChunkIE());
    }


    public IEnumerator GenChunkIE()
    {
        yandexSDK.SaveWithCheck(chunkNum);

        if (spawnedChunks.Count >= 3)
        {
            spawnedChunks[0].animDoor.CrossFade("Door Close", 0.001f);
            yield return new WaitForSeconds(1f);
            spawnedChunks[0].animDoor.transform.parent.transform.parent = spawnedChunks[1].transform;
            Destroy(spawnedChunks[0].gameObject);
            spawnedChunks.RemoveAt(0);
        }
        else if (!spawnedChunks.Contains(FirstChunk))
        {
            SpawnChunk();
        }

        if (canSpawnSeekRooms)
        {
            SpawnChunkSeek();

            if (waypointsMoveBoss)
            {
                waypointsMoveBoss.waypoints.Add(spawnedChunks[spawnedChunks.Count - 1].Middle.position);
                waypointsMoveBoss.waypoints.Add(spawnedChunks[spawnedChunks.Count - 1].End.position);
            }
        }
        else
        {
            SpawnChunk();
        }

        if (chunkNum > 3)
        {
            if ((chunkNum % 25 == 0) && !isSeekNow)
            {
                if (isRushNow)
                {
                    waypointsMoveBoss.DestroyAll();
                }
                isSeekNow = true;
                canSpawnSeekRooms = true;
                chunkNumSeek = 0;
            }
            else if (!isSeekNow && !isRushNow && (chunkNum % 24 != 0) && Random.Range(0, 100) < 13)
            {
                if (isRushNowSaveLast)
                {
                    isRushNowSaveLast = false;
                    yield break;
                }

                isRushNow = true;
                isRushNowSaveLast = true;
                hintRush.SetActive(true);

                int times = Mathf.Clamp(Mathf.RoundToInt(15 -(0.36f * chunkNum)), 8, 15);

                for (int t = 0; t < times; t++)
                {
                    if (cameraController.isHide == false)
                    {
                        yield return new WaitForSeconds(1);
                    }
                    else
                    {
                        break;
                    }
                }

                SpawnBoss(bossRush);
            }
        }

    }

    void SpawnBoss(GameObject bossPrefab)
    {
        GameObject boss = Instantiate(bossPrefab);
        WaypointsMove waypointsMove = boss.GetComponent<WaypointsMove>();
        waypointsMoveBoss = waypointsMove;
        waypointsMove.cameraController = cameraController;

        List<Vector3> tempPoints = new List<Vector3>();
        if (isRushNow)
        {
            boss.transform.position = spawnedChunks[spawnedChunks.Count - 3].Begin.position;
            for (int i = 0; i < spawnedChunks.Count; i++)
            {
                tempPoints.Add(spawnedChunks[i].Begin.position);
                tempPoints.Add(spawnedChunks[i].Middle.position);
            }
            tempPoints.Add(spawnedChunks[spawnedChunks.Count - 1].End.position);
            tempPoints.Add(tempPoints[tempPoints.Count - 1] + (tempPoints[tempPoints.Count - 1] - tempPoints[tempPoints.Count - 2]) * 2);
        }
        else if (isSeekNow)
        {
            boss.transform.position = spawnedChunks[spawnedChunks.Count - 3].Middle.position;
            tempPoints.Add(spawnedChunks[spawnedChunks.Count - 3].End.position);
            tempPoints.Add(spawnedChunks[spawnedChunks.Count - 2].Middle.position);
            tempPoints.Add(spawnedChunks[spawnedChunks.Count - 2].End.position);
            tempPoints.Add(spawnedChunks[spawnedChunks.Count - 1].Middle.position);
            tempPoints.Add(spawnedChunks[spawnedChunks.Count - 1].End.position);
        }


        waypointsMove.waypoints = tempPoints;
        waypointsMove.chunksPlacer = this;
    }

}