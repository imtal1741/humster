using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMF;
using EZCameraShake;

public class WaypointsMove : MonoBehaviour
{
    public List<Vector3> waypoints;
    public Vector3 offset;
    public float speed = 5;
    public bool isSeek;
    bool seekOnLastPoint;

    [HideInInspector] public CameraController cameraController;
    [HideInInspector] public ChunksPlacer chunksPlacer;

    int index;
    [HideInInspector] public CameraShakeInstance shake;

    bool isPlayerConnect;
    AudioSource audioSourcePlayer;
    AudioSource audioSource;

    void Start()
    {
        shake = CameraShaker.Instance.StartShake(3f, 4f, 0.1f, Vector3.zero, new Vector3(0.1f, 0.1f, 0));
        shake.DeleteOnInactive = false;

        audioSource = GetComponent<AudioSource>();
        if (isSeek)
        {
            audioSourcePlayer = cameraController.audioEffects.audioSource;
        }

        transform.position += offset;
        transform.rotation = Quaternion.LookRotation((waypoints[1] - waypoints[0]).normalized);
    }


    void Update()
    {
        if (index < waypoints.Count)
        {
            Vector3 destination = waypoints[index];
            Vector3 newPos = Vector3.MoveTowards(transform.position, destination + offset, speed * Time.deltaTime);
            transform.position = newPos;

            float distance = Vector3.Distance(transform.position, destination + offset);

            if (isSeek && index == waypoints.Count -1 && distance <= 3f && seekOnLastPoint == false)
            {
                seekOnLastPoint = true;
                StartCoroutine(DoorBeatingSeekIE());
            }

            if (distance <= 1f)
            {
                index++;

                if (index != waypoints.Count)
                    transform.rotation = Quaternion.LookRotation((waypoints[index] - destination).normalized);
            }
        }

        if (shake != null)
        {
            float distPlayer = Vector3.Distance(transform.position, cameraController.transform.position);
            float parabolaX = Mathf.Clamp((25 - distPlayer) / 1.5f, 0f, 20f);
            float parabolaY = -0.025f * Mathf.Pow((parabolaX - 20f), 2f) + 10f;
            shake.ScaleMagnitude = parabolaY;

            if (isPlayerConnect)
            {
                shake.DeleteOnInactive = true;
                shake.StartFadeOut(3f);
                shake = null;
            }
        }

        if (index == waypoints.Count)
        {
            index++;

            if (isSeek == false)
            {
                DestroyAll();
            }
        }
    }

    IEnumerator DoorBeatingSeekIE()
    {
        chunksPlacer.spawnedChunks[0].animDoor.CrossFade("Door Close", 0.001f);
        shake.DeleteOnInactive = true;
        shake.StartFadeOut(3f);
        shake = null;

        audioSourcePlayer.Stop();

        audioSource.Stop();
        AudioClip tempClip = cameraController.audioEffects.beat;

        cameraController.audioEffects.PlaySound(cameraController.audioEffects.endSound);
        yield return new WaitForSeconds(1f);
        audioSource.PlayOneShot(tempClip);
        CameraShaker.Instance.ShakeOnce(0.8f, 3.5f, 0f, 1.5f);
        yield return new WaitForSeconds(1f);
        audioSource.PlayOneShot(tempClip);
        CameraShaker.Instance.ShakeOnce(0.8f, 3.5f, 0f, 1.5f);
        yield return new WaitForSeconds(1.5f);
        audioSource.PlayOneShot(tempClip);
        CameraShaker.Instance.ShakeOnce(0.8f, 3.5f, 0f, 1.5f);
        yield return new WaitForSeconds(1f);
        DestroyAll();
    }

    public void DestroyAll()
    {
        if (shake != null)
        {
            shake.DeleteOnInactive = true;
            shake.StartFadeOut(3f);
            shake = null;
        }

        if (isSeek)
        {
            chunksPlacer.cameraController.bobIntensity = chunksPlacer.startBobIntensity;
            chunksPlacer.advancedWalkerController.movementSpeed = chunksPlacer.startMovementSpeed;
            chunksPlacer.isSeekNow = false;
            chunksPlacer.canSpawnSeekRooms = false;
        }
        else
        {
            chunksPlacer.isRushNow = false;
            chunksPlacer.hintRush.SetActive(false);
        }
        chunksPlacer.waypointsMoveBoss = null;

        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.CompareTag("Player"))
        {
            if (cameraController.isHide == false)
            {
                Health health = c.gameObject.GetComponent<Health>();

                health.Respawn(true, gameObject.name);
            }
            else
            {
                isPlayerConnect = true;
            }
        }
    }
}
