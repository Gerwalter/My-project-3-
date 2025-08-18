using System.Collections.Generic;
using UnityEngine;

public static class NPCAlertSystem
{
    public static List<PatrollingNPC> registeredNPCs = new List<PatrollingNPC>();
    private static bool isPlayerSpotted = false;
    private static Vector3 lastKnownPosition;

    public static void RegisterNPC(PatrollingNPC npc)
    {
        if (!registeredNPCs.Contains(npc))
            registeredNPCs.Add(npc);
    }

    public static void UnregisterNPC(PatrollingNPC npc)
    {
        if (registeredNPCs.Contains(npc))
            registeredNPCs.Remove(npc);
    }

    public static void AlertAll(Vector3 playerPosition)
    {
        isPlayerSpotted = true;
        lastKnownPosition = playerPosition;

        foreach (var npc in registeredNPCs)
        {
            npc.OnPlayerSpotted(playerPosition);
        }
    }

    public static void ClearAlert()
    {
        isPlayerSpotted = false;

        foreach (var npc in registeredNPCs)
        {
            npc.OnAlertCleared();
        }
    }

    public static bool IsPlayerSpotted() => isPlayerSpotted;
    public static Vector3 GetLastKnownPosition() => lastKnownPosition;
}
