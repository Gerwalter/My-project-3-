using System.Collections;
using UnityEngine;

public class InvestigateState : NPCBaseState
{
    protected override IEnumerator StartMainRoutine(PatrollingNPC npc)
    {
        Node startNode = npc.GetClosestNode();
        Node targetNode = npc.GetNodeAtPosition(npc.lastSeenPosition);

        if (targetNode == null)
        {
            npc.SwitchState(new PatrolState());
            yield break;
        }

        npc.GeneratePath(startNode, targetNode);

        if (npc.currentPath == null || npc.currentPath.Count == 0)
        {
            npc.SwitchState(new PatrolState());
            yield break;
        }

        // Seguir el camino hasta el punto
        yield return npc.StartCoroutine(npc.FollowPath());

        // Determinar duracion: si fue una distraccion, usar la duracion especial
        float timer = 0f;
        float maxDuration = npc.heardDistraction ? npc.distractionInvestigateDuration : npc.investigateDuration;

        // resetear la marca (ya fue usada)
        npc.heardDistraction = false;

        while (timer < maxDuration)
        {
            if (npc.IsPlayerVisible())
            {
                npc.SwitchState(new ChaseState());
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // Volver a patrulla
        npc.SwitchState(new PatrolState());
    }
}
