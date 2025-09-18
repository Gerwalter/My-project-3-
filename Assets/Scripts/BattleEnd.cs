using UnityEngine;

public class BattleEnd : MonoBehaviour
{
    public void OnBattleWon()
    {
        BattleManager.Instance.EndBattle();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) 
            {
            OnBattleWon();
            }
    }
}
