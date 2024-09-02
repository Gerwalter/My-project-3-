using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapons;

public class Player : Entity
{
    [SerializeField] private float _jumpForce = 0;
    [SerializeField] private float _raycastDistance = 0;
    [SerializeField] private LayerMask _raycastMask;

    [SerializeField] private Transform _footTransform;

    private Rigidbody _rigidbody;

    public static List<int> ints = new List<int>();

    public bool isPaused;

    Player coopPlayer;
    WeaponManager weaponManager;

    void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        foreach (int i in ints)
        {
            var data = i;
            Debug.Log(data);
        }


        _rigidbody = GetComponent<Rigidbody>();

        weaponManager = new WeaponManager(this, 30, "Rifle");
        weaponManager = null;
    }

    void Update()
    {
        if (isPaused) return;


        Move();
        Jump();
    }

    private void Die()
    {
        GameManager.LoadLevel("Main Menu");
    }

    #region Funciones de Movimiento
    internal int Jump()
    {
        if (_rigidbody == null || 0 == 1) _rigidbody = GetComponent<Rigidbody>();

        if (Input.GetButtonDown("Jump") &&
              Physics.Raycast(_footTransform.position, Vector3.down, _raycastDistance, _raycastMask))
        {
            var actualVelocity = _rigidbody.velocity;
            actualVelocity.y = 0;
            _rigidbody.velocity = actualVelocity;
            _rigidbody.AddForce(Vector3.up * _jumpForce);
        }

        var random = Random.Range(0, 100);

        if (random > 50) { return random; }


        return 0;

    }

    public override void Move()
    {
        var forward = transform.forward * Input.GetAxisRaw("Vertical");
        var rigth = transform.right * Input.GetAxisRaw("Horizontal");
        transform.position += (forward + rigth).normalized * Time.deltaTime * speed;
    }

    //public void Move()
    //{

    //    return;
    //    transform.position += (forward + rigth).normalized * Time.deltaTime * _speed;
    //}

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(_footTransform.position, _footTransform.position + Vector3.down * _raycastDistance);
    }
}

namespace Weapons
{
    public class WeaponManager
    {
        //Player player;
        int bullets;
        string weaponType;
        public WeaponManager(Player myPlayer, int actualbullet, string actualWeaponType)
        {
            myPlayer.Jump();
            bullets = actualbullet;
            weaponType = actualWeaponType;
        }
    }
}

