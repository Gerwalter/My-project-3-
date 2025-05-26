using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionNode : Node
{
    public Node trueNode;
    public Node falseNode;
    public TypeQuestion questionType;


    public override void Test(Patranger npc)
    {
        switch (questionType) 
        {
            case TypeQuestion.Steal:
                if (npc.lupinranger.IsStealing)
                {
                    trueNode.Test(npc);
                }
                else
                {
                    falseNode.Test(npc);
                }
                break;
            case TypeQuestion.Distance:
                if (Vector3.Distance(npc.lupinranger.transform.position, npc.transform.position) < 2)
                {
                    trueNode.Test(npc);
                }
                else
                {
                    falseNode.Test(npc);
                }
                break;
            case TypeQuestion.Armed:
                if (npc.lupinranger.IsArmed) 
                {
                    trueNode.Test(npc);
                }
                else
                {
                    falseNode.Test(npc);
                }
                break;
        }
    }
}

public enum TypeQuestion
{
    Steal, Distance, Armed
}