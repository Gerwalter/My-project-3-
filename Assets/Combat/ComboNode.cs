using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ComboNode", menuName = "Combat/Combo Node")]
[System.Serializable]
public class ComboNode : ScriptableObject
{
    public string nodeName;
    public AnimationClip animationClip; // el clip asociado al nodo

    [System.Serializable]
    public class ComboTransition
    {
        public ComboInput input;
        public ComboNode nextNode;
        public string comboID;
    }

    public List<ComboTransition> transitions = new List<ComboTransition>();

    public ComboNode GetNextNode(ComboInput input)
    {
        foreach (var transition in transitions)
        {
            if (transition.input == input)
            {
                return transition.nextNode;
            }
        }
        return null;
    }
}
