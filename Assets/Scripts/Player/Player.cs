using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class Player : HP
{
    [Header("<color=#6A89A7>Animation</color>")]
    [SerializeField] private string _isMovName = "isMoving";
    [SerializeField] private string _isGroundName = "isGrounded";
    [SerializeField] private string _jumpName = "onJump";
    [SerializeField] private string _xName = "xAxis";
    [SerializeField] private string _zName = "zAxis";
    public Animator _anim;

    [Header("<color=#6A89A7>Camera</color>")]
    [SerializeField] private Transform _camTarget;
    [SerializeField] private FireShader FireShader;

    public Transform GetCamTarget { get { return _camTarget; } }
    public Vector3 _camForwardFix = new(), _camRightFix = new();
    protected Transform _camTransform;

    [Header("<color=#6A89A7>Inputs</color>")]
    [SerializeField] private KeyCode _intKey = KeyCode.F;
    [SerializeField] private KeyCode _jumpKey = KeyCode.Space;

    [Header("<color=#6A89A7>UI</color>")]
    [SerializeField] private Image healthBar;
    [SerializeField] private Image ultimateBar;
    [SerializeField] private Image overchardedBar;

    [Header("<color=#6A89A7>Physics - Interaction</color>")]
    [SerializeField] private Transform _intOrigin;
    [SerializeField] private float _intRayDist = 1.0f;
    [SerializeField] private LayerMask _intMask;
    [SerializeField] private Ray _intRay;
    private RaycastHit _intHit;
    public float _sphereIntRadius = 0.5f; // Radio de la esfera

    [Header("<color=#6A89A7>Physics - Jumping</color>")]
    [SerializeField] private float _jumpForce = 5.0f;
    [SerializeField] private float _jumpRayDist = 0.75f;
    [SerializeField] private LayerMask _jumpMask;
    public Vector3 _jumpOffset = new();
    [SerializeField] private Ray _jumpRay;

    [Header("<color=#6A89A7>Physics - Movement</color>")]
    [SerializeField] private float _movRayDist = 0.75f;
    [SerializeField] private LayerMask _movMask;
    [SerializeField] private float _movSpeed = 3.5f;
    public Vector3 _dir = new(), _movRayDir = new();
    private Vector3 _dirFix = new();
    [SerializeField] private Ray _movRay;

    [Header("<color=yellow>Attack</color>")]
    [SerializeField] private Transform _atkOrigin;
    [SerializeField] private float _atkRayDist = 1.0f;
    [SerializeField] private LayerMask _atkMask;
    [SerializeField] private int _atkDmg = 20;
    private Ray _atkRay;
    private RaycastHit _atkHit;
    [SerializeField] private float originaldmg;
    [SerializeField] private int dmgMultiplier = 2; // Multiplicador de velocidad
    public float _sphereAtkRadius = 0.5f;
    [SerializeField] private ElementType selectedElement;
    [SerializeField] private float _ultimateCharge;


    [Header("<color=#6A89A6>Physics - Speed</color>")]
    [SerializeField] private float originalSpeed;
    [SerializeField] private float speedMultiplier = 2.0f;
    [SerializeField] private float duration = 5.0f;




    private Rigidbody _rb;

    [Header("<color=red>Misc</color>")]
    [SerializeField] private VisualEffect _fire;
    [SerializeField] private GameObject gameObje;
    [SerializeField] private Lock Handle;


    [Header("<color=blue>Grapple</color>")]
    public bool groundCheck;
    public Grappling grapple;
    public bool EnableMovementAfterCollision = true;
    [SerializeField] public bool freeze = false;
    [SerializeField] public bool activeGrapple = false;
    [SerializeField] private Vector3 velocityToSet;
    [SerializeField] private float velocity;
    [SerializeField] private float maxGrappleVelocity = 20f;

    // Nueva variable para desactivar el movimiento


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeRotation;

       // GameManager.Instance.Player = this;


    }

    private void Start()
    {
        _camTransform = Camera.main.transform;
       // healthBar = CanvasReferencesManager.Instance.Healthbar;
       // ultimateBar = CanvasReferencesManager.Instance.Ultimate;
       // overchardedBar = CanvasReferencesManager.Instance.Overcharge;
       // Handle = CanvasReferencesManager.Instance.Handle;
        _anim = GetComponentInChildren<Animator>();

        GetLife = maxLife;
        UpdateHealthBar();
       // FireShader.DeactivateFire();
    }
    private void Update()
    {
        ElementalCast();
        if (freeze)
        {
            _anim.SetFloat(_xName, 0.0f);
            _anim.SetFloat(_zName, 0.0f);

            return;
        }
        _dir.x = Input.GetAxisRaw("Horizontal");
        _dir.z = Input.GetAxisRaw("Vertical");

        if (!IsBlocked(_dir.x, _dir.z))
        {
            _anim.SetFloat(_xName, _dir.x);
            _anim.SetFloat(_zName, _dir.z);
        }
        else
        {
            _anim.SetFloat(_xName, 0.0f);
            _anim.SetFloat(_zName, 0.0f);
        }

        _anim.SetBool(_isGroundName, IsGrounded());
        _anim.SetBool(_isMovName, _dir.x != 0 || _dir.z != 0);

        if (Input.GetKeyDown(_intKey))
        {
            _anim.SetTrigger("Int");
        }

        if (Input.GetKeyDown(_jumpKey) && IsGrounded())
        {
            _anim.SetTrigger(_jumpName);
            Jump();
        }

        UpdateHealthBar();
        groundCheck = IsGrounded();
        Dash();
        Ultimate(true);
        UseUltimate();
    }

    void Dash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && _canDash)
        {
            if (_dashResetCoroutine != null)
            {
                StopCoroutine(_dashResetCoroutine); // Detenemos el reset si hay un nuevo dash
            }
            StartCoroutine(Dashing());
        }
    }

    [Header("<color=red>Dash</color>")]
    [SerializeField] private float _dashForce = 10f; // Fuerza del dash
    [SerializeField] private float _dashCooldown = 0.2f; // Cooldown entre dashes consecutivos
    [SerializeField] private float _dashResetTime = 0.5f; // Tiempo máximo entre dashes consecutivos
    [SerializeField] private float _dashFinalCooldown = 1f; // Cooldown después del tercer dash
    [SerializeField] private float _dashDuration = 0.2f; // Duración de cada dash

    private int _dashCount = 0; // Contador de dashes consecutivos
    private bool _canDash = true; // Indica si el jugador puede iniciar un dash
    private bool _isDashing = false;
    private Vector3 _dashDirection;
    private Coroutine _dashResetCoroutine;

    IEnumerator Dashing()
    {
        _isDashing = true;
        _canDash = false;

        // Guardar la dirección del dash
        _dashDirection = _dirFix;

        float dashTime = 0f;
        while (dashTime < _dashDuration)
        {
            _rb.MovePosition(transform.position + _dashDirection * _dashForce * Time.deltaTime);
            dashTime += Time.deltaTime;
            yield return null;
        }

        _isDashing = false;
        print(_dashCount);
        _dashCount++;

        // Comprobamos si es el tercer dash
        if (_dashCount >= 3)
        {
            _dashCount = 0; // Reset del contador
            yield return new WaitForSeconds(_dashFinalCooldown); // Espera el cooldown largo
        }
        else
        {
            _dashResetCoroutine = StartCoroutine(DashResetTimer()); // Inicia el temporizador de reset
            yield return new WaitForSeconds(_dashCooldown); // Cooldown entre dashes
        }

        _canDash = true; // Permitir realizar un nuevo dash
    }

    private IEnumerator DashResetTimer()
    {
        yield return new WaitForSeconds(_dashResetTime);
        _dashCount = 0; // Reset del contador de dashes
    }

    private void FixedUpdate()
    {
        if (freeze) return; // Si el movimiento está desactivado, no hacer nada

        if ((_dir.x != 0.0f || _dir.z != 0.0f) && !IsBlocked(_dir.x, _dir.z))
        {
            Movement(_dir);
        }
    }
    public void DisableMovement()
    {
        freeze = true;
    }

    public void EnableMovement()
    {
        freeze = false;
    }
    private void UpdateHealthBar()
    {
        float lifePercent = GetLife / maxLife;
        healthBar.fillAmount = lifePercent;
        healthBar.color = Color.Lerp(Color.red, Color.green, lifePercent);
    }



    public void Interact()
    {
        _intRay = new Ray(_intOrigin.position, transform.forward);

        if (Physics.SphereCast(_intRay, _sphereIntRadius, out _intHit, _intRayDist, _intMask))
        {
            if (_intHit.collider.TryGetComponent<ButtonBehaviour>(out ButtonBehaviour intObj))
            {
                intObj.OnInteract();
            }
        }
    }

    private IEnumerator ApplySpeedBoost()
    {
        originalSpeed = _movSpeed; // Guarda la velocidad original
        _movSpeed *= speedMultiplier; // Aplica el multiplicador

        yield return new WaitForSeconds(duration); // Espera el tiempo de duración

        _movSpeed = originalSpeed; // Restaura la velocidad original
    }
    public void SpeedBooster()
    {
        StartCoroutine(ApplySpeedBoost());
    }
    private void Jump()
    {
        _rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
    }

    private void Movement(Vector3 dir)
    {
        if (_isDashing) return;
        _camForwardFix = _camTransform.forward;
        _camRightFix = _camTransform.right;

        _camForwardFix.y = 0.0f;
        _camRightFix.y = 0.0f;

        Rotate(_camForwardFix);

        _dirFix = (_camRightFix * dir.x + _camForwardFix * dir.z).normalized;

        _rb.MovePosition(transform.position + _dirFix * _movSpeed * Time.fixedDeltaTime);
    }

    private void Rotate(Vector3 dir)
    {
        transform.forward = dir;
    }


    public bool IsGrounded()
    {
        _jumpOffset = new Vector3(transform.position.x, transform.position.y + 0.125f, transform.position.z);

        _jumpRay = new Ray(_jumpOffset, -transform.up);

        return Physics.Raycast(_jumpRay, _jumpRayDist, _jumpMask);
    }

    private bool IsBlocked(float x, float z)
    {
        _movRayDir = (transform.right * x + transform.forward * z);

        _movRay = new Ray(transform.position, _movRayDir);

        return Physics.Raycast(_movRay, _movRayDist, _movMask);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(_jumpRay);

        // Dibuja la línea del SphereCast
        Gizmos.color = Color.red; // Color del gizmo
        Gizmos.DrawLine(_intOrigin.position, _intOrigin.position + transform.forward * _intRayDist);

        // Dibuja la esfera al final del SphereCast
        Gizmos.color = Color.yellow; // Color de la esfera
        Gizmos.DrawWireSphere(_intRay.origin + _intRay.direction * _intRayDist, _sphereIntRadius);

        // Dibuja la línea del SphereCast
        Gizmos.color = Color.white; // Color del gizmo
        Gizmos.DrawLine(_atkOrigin.position, _atkOrigin.position + transform.forward * _atkRayDist);

        // Dibuja la esfera al final del SphereCast
        Gizmos.color = Color.green; // Color de la esfera
        Gizmos.DrawWireSphere(_atkRay.origin + _atkRay.direction * _atkRayDist, _sphereAtkRadius);
    }


    private IEnumerator ApplyDMGBoost()
    {
        originaldmg = _atkDmg; // Guarda la velocidad original
        _atkDmg *= dmgMultiplier; // Aplica el multiplicador

        yield return new WaitForSeconds(duration); // Espera el tiempo de duración

        _atkDmg = dmgMultiplier; // Restaura la velocidad original
    }
    public void DMGBooster()
    {
        StartCoroutine(ApplyDMGBoost());
    }
    public void Attack()
    {
        _atkRay = new Ray(_atkOrigin.position, transform.forward);

        if (Physics.SphereCast(_atkRay, _sphereAtkRadius, out _atkHit, _atkRayDist, _atkMask))
        {
            if (_atkHit.collider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.ReceiveDamage(_atkDmg, selectedElement);
            }
            else if (_atkHit.collider.TryGetComponent<HealthSystem>(out HealthSystem enemyHealth))
            {
                enemyHealth.ReceiveDamage(_atkDmg, selectedElement);
            }
            else if (_atkHit.collider.TryGetComponent<Boss>(out Boss BossHealth))
            {
                BossHealth.ReciveDamage(_atkDmg, selectedElement);
            }
            _ultimateCharge++;
        }

        else if (Physics.Raycast(_atkRay, out _atkHit, _atkRayDist, _atkMask))
        {
            if (_atkHit.collider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.ReceiveDamage(_atkDmg, selectedElement);
            }
            else if (_atkHit.collider.TryGetComponent<HealthSystem>(out HealthSystem enemyHealth))
            {
                enemyHealth.ReceiveDamage(_atkDmg, selectedElement);
            }
            else if (_atkHit.collider.TryGetComponent<Boss>(out Boss BossHealth))
            {
                BossHealth.ReciveDamage(_atkDmg, selectedElement);
            }
            _ultimateCharge++;
        }
    }

    public void PerformLiftAttack()
    {
        _atkRay = new Ray(_atkOrigin.position, transform.forward);

        if (Physics.Raycast(_atkRay, out _atkHit, _atkRayDist, _atkMask))
        {
            if (_atkHit.collider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.ReceiveDamage(_atkDmg, selectedElement);
                enemy.ApplyLiftImpulse();
            }
            else if (_atkHit.collider.TryGetComponent<HealthSystem>(out HealthSystem enemyHealth))
            {
                int randomValue = Random.Range(0, 101); // Incluye 0 y 100
                if (randomValue >= 20)
                {
                    enemyHealth.ApplyContinuousDamageFromPlayer(10f, 2.5f, selectedElement);
                }
                else
                {
                    enemyHealth.ReceiveDamage(_atkDmg, selectedElement);
                }

            }
            else if (_atkHit.collider.TryGetComponent<Boss>(out Boss BossHealth))
            {
                BossHealth.ReciveDamage(_atkDmg);
                BossHealth.ApplyLiftImpulse();
            }

            _ultimateCharge++;
        }
    }

    [SerializeField] private float maxUltimate;
    [SerializeField] private float maxOvercharge;

    [SerializeField] UltimateAbilities[] _ultimateAbilities;
    [SerializeField] private int selectedAbilityIndex = 0;

    public void Ultimate(bool isOvercharged = false)
    {
        maxOvercharge = maxUltimate * 2f;

        float currentMax = isOvercharged ? maxOvercharge : maxUltimate;

        _ultimateCharge = Mathf.Clamp(_ultimateCharge, 0, currentMax);

        float normalFillPercentage = Mathf.Clamp01(_ultimateCharge / maxUltimate);

        float overchargeFillPercentage = isOvercharged ? Mathf.Clamp01((_ultimateCharge - maxUltimate) / maxUltimate) : 0;

        ultimateBar.fillAmount = normalFillPercentage;
        ultimateBar.color = Color.Lerp(Color.black, Color.green, normalFillPercentage);

        overchardedBar.fillAmount = overchargeFillPercentage;
        overchardedBar.color = Color.Lerp(Color.green, Color.yellow, overchargeFillPercentage);
    }

    public void UseUltimate()
    {
        // Obtén la habilidad seleccionada
        UltimateAbilities selectedAbility = _ultimateAbilities[selectedAbilityIndex];

        if (Input.GetKeyDown(KeyCode.Z))
        {
            // Cambiar al siguiente índice en el array
            selectedAbilityIndex = (selectedAbilityIndex + 1) % _ultimateAbilities.Length;
            Debug.Log($"Switched to ability: {_ultimateAbilities[selectedAbilityIndex].Name}");
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            // Verifica si hay suficiente carga para usar la habilidad
            if (_ultimateCharge >= selectedAbility.Cost)
            {
                // Consume la carga necesaria
                _ultimateCharge -= selectedAbility.Cost;

                // Asegúrate de mantener la carga dentro de los límites
                _ultimateCharge = Mathf.Clamp(_ultimateCharge, 0, maxOvercharge);

                // Instancia el prefab de la habilidad si está asignado
                if (selectedAbility.Ability != null)
                {
                    Instantiate(selectedAbility.Ability, transform.position, transform.rotation);
                }

                Debug.Log($"Used ability: {selectedAbility.Name}, remaining charge: {_ultimateCharge}");
            }
            else
            {
                Debug.Log("Not enough charge to use the selected ability!");
            }
        }
    }
    public void Cast()
    {
        _anim.SetTrigger("Cast");
    }



    public void Die()
    {
        //Handle.OnDie();
        freeze = true;
        gameObje.SetActive(false);

        // Rend1.enabled = false; Rend2.enabled = false;
    }

    public Vector3 CalculateJumpVelocity(Vector3 startpoint, Vector3 endpoint, float trayectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endpoint.y - startpoint.y;
        Vector3 displacementZX = new Vector3(endpoint.x - startpoint.x, 0f, endpoint.z - startpoint.z);

        Vector3 velocity = Vector3.up * Mathf.Sqrt(-2 * gravity * trayectoryHeight);
        Vector3 velocityZX = displacementZX / (Mathf.Sqrt(-2 * trayectoryHeight / gravity)
            + Mathf.Sqrt(2 * (displacementY - trayectoryHeight) / gravity));

        return velocity + velocityZX;
    }

    public void JumpToPosition(Vector3 targetposition, float trajectoryHeight)
    {
        activeGrapple = true;
        velocityToSet = CalculateJumpVelocity(transform.position, targetposition, trajectoryHeight);
        Invoke(nameof(Setvelocity), 0.1f);
    }



    public void ResetRestrictions()
    {
        activeGrapple = false;
    }


    private void Setvelocity()
    {
        EnableMovementAfterCollision = true;

        // Limitar la magnitud de la velocidad al máximo permitido
        if (velocityToSet.magnitude > maxGrappleVelocity)
        {
            velocityToSet = velocityToSet.normalized * maxGrappleVelocity;
        }

        _rb.velocity = velocityToSet * velocity;
    }

    private bool isDead = false;
    public override void ReciveDamage(float damage)
    {
        GetLife -= damage;

        if (isDead) return; // Si ya está muerto, no hacer nada

        if (GetLife <= 0)
        {
            if (_anim != null)
            {
                _anim.SetTrigger("Die");
            }
            isDead = true; // Marcar como muerto
        }
        else
        {
            if (_anim != null)
            {
                _anim.SetTrigger("Hit");
                // _bloodVFX.SendEvent("OnTakeDamage");
            }
        }
    }

    public void MovePlayer(float force)
    {
        Vector3 forwardDirection = transform.forward; // Dirección actual del jugador
        _rb.AddForce(forwardDirection * force, ForceMode.Impulse);
    }

    public void ApplyForwardJumpImpulse(float forwardForce, float jumpForce)
    {
        // Dirección hacia adelante basada en la orientación actual del jugador
        Vector3 forwardDirection = transform.forward * forwardForce;

        // Impulso en el eje Y para el salto
        Vector3 upwardImpulse = Vector3.up * jumpForce;

        // Aplica ambas fuerzas al Rigidbody
        _rb.AddForce(forwardDirection + upwardImpulse, ForceMode.Impulse);
    }
    public override void Health(float amount)
    {
        GetLife += amount;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (EnableMovementAfterCollision)
        {
            EnableMovementAfterCollision = false;
            ResetRestrictions();

           // grapple.stopGrapple();
        }
    }


    public enum ElementType
    {
        Normal,
        Fire,
        Electric,
    }
    public void ElementalCast()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            selectedElement = (ElementType)(((int)selectedElement + 1) % System.Enum.GetValues(typeof(ElementType)).Length);
            Debug.Log("Elemento seleccionado: " + selectedElement);
        }
    }
    public void PlayVFX()
    {
        _fire.SendEvent("OnFire");
    }

    public void PlayVFXAttack()
    {
        _fire.SendEvent("Attack");
    }
}
