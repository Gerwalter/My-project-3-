using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DMGBoost : UltimateAbility
{
    [SerializeField] private PlayerAttack _player;

    private void Awake()
    {
        //_player = GameManager.Instance.Player;
    }
    void Start()

    {
        Ability();
    }

    public override void Ability()
    {
        _player.DMGBooster();
        Destroy(gameObject, 2);
    }
}
