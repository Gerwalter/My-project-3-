using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SlotMachine : ButtonBehaviour
{
    [SerializeField] private Renderer _renderer;

    // Array de imágenes
    public Sprite[] slotImages;
    public Image[] slots; // Las tres imágenes de la máquina tragamonedas
    public float spinDuration = 2.0f; // Duración total del giro por slot
    public float delayBetweenSlots = .5f;
    public AudioClip gambling, awwdangit, slotsound, winnin;
    private AudioSource audioSource;

    private void Start()
    {
        // Asegurarse de que haya un AudioSource en el objeto
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public override void OnInteract()
    {
        float r = Random.Range(0f, 1f);
        float g = Random.Range(0f, 1f);
        float b = Random.Range(0f, 1f);

        // Crea un nuevo color con los valores generados.
        Color randomColor = new Color(r, g, b);

        // Aplica el color al material del objeto.
        _renderer.material.color = randomColor;

        // Reproduce el sonido de gambling
        audioSource.PlayOneShot(gambling);

        // Iniciar la corrutina una vez que termine el sonido de gambling
        StartCoroutine(WaitForGamblingSound());
    }

    IEnumerator WaitForGamblingSound()
    {
        // Esperar hasta que el clip "gambling" termine de reproducirse
        yield return new WaitWhile(() => audioSource.isPlaying);

        // Reproducir el sonido de los slots en loop mientras giran
        audioSource.clip = slotsound;
        audioSource.loop = true;
        audioSource.Play();

        // Iniciar el giro de los slots
        StartCoroutine(SpinSlots());
    }

    IEnumerator SpinSlots()
    {
        yield return new WaitForSeconds(.6f);
        for (int i = 0; i < slots.Length; i++)
        {
            // Iniciar el giro del slot
            yield return StartCoroutine(SpinSlot(slots[i], spinDuration));

            // Esperar un poco antes de girar el siguiente slot
            yield return new WaitForSeconds(delayBetweenSlots);
        }

        // Verificar si todas las imágenes coinciden
        CheckWin();
    }

    IEnumerator SpinSlot(Image slot, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            int randomIndex = Random.Range(0, slotImages.Length);
            slot.sprite = slotImages[randomIndex];
            elapsedTime += Time.deltaTime;
            yield return new WaitForSeconds(0.1f); // Ajustar la velocidad del giro
        }
    }

    void CheckWin()
    {
        // Detener el sonido de los slots
        audioSource.Stop();

        int[] selectedIndexes = new int[3];
        for (int i = 0; i < slots.Length; i++)
        {
            selectedIndexes[i] = System.Array.IndexOf(slotImages, slots[i].sprite);
        }

        if (selectedIndexes[0] == selectedIndexes[1] && selectedIndexes[1] == selectedIndexes[2])
        {
            SFXManager.instance.PlaySFXClip(winnin, transform, 1f);
            Debug.Log("Ganaste");
        }
        else
        {
            SFXManager.instance.PlaySFXClip(awwdangit, transform, 1f);
            Debug.Log("Sigue intentando");
        }
    }
}
