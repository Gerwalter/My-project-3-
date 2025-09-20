using System.Collections;
using UnityEngine;

public class ChaseState : INPCState
{
    Coroutine chaseRoutine;

    public void Enter(PatrollingNPC npc)
    {
        chaseRoutine = npc.StartCoroutine(ChasePlayer(npc));
    }

    IEnumerator ChasePlayer(PatrollingNPC npc)
    {
        while (npc.IsPlayerVisible())
        {
            npc.FOVAgent.ViewAngle = 360;
            npc.lastSeenPosition = npc.player.transform.position;

            // Verificar distancia para "atrapar"
            float distanceToPlayer = Vector3.Distance(npc.transform.position, npc.lastSeenPosition);
            if (distanceToPlayer <= npc.captureRange) // Rango de captura
            {
                Debug.Log($"{npc.name} atrapó al jugador!");
                // Aquí en el futuro puedes restar vida al jugador

                npc.hasTriggered = true;
                // Cambiar estado de vuelta a patrulla
                npc.SwitchState(new PatrolState());
                yield break; // Termina la corrutina correctamente
            }

            // Rotar hacia el jugador
            Vector3 dir = (npc.lastSeenPosition - npc.transform.position).normalized;
            dir.y = 0;

            npc.transform.rotation = Quaternion.Slerp(
                npc.transform.rotation,
                Quaternion.LookRotation(dir),
                5f * Time.deltaTime
            );

            // Mover hacia el jugador
            npc.transform.position += dir * npc.chaseSpeed * Time.deltaTime;
            yield return null;
        }

        npc.SwitchState(new PatrolState());
    }



    public void Update(PatrollingNPC npc) { }

    public void Exit(PatrollingNPC npc)
    {
        npc.StopCoroutine(npc.FollowPath());
        npc.StopCoroutine(npc.PatrolRoutine());
        if (chaseRoutine != null)
            npc.StopCoroutine(chaseRoutine);
    }
}
