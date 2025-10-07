using System.Collections;
using UnityEngine;

public class PatrolState : NPCBaseState
{
    protected override IEnumerator StartMainRoutine(PatrollingNPC npc)
    {
        npc.StartPatrolRoutine();

        while (true)
        {
            if (npc.IsPlayerVisible())
            {
                npc.lastSeenPosition = npc.player.transform.position;
                npc.SwitchState(new ChaseState());
                yield break;
            }

            yield return null;
        }
    }

    public override void Exit(PatrollingNPC npc)
    {
        base.Exit(npc);
        npc.StopPatrolRoutine();
    }
}
