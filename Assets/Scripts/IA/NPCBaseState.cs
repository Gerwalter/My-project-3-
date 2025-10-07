using System.Collections;
using UnityEngine;

public abstract class NPCBaseState : INPCState
{
    private Coroutine runningRoutine;

    public virtual void Enter(PatrollingNPC npc)
    {
        IEnumerator routine = StartMainRoutine(npc);
        if (routine != null)
            runningRoutine = npc.StartCoroutine(routine);
    }

    // Las subclases devuelven una rutina IEnumerator
    protected virtual IEnumerator StartMainRoutine(PatrollingNPC npc) => null;

    public virtual void Update(PatrollingNPC npc) { }

    public virtual void Exit(PatrollingNPC npc)
    {
        if (runningRoutine != null)
        {
            npc.StopCoroutine(runningRoutine);
            runningRoutine = null;
        }
    }
}
