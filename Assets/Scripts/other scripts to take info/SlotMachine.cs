using System.Collections;
using UnityEngine;

public class SlotMachine : ButtonBehaviour
{
    [SerializeField] private Renderer _renderer;

    public Texture[] slotTextures;
    public Renderer[] slots;
    public float spinDuration = 2.0f;
    public float delayBetweenSlots = .5f;
    public AudioClip gambling, awwdangit, slotsound, winnin;

    [SerializeField] private string[] winMessages = { "Ganaste", "Bien", "Haha" };

    [SerializeField] private int itemCost = 10; // Costo del objeto en oro
    [SerializeField] private GoldManager _goldManager;

    public override void OnInteract()
    {
        float r = Random.Range(0f, 1f);
        float g = Random.Range(0f, 1f);
        float b = Random.Range(0f, 1f);

        // Crea un nuevo color con los valores generados.
        Color randomColor = new Color(r, g, b);



        _renderer.material.color = randomColor;
        if (_goldManager.SpendGold(itemCost/2))
        {
            SFXManager.instance.PlaySFXClip(gambling, transform, 1f);
            StartCoroutine(SpinSlots());
        }
        else
        {
            SFXManager.instance.PlaySFXClip(awwdangit, transform, 1f);
        }


    }

    IEnumerator SpinSlots()
    {
        yield return new WaitForSeconds(1.5f);
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

    IEnumerator SpinSlot(Renderer slot, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            int randomIndex = Random.Range(0, slotTextures.Length);
            slot.material.SetTexture("_MainTex", slotTextures[randomIndex]); // Cambia la textura del material
            elapsedTime += Time.deltaTime;
            yield return new WaitForSeconds(0.1f); // Ajustar la velocidad del giro
        }
    }

    void CheckWin()
    {

        int[] selectedIndexes = new int[slots.Length];
        for (int i = 0; i < slots.Length; i++)
        {
            Texture currentTexture = slots[i].material.GetTexture("_MainTex");
            selectedIndexes[i] = System.Array.IndexOf(slotTextures, currentTexture);
        }

        // Verificar si todas las texturas coinciden
        if (selectedIndexes[0] == selectedIndexes[1] && selectedIndexes[1] == selectedIndexes[2])
        {
            // Obtener el índice de la textura ganadora
            int winningIndex = selectedIndexes[0];

            // Reproducir el sonido de victoria
            SFXManager.instance.PlaySFXClip(winnin, transform, 1f);

            // Enviar el mensaje correspondiente
            if (winningIndex >= 0 && winningIndex < winMessages.Length)
            {
                Debug.Log(winMessages[winningIndex]);
                _goldManager.AddGold(itemCost * 3);
            }
        }
        else
        {
            SFXManager.instance.PlaySFXClip(awwdangit, transform, 1f);
            Debug.Log("Sigue intentando");
        }
    }
}
