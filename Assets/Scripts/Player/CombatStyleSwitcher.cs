using UnityEngine;

public class CombatStyleSwitcher : MonoBehaviour
{

    public ComboNode aggressiveStyle;
    public ComboNode defensiveStyle;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EventManager.Trigger("ComboChanger", aggressiveStyle, 1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EventManager.Trigger("ComboChanger", defensiveStyle, 1);
        }
    }

}
