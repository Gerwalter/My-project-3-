using UnityEngine;

public class BattleEnd : MonoBehaviour
{
    public void OnBattleWon()
    {
        BattleManager.Instance.EndBattle();
    }
}
