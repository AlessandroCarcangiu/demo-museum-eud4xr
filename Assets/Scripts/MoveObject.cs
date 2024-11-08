using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public Transform cameraTransform; // Riferimento al transform della camera
    public float distance = 1f;       // Distanza che la "mano" deve percorrere in avanti
    public float speed = 5f;          // Velocità di movimento

    private Vector3 localStartPosition; // Posizione locale iniziale rispetto alla camera
    private bool isMoving = false;      // Controlla se la mano si sta muovendo avanti o indietro
    private bool movingForward = true;  // Controlla la direzione attuale del movimento

    void Start()
    {
        // Salva la posizione iniziale locale rispetto alla camera
        localStartPosition = cameraTransform.InverseTransformPoint(transform.position);
    }

    void Update()
    {
        // Se il tasto sinistro viene premuto e la mano non è in movimento
        if (!isMoving && Input.GetMouseButtonDown(0))
        {
            isMoving = true;       // Inizia il movimento
            movingForward = true;  // Imposta la direzione di movimento in avanti
        }

        // Movimento della mano avanti e indietro
        if (isMoving)
        {
            // Calcola la posizione locale target a seconda della direzione
            Vector3 targetLocalPosition = movingForward
                ? localStartPosition + Vector3.forward * distance
                : localStartPosition;

            // Calcola la posizione globale target in base alla camera
            Vector3 targetWorldPosition = cameraTransform.TransformPoint(targetLocalPosition);

            // Muovi la mano verso la posizione target nel mondo
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetWorldPosition,
                speed * Time.deltaTime
            );

            // Mantieni l'orientamento della mano allineato alla camera
            transform.rotation = cameraTransform.rotation;

            // Controlla se ha raggiunto la posizione target
            if (transform.position == targetWorldPosition)
            {
                if (movingForward)
                {
                    // Se era in avanti, inizia il movimento indietro
                    movingForward = false;
                }
                else
                {
                    // Altrimenti, ha finito il ciclo e può fermarsi
                    isMoving = false;
                }
            }
        }
    }
}
