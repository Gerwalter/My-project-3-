using UnityEngine;
using System.Collections;

public class PatrolState : INPCState
{
    Coroutine patrolRoutine;

    public void Enter(PatrollingNPC npc)
    {
        npc.hasBeenAlerted = false;
        patrolRoutine = npc.StartCoroutine(npc.PatrolRoutine());
    }
    public void Update(PatrollingNPC npc)
    {
        if (npc.IsPlayerVisible())
        {
            Debug.Log("AAA");
            npc.lastSeenPosition = npc.player.transform.position;
            npc.SwitchState(new ChaseState());
        }
    }

    public void Exit(PatrollingNPC npc)
    {
        if (patrolRoutine != null)
            npc.StopCoroutine(patrolRoutine);
    }
}
