using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class WhipRenderer : MonoBehaviour
{
    [Header("Referencias")]
    public Transform basePoint;        // Origen del látigo
    public Transform tipPoint;         // Punta (el cubo)

    [Header("Ajustes de línea")]
    public int maxPoints = 60;
    public float minPointDistance = 0.05f;
    public float smoothSpeed = 10f;

    [Header("Auto Clear / Fade")]
    public bool enableAutoClear = true;
    public float clearAfterSeconds = 0.8f;
    public float fadeDuration = 0.25f;
    public bool keepBasePoint = true;

    [Header("Ocultar al estar retraído")]
    public float retractHideDistance = 0.1f; // distancia entre base y punta para ocultar
    public bool hideWhenRetracted = true;

    LineRenderer lr;
    List<Vector3> points = new List<Vector3>();
    Queue<float> pointTimes = new Queue<float>();

    float initialWidth;
    bool isVisible = true;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        initialWidth = lr.startWidth;
        if (basePoint == null || tipPoint == null)
            Debug.LogWarning("WhipRenderer: asigna basePoint y tipPoint en el inspector.");
    }

    void Start()
    {
        ClearAll();
        if (hideWhenRetracted) lr.enabled = false; // empieza oculto
    }

    void LateUpdate()
    {
        if (basePoint == null || tipPoint == null) return;

        float distBaseTip = Vector3.Distance(basePoint.position, tipPoint.position);

        // Si el látigo está retraído, ocultar y limpiar
        if (hideWhenRetracted && distBaseTip <= retractHideDistance)
        {
            if (isVisible)
            {
                lr.enabled = false;
                ClearAll();
                isVisible = false;
            }
            return;
        }

        // Si el látigo empieza a extenderse, volver a mostrar
        if (hideWhenRetracted && !isVisible && distBaseTip > retractHideDistance)
        {
            lr.enabled = true;
            isVisible = true;
        }

        // Asegurar que el primer punto sea la base
        if (keepBasePoint)
        {
            if (points.Count == 0)
            {
                points.Add(basePoint.position);
                pointTimes.Enqueue(Time.time);
            }
            else points[0] = basePoint.position;
        }

        Vector3 lastPoint = points.Count > 0 ? points[points.Count - 1] : basePoint.position;

        // Añadir puntos si la punta se aleja
        if (Vector3.Distance(lastPoint, tipPoint.position) >= minPointDistance)
        {
            points.Add(tipPoint.position);
            pointTimes.Enqueue(Time.time);

            while (points.Count > maxPoints)
            {
                points.RemoveAt(keepBasePoint ? 1 : 0);
                if (pointTimes.Count > 0) pointTimes.Dequeue();
            }
        }
        else if (points.Count > 0)
        {
            // Suavizar el último punto hacia la punta
            if (smoothSpeed <= 0f)
                points[points.Count - 1] = tipPoint.position;
            else
                points[points.Count - 1] = Vector3.Lerp(points[points.Count - 1], tipPoint.position, Time.deltaTime * smoothSpeed);
        }

        // Auto clear / fade
        if (enableAutoClear)
        {
            while (pointTimes.Count > 0 && Time.time - pointTimes.Peek() > clearAfterSeconds)
            {
                if (keepBasePoint && points.Count > 1)
                    points.RemoveAt(1);
                else if (!keepBasePoint && points.Count > 0)
                    points.RemoveAt(0);
                pointTimes.Dequeue();
            }

            if (fadeDuration > 0f && points.Count > 0)
            {
                float oldestTime = pointTimes.Count > 0 ? pointTimes.Peek() : Time.time;
                float age = Time.time - oldestTime;
                float t = Mathf.Clamp01((age - clearAfterSeconds) / fadeDuration);
                SetLineAlpha(1f - t);
                if (t >= 1f && points.Count <= (keepBasePoint ? 1 : 0))
                    ClearAll();
            }
        }

        UpdateLineRenderer();
    }

    void SetLineAlpha(float a)
    {
        Gradient g = new Gradient();
        GradientColorKey[] colorKeys = lr.colorGradient.colorKeys;
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[colorKeys.Length];
        for (int i = 0; i < colorKeys.Length; i++)
            alphaKeys[i] = new GradientAlphaKey(a, colorKeys[i].time);
        g.SetKeys(colorKeys, alphaKeys);
        lr.colorGradient = g;
    }

    void UpdateLineRenderer()
    {
        lr.positionCount = points.Count;
        for (int i = 0; i < points.Count; i++)
            lr.SetPosition(i, points[i]);
    }

    public void ClearAll()
    {
        points.Clear();
        pointTimes.Clear();
        if (basePoint != null && keepBasePoint)
        {
            points.Add(basePoint.position);
            pointTimes.Enqueue(Time.time);
        }
        UpdateLineRenderer();
        SetLineAlpha(1f);
    }
}
