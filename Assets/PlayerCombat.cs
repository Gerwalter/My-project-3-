using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public ComboNode rootNode; // Nodo inicial
    private ComboNode currentNode;
    public KeyCode keyCode;
    //private Animator animator;
    private bool isAttacking = false;
   [SerializeField] private float comboTimer = 0f;
                    public float comboResetTime = 1.2f;
   [SerializeField] private bool canCombo;
    public bool CanCombo { get { return canCombo; } set { canCombo = value; } }

    private Queue<ComboInput> inputBuffer = new Queue<ComboInput>();

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
        if (nextNode != null)
        {
            StartCoroutine(PerformAttack(nextNode));
            currentNode = nextNode;
        }
        else if (currentNode == rootNode)
        {
            // Reintentar desde el nodo raíz
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

    public Transform attackPoint; // Asigna un Empty en la mano o arma
    public float attackRange = 1.5f;
    public LayerMask enemyLayers;
    IEnumerator PerformAttack(ComboNode node)
    {

        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        int enemiesHit = 0;

        foreach (Collider enemy in hitEnemies)
        {
            // Aquí haces daño real
            // enemy.GetComponent<EnemyHealth>()?.TakeDamage(damageAmount);

            enemiesHit++;
        }

        // Registrar los golpes reales en el combo counter
        if (enemiesHit > 0)
        {
            var comboCounter = FindObjectOfType<ComboCounter>();
            for (int i = 0; i < enemiesHit; i++)
            {
                comboCounter.RegisterHit();
            }
        }

        isAttacking = true;
        comboTimer = 0f;
        EventManager.Trigger("Attack", 2);
        Debug.Log("Ejecutando nodo de ataque: " + node.nodeName); // Este es el log

        //animator.Play(node.animationClip.name);



        yield return new WaitForSeconds(node.duration);

        isAttacking = false;
    }
    void OnDrawGizmosSelected()
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