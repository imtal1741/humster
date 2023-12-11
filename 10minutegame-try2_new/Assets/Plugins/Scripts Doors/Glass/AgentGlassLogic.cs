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
    bool moveDown = false;

    NavMeshPath path;

    public GlassLineManager glassLineManager;
    public GlassLine lastGlassLine;
    public List<GlassLine> memoryGlassLine = new List<GlassLine>();
    public List<int> memoryInt = new List<int>();


    public Transform followObject;

    void Start()
    {
        path = new NavMeshPath();
        startPos = transform.position;
        navMeshAgent = GetComponent<NavMeshAgent>();

        StartCoroutine(ChoicePath());
    }
    void LateUpdate()
    {
        if (moveDown)
            transform.Translate(Vector3.down * 3f * Time.deltaTime, Space.World);

        if (path.corners.Length > 0)
        {
            for (int i = 0; i < path.corners.Length - 1; i++)
                Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
        }
    }

    private IEnumerator ChoicePath()
    {

        CalculatePath();

        yield return new WaitForSeconds(Random.Range(0.3f, 0.6f));

        if (path.corners.Length > 1)
        {
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                while (navMeshAgent.hasPath)
                {
                    Debug.Log("hasPath");
                    yield return new WaitForSeconds(0.5f);
                }

                if (i != path.corners.Length && navMeshAgent.enabled == true)
                {
                    Vector3 target = path.corners[i + 1];

                    Vector3 targetPosToRot = new Vector3(target.x, transform.position.y, target.z);
                    transform.DOLookAt(targetPosToRot, 0.5f);


                    yield return new WaitForSeconds(Random.Range(0.6f, 1f));


                    agentMover.SetDestination(path.corners[i + 1]);

                    yield return new WaitForSeconds(0.5f);
                }
            }
        }



    }
    void CalculatePath()
    {
        if (lastGlassLine == null)
        {
            if (memoryGlassLine.Count > 0)
            {
                // Go to first Line (true glass)
                agentMover.SetDestination(GetGlassPos(memoryGlassLine[0], memoryInt[0], false));
            }
            else
            {
                // Go to first Line (random glass)
                //agentMover.SetDestination(GetGlassPos(glassLineManager.glassLines[0], 0, true));
                NavMesh.CalculatePath(transform.position, GetGlassPos(glassLineManager.glassLines[0], 0, true), NavMesh.AllAreas, path);
            }
        }
        else
        {
            int nextGlassId = glassLineManager.glassLines.IndexOf(lastGlassLine) + 1;
            if (nextGlassId > glassLineManager.glassLines.Count)
            {
                // Set Destination portal
                //return;
            }


            if (memoryGlassLine.Count >= nextGlassId)
            {
                // Go to first Line (true glass)
                agentMover.SetDestination(GetGlassPos(memoryGlassLine[nextGlassId], nextGlassId, false));
            }
            else
            {
                // Go to first Line (random glass)
                agentMover.SetDestination(GetGlassPos(memoryGlassLine[nextGlassId], 0, true));
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
        if (navMeshAgent.isOnOffMeshLink)
        {
            navMeshAgent.CompleteOffMeshLink();
        }
        navMeshAgent.enabled = false;
        lastGlassLine = null;
        moveDown = true;

        yield return new WaitForSeconds(2f);

        Respawn();
    }
    void Respawn()
    {
        moveDown = false;
        transform.position = startPos;
        navMeshAgent.enabled = true;
    }



    void OnTriggerEnter(Collider other)
    {
        GlassLine glassLine = other.GetComponent<GlassLine>();
        if (!glassLine)
            return;

        // Caching
        GlassLineManager gl_Manager = glassLine.glassLineManager;
        int index = gl_Manager.glassLines.IndexOf(glassLine);



        lastGlassLine = glassLine;

        // Update Memory
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
                GlassLine tempGlassLine = gl_Manager.glassLines[i];
                memoryGlassLine.Add(tempGlassLine);
                memoryInt.Add(tempGlassLine.trueGlass);
            }
        }




    }
}
