using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables; // Importar para usar PlayableDirector

public class BossSpawner : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private PlayableDirector director; // Referencia al PlayableDirector
    public AudioClip Theme;
    [SerializeField] private FireShader FireShader;
   // [SerializeField] private BoxCollider collider;

    [Header("Spawn Area Settings")]
    [SerializeField] private Transform spawnPoint; // Lista de puntos de spawn
    [SerializeField] private Boss enemy;


    private void Awake()
    {
        if (_player == null) _player = GameManager.Instance.Player;
        if (FireShader == null) FireShader = FindObjectOfType<FireShader>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verificamos si el objeto que colisiona está en la capa del jugador
        if (((1 << other.gameObject.layer) & playerLayerMask) != 0)
        {
            //FireShader.ActivateFire();
            print("Jugador detectado.");
            if (director != null)
            {
                _player.freeze = true; // Congela al jugador
                director.Play(); // Inicia la reproducción del PlayableDirector
          
                // Subscribirse al evento "stopped"
                director.stopped += OnTimelineFinished;
                //FireShader.ActivateFire();
            }
            else
            {
                Debug.LogWarning("No se asignó un PlayableDirector en el inspector.");
            }
        }
    }

    public void SpawnBoss()
    {
        Instantiate(enemy, spawnPoint.position, Quaternion.identity);

    }

    public void Flame()
    {
        FireShader.ActivateFire();
    }

    private void OnTimelineFinished(PlayableDirector finishedDirector)
    {
        if (finishedDirector == director)
        {
            Debug.Log("Timeline finalizada. Liberando al jugador.");
            _player.freeze = false; // Libera al jugador
            director.stopped -= OnTimelineFinished; // Desuscribirse del evento
            FireShader.ActivateFire();
            //SFXManager.instance.PlaySFXClip(Theme, transform, 1f);
            //Destroy(gameObject);
        }
    }
}
