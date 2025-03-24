using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
public enum ElementType
{
    Normal,
    Fire,
    Electric,
}
public class PlayerAttack : Player
{
    [Header("<color=red>Misc</color>")]
    [SerializeField] private VisualEffect _fire;

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
    private void Update()
    {
        Ultimate(true);
        UseUltimate();

    }

    private void Start()
    {
        _anim = GetComponentInChildren<Animator>();
    }

    private void OnDrawGizmos()
    {

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
    public override void Attack()
    {
        _atkRay = new Ray(_atkOrigin.position, transform.forward);

        if (Physics.SphereCast(_atkRay, _sphereAtkRadius, out _atkHit, _atkRayDist, _atkMask))
        {
            if (_atkHit.collider.TryGetComponent<HP>(out HP enemy))
            {
                enemy.ReciveDamage(_atkDmg);
            }
            _ultimateCharge++;
        }

        else if (Physics.Raycast(_atkRay, out _atkHit, _atkRayDist, _atkMask))
        {
            if (_atkHit.collider.TryGetComponent<HP>(out HP enemy))
            {
                enemy.ReciveDamage(_atkDmg);
            }
            _ultimateCharge++;
        }
    }

    public override void PerformLiftAttack()
    {
        _atkRay = new Ray(_atkOrigin.position, transform.forward);

        if (Physics.Raycast(_atkRay, out _atkHit, _atkRayDist, _atkMask))
        {
            if (_atkHit.collider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.ReciveDamage(_atkDmg);
                enemy.ApplyLiftImpulse();
            }
            _ultimateCharge++;
        }
    }

    [SerializeField] private float maxUltimate;
    [SerializeField] private float maxOvercharge;

    [SerializeField] UltimateAbilities[] _ultimateAbilities;
    [SerializeField] private int selectedAbilityIndex = 0;
    private float duration;

    [Header("<color=#6A89A7>UI</color>")]
    [SerializeField] private Image ultimateBar;
    [SerializeField] private Image overchardedBar;

    public override void Cast()
    {
        _anim.SetTrigger("Cast");
        selectedElement = (ElementType)(((int)selectedElement + 1) % System.Enum.GetValues(typeof(ElementType)).Length);
        Debug.Log("Elemento seleccionado: " + selectedElement);
    }
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

    public override void PlayVFX()
    {
        _fire.SendEvent("OnFire");
    }

    public override void PlayVFXAttack()
    {
        _fire.SendEvent("Attack");
    }
}
