using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class AgentGlassLogic : MonoBehaviour
{

    public AgentMover agentMover;
    Vector3 startPos;
    NavMeshAgent navMeshAgent;
    Animator anim;
    [HideInInspector] public bool moveDown = false;
    public bool waitColab = false;

    NavMeshPath path;
    bool lastPathIsRandom;

    public GlassLineManager glassLineManager;
    public GlassLine lastGlassLine;
    public List<GlassLine> memoryGlassLine = new List<GlassLine>();
    public List<int> memoryInt = new List<int>();


    public Transform EndObject;

    private Coroutine choicePathCoroutine; //my co-routine

    void Start()
    {
        path = new NavMeshPath();
        startPos = transform.position;
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        this.StartCoroutine(ChoicePath(), ref choicePathCoroutine);
    }
    void LateUpdate()
    {
        if (moveDown)
            transform.Translate(Vector3.down * 3f * Time.deltaTime, Space.World);


        // just debug
        if (path.corners.Length > 0)
        {
            for (int i = 0; i < path.corners.Length - 1; i++)
                Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
        }
    }

    private IEnumerator ChoicePath()
    {
        yield return new WaitForSeconds(Random.Range(0.2f, 0.4f));


        //if (lastGlassLine)
        //{
        //    if (lastGlassLine == glassLineManager.glassLines[glassLineManager.glassLines.Count - 1])
        //    {
        //        // Set Destination portal
        //        //return;
        //    }
        //}


        //Check visible bots on next glass
        if (lastGlassLine == null)
        {
            if (glassLineManager.glassLines[0].lastKnownTime + 5 > Time.time)
            {
                UpdateMemory(glassLineManager.glassLines[0]);
            }
        }
        else
        {
            int index = glassLineManager.glassLines.IndexOf(lastGlassLine) + 1;

            if (glassLineManager.glassLines[index].lastKnownTime + 5 > Time.time)
            {
                UpdateMemory(glassLineManager.glassLines[index]);
            }
            else
            {
                if (waitColab == false)
                {
                    waitColab = true;
                    yield return new WaitForSeconds(Random.Range(2f, 5f));
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

        Debug.Log("Complete Path");


        int r = Random.Range(0, 100);
        if (r < 50)
        {
            // move
            if (lastGlassLine)
            {
                float rfactor = Random.Range(-0.8f, 0.8f);
                Vector3 target = lastGlassLine.trueGlassCenter + (lastGlassLine.trueGlassBounds * rfactor);

                if (Random.Range(0, 100) < 50)
                {
                    Vector3 targetPosToRot = new Vector3(target.x, transform.position.y, target.z);
                    transform.DOLookAt(targetPosToRot, 0.3f);
                    yield return new WaitForSeconds(Random.Range(0.2f, 0.3f));
                }


                agentMover.SetDestination(target);
            }
        }
        else if (r < 75)
        {
            // jump
            anim.SetTrigger("Jump");
        }
        else if (r < 90)
        {
            // look other

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 4);
            foreach (var hitCollider in hitColliders)
            {
                AgentGlassLogic otherAgent = hitCollider.GetComponent<AgentGlassLogic>();
                if (otherAgent)
                {
                    Vector3 target = otherAgent.transform.position;
                    Vector3 targetPosToRot = new Vector3(target.x, transform.position.y, target.z);
                    transform.DOLookAt(targetPosToRot, 0.3f);
                    yield return new WaitForSeconds(Random.Range(0.6f, 1.2f));
                    target = EndObject.position;
                    targetPosToRot = new Vector3(target.x, transform.position.y, target.z);
                    transform.DOLookAt(targetPosToRot, 0.3f);
                    yield return new WaitForSeconds(0.3f);
                    break;
                }
            }
        }


        // Wait after jump
        yield return new WaitForSeconds(0.4f);

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


    public void Dead()
    {
        StartCoroutine(Dead_IE());
    }
    private IEnumerator Dead_IE()
    {
        this.TryStopCoroutine(ref choicePathCoroutine);
        if (navMeshAgent.isOnOffMeshLink)
        {
            navMeshAgent.CompleteOffMeshLink();
        }
        navMeshAgent.enabled = false;
        moveDown = true;

        yield return new WaitForSeconds(2f);

        Respawn();
    }
    void Respawn()
    {
        moveDown = false;
        waitColab = false;
        lastGlassLine = null;
        transform.position = startPos;
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
