using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour, IAnimObservable
{

    public ComboNode rootNode;
    private ComboNode currentNode;
    public KeyCode keyCode;
    public KeyCode fireKey;
    private bool isAttacking = false;
    [SerializeField] private float comboTimer = 0f;
    public float comboResetTime = 1.2f;
    public float comboReset = 1.2f;
    [SerializeField] private bool canCombo;
    [SerializeField] private float shootRepeatRate = 0.2f;
    private float shootTimer = 0f;
    public float Damage;
    public bool CanCombo { get { return canCombo; } set { canCombo = value; } }

    private Queue<ComboInput> inputBuffer = new Queue<ComboInput>();
    [SerializeField] private ComboInput comboInput;
    void Start()
    {
        currentNode = rootNode;
        //     EventManager.Subscribe("OnAttack", OnAttack);
        //     EventManager.Subscribe("ComboChanger", ComboChanger);
        //UnlockDefaultCombos(rootNode);
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

                    foreach (var observer in _observers)
                        observer.OnShootStateChanged(true);

                    shootTimer = 0f;
                }
            }
            else
            {
                shootTimer = shootRepeatRate;
                comboResetTime = comboReset;

                foreach (var observer in _observers)
                    observer.OnShootStateChanged(false);
            }
            if (!isAttacking && inputBuffer.Count > 0)
            {
                ComboInput input = inputBuffer.Dequeue();
                TryExecuteNode(input);
            }
        }
    }

    void ComboChanger(params object[] args)
    {
        var combo = (ComboNode)args[0];
        ChangeCombatStyle(combo);
    }

    public void ChangeCombatStyle(ComboNode newRootNode)
    {
        rootNode = newRootNode;
        currentNode = rootNode;
        ResetCombo(); // Opcional: limpia el estado actual del combo para evitar inconsistencias
        Debug.Log("Estilo de combate cambiado." + newRootNode.name);
    }
    void TryExecuteNode(ComboInput input)
    {
        ComboNode nextNode = currentNode.GetNextNode(input);
        comboInput = input;

        if (nextNode != null)
        {
            //StartCoroutine(PerformAttack(nextNode));
            PerformAttack(nextNode);
            currentNode = nextNode;
        }
        else if (currentNode == rootNode)
        {
            ComboNode rootNext = rootNode.GetNextNode(input);
            if (rootNext != null)
            {
                //StartCoroutine(PerformAttack(rootNext));
                PerformAttack(rootNext);
                currentNode = rootNext;
            }
        }
    }/*
    void UnlockDefaultCombos(ComboNode node, HashSet<ComboNode> visitedNodes = null)
    {
        if (visitedNodes == null)
            visitedNodes = new HashSet<ComboNode>();

        // Si ya visitamos este nodo, salimos para evitar bucle
        if (visitedNodes.Contains(node))
            return;

        visitedNodes.Add(node);

        foreach (var transition in node.transitions)
        {
            if (transition.unlockByDefault && !ComboUnlockManager.Instance.IsUnlocked(transition.comboID))
            {
                ComboUnlockManager.Instance.UnlockCombo(transition.comboID);
            }

            if (transition.nextNode != null)
                UnlockDefaultCombos(transition.nextNode, visitedNodes);
        }
    }*/
    private IAnimObserver animationObserver;

    public Transform attackPoint; // Asigna un Empty en la mano o arma
    public float attackRange = 1.5f;
    public LayerMask enemyLayers;
    float stylePerEnemy;
    void PerformAttack(ComboNode node)
    {
        Debug.Log("Ejecutando Ataque " + node);

        foreach (var observer in _observers)
            observer.OnAttackTriggered(node); // <--- ahora pasa el nodo entero
    }

    void OnAttack(params object[] args)
    {
        Attack();
    }
    private void Attack()
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

            // 🔹 Llamamos directamente al evento de daño para este enemigo
            EventManager.Trigger("SendDamage", Damage, enemy.gameObject);
            // Puedes ajustar el "10f" para usar valores según comboInput o un daño real
        }

        // Registrar los golpes reales en el combo counter
        StyleMeter styleMeter = FindObjectOfType<StyleMeter>();

        if (enemiesHit > 0)
        {
            for (int i = 0; i < enemiesHit; i++)
            {
                EventManager.Trigger("RegisterHit");
            }

            if (styleMeter != null)
            {
                styleMeter.AddStylePoints(enemiesHit * stylePerEnemy);
            }
        }

        isAttacking = true;
        comboTimer = 0f;
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

    public void Subscribe(IAnimObserver x)
    {
        if (_observers.Contains(x)) return;
        _observers.Add(x);
    }

    [SerializeField] List<IAnimObserver> _observers = new List<IAnimObserver>();

    public void Unsubscribe(IAnimObserver x)
    {
        if (_observers.Contains(x)) return;
        _observers.Remove(x);
    }
    void OnEnable()
    {
        EventManager.Subscribe("OnAttack", OnAttack);
        EventManager.Subscribe("ComboChanger", ComboChanger);
        canCombo = true;
    }

    void OnDisable()
    {
        EventManager.Unsubscribe("OnAttack", OnAttack);
        EventManager.Unsubscribe("ComboChanger", ComboChanger);
    }
}