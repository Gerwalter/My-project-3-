using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public ComboNode rootNode; // Nodo inicial
    private ComboNode currentNode;
    public KeyCode keyCode;
    public KeyCode fireKey;
    [SerializeField] private Animator animator;
    private bool isAttacking = false;
   [SerializeField] private float comboTimer = 0f;
                    public float comboResetTime = 1.2f;
   [SerializeField] private bool canCombo;

    [SerializeField] private float shootRepeatRate = 0.2f;
    private float shootTimer = 0f;

    public bool CanCombo { get { return canCombo; } set { canCombo = value; } }

    private Queue<ComboInput> inputBuffer = new Queue<ComboInput>();
    [SerializeField] private ComboInput comboInput;
    void Start()
    {
       // animator = GetComponent<Animator>();
        currentNode = rootNode;

        UnlockDefaultCombos(rootNode);
    }

    void Update()
    {
        if (canCombo)
        {
            comboTimer += Time.deltaTime;

            if (comboTimer > comboResetTime)
            {
                ResetCombo();
            }

            if (Input.GetButtonDown("Fire1"))
            {
                inputBuffer.Enqueue(ComboInput.Light);
            }

            if (Input.GetButtonDown("Fire2"))
            {
                inputBuffer.Enqueue(ComboInput.Heavy);
            }

            if (Input.GetKeyDown(keyCode))
            {
                inputBuffer.Enqueue(ComboInput.Finisher);
            }

            if (Input.GetKey(fireKey))
            {
                shootTimer += Time.deltaTime;
                comboResetTime = 0;
                if (shootTimer >= shootRepeatRate)
                {
                    inputBuffer.Enqueue(ComboInput.Shoot);
                    animationObserver?.OnShootStateChanged(true);
                    shootTimer = 0f;
                }
            }
            else
            {
                shootTimer = shootRepeatRate;
                comboResetTime = 2;
                animationObserver?.OnShootStateChanged(false);// Esto permite que al soltar y volver a presionar, dispare de inmediato
            }
            if (!isAttacking && inputBuffer.Count > 0)
            {
                ComboInput input = inputBuffer.Dequeue();
                TryExecuteNode(input);
            }
        }
    }

    void TryExecuteNode(ComboInput input)
    {
        ComboNode nextNode = currentNode.GetNextNode(input);
        comboInput = input; 

        if (nextNode != null)
        {
            StartCoroutine(PerformAttack(nextNode));
            currentNode = nextNode;
        }
        else if (currentNode == rootNode)
        {
            ComboNode rootNext = rootNode.GetNextNode(input);
            if (rootNext != null)
            {
                StartCoroutine(PerformAttack(rootNext));
                currentNode = rootNext;
            }
        }
    }
    void UnlockDefaultCombos(ComboNode node)
    {
        foreach (var transition in node.transitions)
        {
            if (transition.unlockByDefault && !ComboUnlockManager.Instance.IsUnlocked(transition.comboID))
            {
                ComboUnlockManager.Instance.UnlockCombo(transition.comboID);
            }

            // Recursivamente desbloquear los nodos hijos
            if (transition.nextNode != null)
                UnlockDefaultCombos(transition.nextNode);
        }
    }
    private IAnimObserver animationObserver;

    void Awake()
    {
        animationObserver = GetComponent<IAnimObserver>();
        if (animationObserver == null)
        {
            Debug.LogWarning("No se encontró un observador de animación.");
        }
    }
    public Transform attackPoint; // Asigna un Empty en la mano o arma
    public float attackRange = 1.5f;
    public LayerMask enemyLayers;
    float stylePerEnemy;
    IEnumerator PerformAttack(ComboNode node)
    {

        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        int enemiesHit = 0;

        switch (comboInput)
        {
            case ComboInput.Light:
                stylePerEnemy = 10;
                break;
            case ComboInput.Heavy:
                stylePerEnemy = 20;
                break;
            case ComboInput.Finisher:
                stylePerEnemy = 50;
                break;
            case ComboInput.Shoot:
                stylePerEnemy = 1;
                break;
            default:
                Debug.Log("Entrada no reconocida.");
                break;
        }

        foreach (Collider enemy in hitEnemies)
        {
            enemiesHit++;
        }

        // Registrar los golpes reales en el combo counter
        StyleMeter styleMeter = FindObjectOfType<StyleMeter>();

        if (enemiesHit > 0)
        {
            for (int i = 0; i < enemiesHit; i++)
            {
                EventManager.Trigger("RegisterHit", 4);
            }

            if (styleMeter != null)
            {
                styleMeter.AddStylePoints(enemiesHit * stylePerEnemy);
            }
        }

        isAttacking = true;
        comboTimer = 0f;
        Debug.Log("Ejecutando nodo de ataque: " + node.nodeName); // Este es el log

        animationObserver?.OnAttackTriggered(comboInput.ToString());



        yield return new WaitForSeconds(node.duration);

        isAttacking = false;
    }
    void OnDrawGizmos()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
    void ResetCombo()
    {
        currentNode = rootNode;
        comboTimer = 0f;
        isAttacking = false;
        inputBuffer.Clear();
    }
}