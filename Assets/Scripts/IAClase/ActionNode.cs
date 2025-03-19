using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class ActionNode : Node
{
    public TypeAction actionType;

    public override void Test(Patranger npc)
    {
        switch (actionType)
        {
            case TypeAction.Patrol:
                Debug.Log("A");
                break;
            case TypeAction.Chase:
                var dir = npc.lupinranger.transform.position - npc.transform.position;
                npc.transform.position += dir.normalized * npc.Speed * Time.deltaTime;
                break;
            case TypeAction.Fight:
                if (npc.lupinranger.IsArmed)
                {
                    Debug.Log("By the Power bested in us by the Global Police we will handle you by force!");
                }
                break;
            case TypeAction.Curse:
                Debug.Log("Damn you Lupinranger!");
                break;
        }
    }
}
public enum TypeAction
{
    Patrol, Chase, Fight, Curse
}