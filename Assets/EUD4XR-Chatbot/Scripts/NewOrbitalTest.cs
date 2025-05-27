using UnityEngine;

public class NewOrbitalTest : MonoBehaviour
{
    public Transform target;
    public Vector3 localOffset = new Vector3(0.15f, -0.15f, 0.45f);

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position;
        Quaternion desiredRotation = target.rotation;

        desiredPosition += desiredRotation * localOffset;
        
        transform.position = desiredPosition;
        transform.rotation = desiredRotation;
    }
}
