using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveItemTest : MonoBehaviour
{
    // Distanza di movimento
    public float moveDistance = 2.0f;
    // Velocità di movimento
    public float moveSpeed = 2.0f;
    // Tempo di attesa tra i movimenti
    public float waitTime = 1.0f;

    // Posizione iniziale dell'oggetto
    private Vector3 originalPosition;
    // Indice per determinare la direzione del movimento
    private int directionIndex = 0;

    void Start()
    {
        // Salva la posizione originale dell'oggetto
        originalPosition = transform.position;
        // Inizia il ciclo di movimento
        StartCoroutine(MoveCycle());
    }

    IEnumerator MoveCycle()
    {
        while (true)
        {
            // Aspetta prima di iniziare il prossimo movimento
            yield return new WaitForSeconds(waitTime);

            // Determina la direzione in base all'indice
            Vector3 targetPosition = originalPosition;

            switch (directionIndex)
            {
                case 0: // Avanti
                    targetPosition += transform.forward * moveDistance;
                    break;
                case 1: // Indietro
                    targetPosition -= transform.forward * moveDistance;
                    break;
                case 2: // Sinistra
                    targetPosition -= transform.right * moveDistance;
                    break;
                case 3: // Destra
                    targetPosition += transform.right * moveDistance;
                    break;
            }

            // Muovi l'oggetto verso la destinazione
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null; // Aspetta il prossimo frame
            }

            // Aspetta un secondo prima di tornare indietro
            yield return new WaitForSeconds(waitTime);

            // Torna alla posizione originale
            while (Vector3.Distance(transform.position, originalPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            // Cambia direzione per il prossimo ciclo
            directionIndex = (directionIndex + 1) % 4;
        }
    }
}
