using System.Collections;
using UnityEngine;

public class ChaseState : NPCBaseState
{
    protected override IEnumerator StartMainRoutine(PatrollingNPC npc)
    {
        npc.FOVAgent.ViewAngle = 360f;
        npc.agent.speed = npc.chaseSpeed;  // Aumenta velocidad
        npc.agent.updateRotation = false;  // Rotación manual para persecución

        while (npc.IsPlayerVisible())
        {
            npc.lastSeenPosition = npc.player.transform.position;

            float distance = Vector3.Distance(npc.transform.position, npc.lastSeenPosition);
            if (distance <= npc.captureRange)
            {
                npc.hasTriggered = true;
                npc.SwitchState(new PatrolState());
                yield break;
            }

            // Actualiza destino cada frame (persecución dinámica)
            npc.agent.SetDestination(npc.lastSeenPosition);

            // Rotación manual
            Vector3 dir = (npc.lastSeenPosition - npc.transform.position).normalized;
            dir.y = 0;
            npc.transform.rotation = Quaternion.Slerp(
                npc.transform.rotation,
                Quaternion.LookRotation(dir),
                5f * Time.deltaTime
            );

            yield return null;
        }

        npc.agent.speed = npc.moveSpeed;  // Vuelve a velocidad normal
        npc.agent.updateRotation = true;
        npc.SwitchState(new InvestigateState());
    }
}