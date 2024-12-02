using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateCombo: UltimateAbility
{

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
        _player._anim.SetTrigger("Ultimate");

        Destroy(gameObject, 2);
    }
}
