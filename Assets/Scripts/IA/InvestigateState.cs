using System.Collections;
using UnityEngine;

public class InvestigateState : INPCState
{
    Coroutine investigateRoutine;

    public void Enter(PatrollingNPC npc)
    {
        investigateRoutine = npc.StartCoroutine(Investigate(npc));
    }

    IEnumerator Investigate(PatrollingNPC npc)
    {
        Node startNode = npc.GetClosestNode();
        Node targetNode = npc.GetNodeAtPosition(npc.lastSeenPosition);

        if (targetNode == null)
        {
            Debug.LogWarning($"{npc.name} no encontró un nodo válido para investigar.");
            npc.SwitchState(new PatrolState());
            yield break;
        }

        npc.GeneratePath(startNode, targetNode);

        if (npc.currentPath == null || npc.currentPath.Count == 0)
        {
            Debug.LogWarning($"{npc.name} no tiene un camino válido hacia el punto de investigación.");
            npc.SwitchState(new PatrolState());
            yield break;
        }

        yield return npc.StartCoroutine(npc.FollowPath());

        float timer = 0f;
        while (timer < npc.investigateDuration)
        {
            if (npc.IsPlayerVisible())
            {
                npc.SwitchState(new ChaseState());
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        npc.SwitchState(new PatrolState());
    }

    public void Update(PatrollingNPC npc) { }

    public void Exit(PatrollingNPC npc)
    {
        if (investigateRoutine != null)
            npc.StopCoroutine(investigateRoutine);
    }
}
