using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class FadingPlatform : MonoBehaviour
{
    [Header("<color=#FF4500>Behaviour</color>")]
    [SerializeField] private float _fadeTime = 3.0f;
    [SerializeField] private float _interval = 5.0f;
    [SerializeField] private float _spawnTime = 3.0f;

    private bool _isActive = false;

    public Collider _col;
    public MeshRenderer _renderer;
    public NavMeshModifier _mod;

    private void Start()
    {
        _col = GetComponent<Collider>();
        _renderer = GetComponent<MeshRenderer>();
        _mod = GetComponent<NavMeshModifier>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Player>() && !_isActive)
        {
            StartCoroutine(FadeBehaviour());
        }
    }

    private IEnumerator FadeBehaviour()
    {
        _isActive = true;

        float t = 0.0f;

        while (t < 1.0f)
        {
            t += Time.deltaTime / _fadeTime;
            yield return null;
        }

        _col.enabled = false;
        _mod.enabled = false;
        _renderer.enabled = false;

        GameManager.Instance.Surface.BuildNavMesh();

        yield return new WaitForSeconds(_interval);

        t = 0.0f;

        while (t < 1.0f)
        {
            t += Time.deltaTime / _spawnTime;
            yield return null;
        }

        _col.enabled = true;
        _mod.enabled = true;
        _renderer.enabled = true;

        GameManager.Instance.Surface.BuildNavMesh();

        _isActive = false;
    }
}
