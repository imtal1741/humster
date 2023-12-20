using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class AgentGlassLogic : MonoBehaviour
{

    public AgentMover agentMover;
    public Transform player;
    public float maxDistanceToPlayer = 15f;
    public Transform startPos;
    NavMeshAgent navMeshAgent;
    Animator anim;
    [HideInInspector] public bool moveDown = false;
    public bool waitColab = false;
    public bool goPortal = false;

    NavMeshPath path;
    bool lastPathIsRandom;

    public GlassLineManager glassLineManager;
    public GlassLine lastGlassLine;
    List<GlassLine> memoryGlassLine = new List<GlassLine>();
    List<int> memoryInt = new List<int>();


    public Transform EndObject;

    private Coroutine choicePathCoroutine; //my co-routine

    void Start()
    {
        path = new NavMeshPath();
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        this.StartCoroutine(ChoicePath(), ref choicePathCoroutine);
    }
    void LateUpdate()
    {
        if (moveDown)
            transform.Translate(Vector3.down * 3f * Time.deltaTime, Space.World);

        if (goPortal)
        {
            // Set Destination portal
            agentMover.SetDestination(EndObject.position);
        }

        // just debug
        if (path.corners.Length > 0)
        {
            for (int i = 0; i < path.corners.Length - 1; i++)
                Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
        }
    }

    private IEnumerator ChoicePath()
    {
        navMeshAgent.updateRotation = false;

        yield return new WaitForSeconds(Random.Range(0.2f, 0.4f));


        if (lastGlassLine)
        {
            if (lastGlassLine == glassLineManager.glassLines[glassLineManager.glassLines.Count - 1])
            {
                // Set Destination portal
                navMeshAgent.updateRotation = true;
                goPortal = true;

                this.TryStopCoroutine(ref choicePathCoroutine);
                yield return null;
            }
        }

        int indexNextGlass = glassLineManager.glassLines.IndexOf(lastGlassLine) + 1;

        if ((player.position - transform.position).magnitude > maxDistanceToPlayer && Random.Range(0, 100) < 50)
        {
            navMeshAgent.updateRotation = true;
            GlassLine gl = glassLineManager.glassLines[indexNextGlass];
            Vector3 fakeGlassPos = Vector3.zero;
            for (int i = 0; i < gl.glasses.Length; i++)
            {
                if (gl.trueGlass != i)
                {
                    fakeGlassPos = gl.glasses[i].position;
                    break;
                }
            }
            agentMover.SetDestination(fakeGlassPos);

            this.TryStopCoroutine(ref choicePathCoroutine);
            yield return null;
        }


        //Check visible bots on next glass
        if (lastGlassLine == null)
        {
            if (glassLineManager.glassLines[0].lastKnownTime + 5 > Time.time)
            {
                UpdateMemory(glassLineManager.glassLines[0]);
            }
        }
        else if (!memoryGlassLine.Contains(glassLineManager.glassLines[indexNextGlass]))
        {
            if (glassLineManager.glassLines[indexNextGlass].lastKnownTime + 5 > Time.time)
            {
                UpdateMemory(glassLineManager.glassLines[indexNextGlass]);
            }
            else
            {
                if (waitColab == false)
                {
                    waitColab = true;
                    yield return new WaitForSeconds(Random.Range(1f, 3f));
                    this.RestartCoroutine(ChoicePath(), ref choicePathCoroutine);
                    yield return null;
                }
            }
        }
        waitColab = false;

        CalculatePath();

        float speedThinking = lastPathIsRandom ? 1f : 0.65f;

        yield return new WaitForSeconds(Random.Range(0.3f * speedThinking, 0.6f * speedThinking));

        if (path.corners.Length > 1)
        {
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                while (navMeshAgent.hasPath)
                {
                    yield return new WaitForSeconds(0.3f);
                }

                if (i != path.corners.Length && navMeshAgent.enabled == true)
                {
                    if (i >= 2)
                        speedThinking = 0.5f;

                    Vector3 target = path.corners[i + 1];

                    Vector3 targetPosToRot = new Vector3(target.x, transform.position.y, target.z);
                    transform.DOLookAt(targetPosToRot, 0.5f * speedThinking);


                    yield return new WaitForSeconds(Random.Range(0.6f * speedThinking, 1f * speedThinking));


                    agentMover.SetDestination(path.corners[i + 1]);
                }
            }
        }

        yield return new WaitForSeconds(Random.Range(1f, 1.8f));


        if (Random.Range(0, 100) < 66)
        {
            // move
            if (lastGlassLine)
            {
                float rfactor = Random.Range(-0.5f, 0.5f);
                Vector3 target = lastGlassLine.trueGlassCenter + (lastGlassLine.trueGlassBounds * rfactor);

                agentMover.SetDestination(target);
            }
        }

        if (Random.Range(0, 100) < 33)
        {
            // jump
            anim.SetTrigger("Jump");
        }



        this.RestartCoroutine(ChoicePath(), ref choicePathCoroutine);

    }
    void CalculatePath()
    {
        if (lastGlassLine == null)
        {
            if (memoryGlassLine.Count > 0)
            {
                // Go to first Line (true glass)
                //agentMover.SetDestination(GetGlassPos(memoryGlassLine[0], memoryInt[0], false));
                NavMesh.CalculatePath(transform.position, GetGlassPos(memoryGlassLine[0], memoryInt[0], false), NavMesh.AllAreas, path);
                lastPathIsRandom = false;
            }
            else
            {
                // Go to first Line (random glass)
                //agentMover.SetDestination(GetGlassPos(glassLineManager.glassLines[0], 0, true));
                NavMesh.CalculatePath(transform.position, GetGlassPos(glassLineManager.glassLines[0], 0, true), NavMesh.AllAreas, path);
                lastPathIsRandom = true;
            }
        }
        else
        {
            int nextGlassId = glassLineManager.glassLines.IndexOf(lastGlassLine) + 1;

            if (memoryGlassLine.Count >= nextGlassId + 1)
            {
                // Go to first Line (true glass)
                //agentMover.SetDestination(GetGlassPos(memoryGlassLine[nextGlassId], nextGlassId, false));
                NavMesh.CalculatePath(transform.position, GetGlassPos(memoryGlassLine[nextGlassId], nextGlassId, false), NavMesh.AllAreas, path);
                lastPathIsRandom = false;
            }
            else
            {
                // Go to first Line (random glass)
                //agentMover.SetDestination(GetGlassPos(glassLineManager.glassLines[nextGlassId], 0, true));
                NavMesh.CalculatePath(transform.position, GetGlassPos(glassLineManager.glassLines[nextGlassId], 0, true), NavMesh.AllAreas, path);
                lastPathIsRandom = true;
            }
        }
    }
    Vector3 GetGlassPos(GlassLine _GlassLine, int glassId, bool needRandom)
    {
        if (needRandom)
        {
            return _GlassLine.glasses[Random.Range(0, _GlassLine.glasses.Length)].position;
        }
        else
        {
            return _GlassLine.glasses[memoryInt[glassId]].position;
        }
    }


    public void Dead(float delay)
    {
        StartCoroutine(Dead_IE(delay));
    }
    private IEnumerator Dead_IE(float delay)
    {
        this.TryStopCoroutine(ref choicePathCoroutine);
        if (navMeshAgent.isOnOffMeshLink)
        {
            navMeshAgent.CompleteOffMeshLink();
        }
        navMeshAgent.enabled = false;
        moveDown = true;
        goPortal = false;

        yield return new WaitForSeconds(delay);

        Respawn();
    }
    void Respawn()
    {
        moveDown = false;
        waitColab = false;
        lastGlassLine = null;
        transform.position = startPos.position;
        navMeshAgent.enabled = true;

        this.RestartCoroutine(ChoicePath(), ref choicePathCoroutine);
    }



    void UpdateMemory(GlassLine glassLine)
    {
        int index = glassLineManager.glassLines.IndexOf(glassLine);

        if (memoryGlassLine.Count == index)
        {
            memoryGlassLine.Add(glassLine);
            memoryInt.Add(glassLine.trueGlass);
        }
        else if (memoryGlassLine.Count < index)
        {
            memoryGlassLine.Clear();
            memoryInt.Clear();

            for (int i = 0; i < index; i++)
            {
                GlassLine tempGlassLine = glassLineManager.glassLines[i];
                memoryGlassLine.Add(tempGlassLine);
                memoryInt.Add(tempGlassLine.trueGlass);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        GlassLine glassLine = other.GetComponent<GlassLine>();
        if (!glassLine)
            return;

        glassLine.lastKnownTime = Time.time;

        lastGlassLine = glassLine;

        // Update Memory
        UpdateMemory(glassLine);
    }
    void OnTriggerExit(Collider other)
    {
        GlassLine glassLine = other.GetComponent<GlassLine>();
        if (!glassLine)
            return;
    }
}
