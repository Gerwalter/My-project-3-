using System.Collections;
using UnityEngine;

public class ChaseState : NPCBaseState
{
    protected override IEnumerator StartMainRoutine(PatrollingNPC npc)
    {
        npc.FOVAgent.ViewAngle = 360f;

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

            Vector3 dir = (npc.lastSeenPosition - npc.transform.position).normalized;
            dir.y = 0;
            npc.transform.rotation = Quaternion.Slerp(
                npc.transform.rotation,
                Quaternion.LookRotation(dir),
                5f * Time.deltaTime
            );

            npc.transform.position += dir * npc.chaseSpeed * Time.deltaTime;
            yield return null;
        }

        npc.SwitchState(new InvestigateState());
    }
}
