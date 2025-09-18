using UnityEngine;

public class BattleEnd : MonoBehaviour
{
    // Ejemplo: llamar esta funci�n cuando todos los enemigos est�n muertos
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