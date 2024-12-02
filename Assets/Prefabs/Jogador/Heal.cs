using UnityEngine;

public class Heal : UltimateAbility
{
    // Start is called before the first frame update

    [SerializeField] private Player _player;

    private void Awake()
    {
        _player = GameManager.Instance.Player;
    }
    void Start()

    {
        Ability();
    }

    public override void Ability()
    {
        _player.Health(5);

        Destroy(gameObject, 2);
    }
}
