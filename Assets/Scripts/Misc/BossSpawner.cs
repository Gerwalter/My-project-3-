using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables; // Importar para usar PlayableDirector

public class BossSpawner : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private PlayableDirector director; // Referencia al PlayableDirector

    private void Awake()
    {
        if (_player == null) _player = GameManager.Instance.Player;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verificamos si el objeto que colisiona está en la capa del jugador
        if (((1 << other.gameObject.layer) & playerLayerMask) != 0)
        {
            print("Jugador detectado.");
            if (director != null)
            {
                _player.freeze = true; // Congela al jugador
                director.Play(); // Inicia la reproducción del PlayableDirector

                // Subscribirse al evento "stopped"
                director.stopped += OnTimelineFinished;
            }
            else
            {
                Debug.LogWarning("No se asignó un PlayableDirector en el inspector.");
            }
        }
    }

    private void OnTimelineFinished(PlayableDirector finishedDirector)
    {
        if (finishedDirector == director)
        {
            Debug.Log("Timeline finalizada. Liberando al jugador.");
            _player.freeze = false; // Libera al jugador
            director.stopped -= OnTimelineFinished; // Desuscribirse del evento
            Destroy(gameObject);
        }
    }
}
