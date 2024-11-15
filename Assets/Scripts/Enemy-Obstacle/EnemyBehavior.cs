using UnityEngine;

/// <summary>
/// Clase base gen�rica para manejar comportamientos espec�ficos de enemigos.
/// </summary>
public abstract class EnemyBehavior<T> : MonoBehaviour where T : System.Enum
{
    [SerializeField] private T _enemyClass;

    public T EnemyClass => _enemyClass;

    /// <summary>
    /// Define la l�gica espec�fica de comportamiento para cada tipo de enemigo.
    /// </summary>
    public abstract void ExecuteBehavior();

    protected virtual void Start()
    {
        Debug.Log($"Enemy initialized with class: {_enemyClass}");
    }
}
