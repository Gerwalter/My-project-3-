using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserConnector : MonoBehaviour, IAlertSystemObserver
{
    [Header("Detección")]
    public LayerMask capasDetectables;   // Asigna aquí la capa del jugador
    public float maxDistanciaExtra = 1f; // Por si el player está justo más lejos
    [SerializeField] private float alertAmount = 1f;
    [Header("Puntos del láser")]
    public Transform pointA;
    public Transform pointB;

    [Header("Opciones del láser")]
    public float umbralAlerta = 50f;  // Valor de alerta necesario para encender el láser
    public float grosor = 0.05f;
    public Color colorLaser = Color.red;

    private LineRenderer line;
    private bool laserActivo = false;

    void Start()
    {
        line = GetComponent<LineRenderer>();

        // Configuración visual inicial
        line.positionCount = 2;
        line.startWidth = grosor;
        line.endWidth = grosor;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = colorLaser;
        line.endColor = colorLaser;

        line.enabled = false; // Apagado desde el inicio

        // Suscribirse al sistema de alerta
        ThiefAlertSystem.instance.Subscribe(this);
    }

    void OnDestroy()
    {
        if (ThiefAlertSystem.instance != null)
            ThiefAlertSystem.instance.Unsubscribe(this);
    }

    void Update()
    {
        if (!laserActivo) return;
        if (pointA == null || pointB == null) return;

        line.SetPosition(0, pointA.position);
        line.SetPosition(1, pointB.position);

        Vector3 origen = pointA.position;
        Vector3 destino = pointB.position;
        Vector3 direccion = (destino - origen).normalized;

        float distancia = Vector3.Distance(origen, destino) + maxDistanciaExtra;

        // --- Raycast para detectar colisión ---
        if (Physics.Raycast(origen, direccion, out RaycastHit hit, distancia, capasDetectables))
        {
            // Si golpea al jugador o algo en esa capa, el láser termina ahí
            line.SetPosition(0, origen);
            line.SetPosition(1, hit.point);

            EventManager.Trigger("IncreaseAlert", alertAmount);
        }
        else
        {
            // Si no golpea, une A y B normalmente
            line.SetPosition(0, origen);
            line.SetPosition(1, destino);
        }
    }

    // ============================
    //       OBSERVER
    // ============================
    public void Notify(float alertaActual, float alertaMaxima)
    {
        // Activar el láser si supera el umbral
        if (alertaActual >= umbralAlerta)
        {
            ActivarLaser();
        }
        else
        {
            ApagarLaser();
        }
    }

    private void ActivarLaser()
    {
        if (!laserActivo)
        {
            laserActivo = true;
            line.enabled = true;
        }
    }

    private void ApagarLaser()
    {
        if (laserActivo)
        {
            laserActivo = false;
            line.enabled = false;
        }
    }
}
