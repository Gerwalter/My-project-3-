using UnityEngine;

public class BattleEnd : MonoBehaviour
{
    // Ejemplo: llamar esta función cuando todos los enemigos estén muertos
    public void OnBattleWon()
    {
        BattleManager.Instance.EndBattle();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            OnBattleWon();
        }
    }
}