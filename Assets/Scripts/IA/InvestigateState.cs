using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class InvestigateState : NPCBaseState
{
    protected override IEnumerator StartMainRoutine(PatrollingNPC npc)
    {
        Vector3 targetPos = npc.heardDistraction ? npc.lastHeardPosition : npc.lastSeenPosition;

        // Validar posicion en NavMesh
        if (!NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            npc.SwitchState(new PatrolState());
            yield break;
        }
        targetPos = hit.position;

        // Mover al punto
        yield return npc.StartCoroutine(npc.MoveToRoutine(targetPos));

        // Si es moneda, buscarla y "recogerla"
        if (npc.isCoinDistraction)
        {
            Collider[] hits = Physics.OverlapSphere(npc.transform.position, npc.FOVAgent.ViewRange, npc.distractionLayer);
            foreach (var hita in hits)
            {
                DistractionObject coin = hita.GetComponent<DistractionObject>();
                if (coin != null && coin.distractionType == DistractionType.Coin)
                {
                    coin.Dest();
                    npc.isCoinDistraction = false; // Cambio: Resetea inmediatamente para evitar loops
                    break;
                }
            }
        }

        // Determinar duracion
        float timer = 0f;
        float maxDuration = npc.isCoinDistraction ? npc.distractionInvestigateDuration : npc.investigateDuration;

        // Resetear marcas
        npc.heardDistraction = false;
        npc.isCoinDistraction = false; // Movido aquí si no se destruyó, pero ya reseteado arriba

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