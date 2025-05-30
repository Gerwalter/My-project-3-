using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ComboNode", menuName = "Combat/Combo Node")]
public class ComboNode : ScriptableObject
{
    public string nodeName;
    [System.Serializable]
    public class ComboTransition
    {
        public ComboInput input;
        public ComboNode nextNode;
        public string comboID;
       // public bool unlockByDefault = false;
    }

    public List<ComboTransition> transitions = new List<ComboTransition>();

    public ComboNode GetNextNode(ComboInput input)
    {
        foreach (var transition in transitions)
        {
            if (transition.input == input)
             //   && ComboUnlockManager.Instance.IsUnlocked(transition.comboID))
            {
                return transition.nextNode;
            }
        }
        return null;
    }
}
