using UnityEngine;

public class OrbitalSolver : MonoBehaviour
{
    public enum OrientationType
    {
        Unmodified,
        FollowTrackedObject,
        FaceTrackedObject,
        YawOnly,
        CameraFacing,
        CameraAligned
    }

    private Transform trackedObject;

    private float lastUpdateTime;

    private float deltaTime;

    [SerializeField]
    private float moveLerpTime = 0.1f;

    [SerializeField]
    private float rotateLerpTime = 0.1f;

    [SerializeField]
    private float scaleLerpTime = 0;

    [SerializeField]
    private bool maintainScaleOnInitialization = true;

    [SerializeField]
    private bool smoothing = true;
    
    [SerializeField]
    private float lifetime = 0;

    private float currentLifetime;

    [SerializeField]
    private OrientationType orientationType = OrientationType.FollowTrackedObject;

    [SerializeField]
    private Vector3 localOffset = new Vector3(0, -1, 1);

    [SerializeField] 
    private Vector3 worldOffset = Vector3.zero;

    [SerializeField]
    private Vector3 additionalRotation; // from SolverHandler

    [SerializeField]
    private bool useAngleStepping = false;
    
    [Range(2, 24)]
    [SerializeField]
    private int tetherAngleSteps = 6;

    void Awake()
    {
        currentLifetime = 0;
        deltaTime = Time.deltaTime;
        lastUpdateTime = Time.realtimeSinceStartup;
    }

    void Start()
    {
        //TODO: add more target types
        
        trackedObject = Camera.main.transform;
    }

    void Update()
    {
        deltaTime = Time.realtimeSinceStartup - lastUpdateTime;
        lastUpdateTime = Time.realtimeSinceStartup;
    }

    void LateUpdate()
    {
        if (trackedObject != null && enabled)
        {
            currentLifetime += deltaTime;

            if (lifetime > 0 && currentLifetime >= lifetime)
            {
                enabled = false;
                return;
            }

            Quaternion yawOnlyRotation = Quaternion.Euler(0, trackedObject.rotation.eulerAngles.y, 0);

            Vector3 desiredPosition = trackedObject.position;
            desiredPosition += SnapToTetherAngleSteps(trackedObject.rotation) * localOffset;
            desiredPosition += SnapToTetherAngleSteps(yawOnlyRotation) * worldOffset;
            Quaternion desiredRotation = CalculateDesiredRotation(desiredPosition);
            Vector3 desiredScale = maintainScaleOnInitialization ? transform.localScale : Vector3.one;

            transform.position = smoothing ? SmoothTo(transform.position, desiredPosition, deltaTime, moveLerpTime) : desiredPosition;
            transform.rotation = smoothing ? SmoothTo(transform.rotation, desiredRotation, deltaTime, rotateLerpTime) : desiredRotation;
            transform.localScale = smoothing ? SmoothTo(transform.localScale, desiredScale, deltaTime, scaleLerpTime) : desiredScale;
        }
    }

    private Quaternion SnapToTetherAngleSteps(Quaternion rotationToSnap)
    {
        if (!useAngleStepping)
            return rotationToSnap;

        float stepAngle = 360f / tetherAngleSteps;
        int numberOfSteps = Mathf.RoundToInt(trackedObject.eulerAngles.y / stepAngle);
        float newAngle = stepAngle * numberOfSteps;

        return Quaternion.Euler(rotationToSnap.eulerAngles.x, newAngle, rotationToSnap.eulerAngles.z);
    }

    private Quaternion CalculateDesiredRotation(Vector3 desiredPosition)
    {
        Quaternion desiredRotation = Quaternion.identity;

        switch (orientationType)
        {
            case OrientationType.Unmodified:
                desiredRotation = transform.rotation;
                break;

            case OrientationType.FollowTrackedObject:
                desiredRotation = trackedObject.rotation;
                break;

            case OrientationType.FaceTrackedObject:
                desiredRotation = Quaternion.LookRotation(trackedObject.position - desiredPosition);
                break;

            case OrientationType.YawOnly:
                float trackedObjectYRotation = trackedObject.eulerAngles.y;
                desiredRotation = Quaternion.Euler(0f, trackedObjectYRotation, 0f);
                break;

            case OrientationType.CameraFacing:
                desiredRotation = Quaternion.LookRotation(Camera.main.transform.position - desiredPosition);
                break;

            case OrientationType.CameraAligned:
                desiredRotation = Camera.main.transform.rotation;
                break;

            default:
                Debug.LogError("Invalid OrientationType");
                break;
        }

        if (useAngleStepping)
            desiredRotation = SnapToTetherAngleSteps(desiredRotation);

        desiredRotation *= Quaternion.Euler(additionalRotation);

        return desiredRotation;
    }

    public static Vector3 SmoothTo(Vector3 source, Vector3 goal, float deltaTime, float lerpTime)
    {
        return Vector3.Lerp(source, goal, lerpTime.Equals(0.0f) ? 1f : deltaTime / lerpTime);
    }

    public static Quaternion SmoothTo(Quaternion source, Quaternion goal, float deltaTime, float lerpTime)
    {
        return Quaternion.Slerp(source, goal, lerpTime.Equals(0.0f) ? 1f : deltaTime / lerpTime);
    }
}
