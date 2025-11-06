using UnityEngine;
using System.Collections;
public class ChaseState : NPCBaseState
{
    protected override IEnumerator StartMainRoutine(PatrollingNPC npc)
    {
        npc.FOVAgent.ViewAngle = 360f;
        npc.agent.speed = npc.chaseSpeed;
        npc.agent.updateRotation = false;

        // Resetear exposición al iniciar persecución
        npc.currentExposure = 0f;

        while (true)
        {
            float distance = Vector3.Distance(npc.transform.position, npc.player.transform.position);
            if (distance <= npc.captureRange)
            {
                npc.hasTriggered = true;
                npc.SwitchState(new PatrolState());
                yield break;
            }

            // Si pierde de vista Y exposición no está llena, vuelve a investigar
            if (!npc.IsPlayerVisible() && !npc.isExposureFull)
            {
                npc.SwitchState(new InvestigateState());
                yield break;
            }

            npc.lastSeenPosition = npc.player.transform.position;
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
    }
}