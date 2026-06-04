using System;
using UnityEngine;

public class FollowSolver : MonoBehaviour
{
    [Tooltip("Reference to the player GameObject")]
    public Transform player;

    [Tooltip("If true, the avatar will face away from the player instead of towards them")]
    public bool invertDirection = false;

    
    private void Start()
    {
        if (!player) player = Camera.main.transform;
    }

    void Update()
    {
        FacePlayerEfficient();
    }
    
    private void FacePlayer()
    {
        Vector3 directionToPlayer = player.transform.position - transform.position;
        if (invertDirection)
        {
            directionToPlayer = -directionToPlayer;
        }
        directionToPlayer.y = 0; // Keep only the horizontal direction
        if (directionToPlayer.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
    
    private void FacePlayerEfficient()     {

        // Compute direction
        Vector3 direction = (player.position - transform.position);

        if (invertDirection)
            direction = -direction;

        // Ignore vertical difference (optional: remove if you want full 3D rotation)
        direction.y = 0f;

        if (direction != Vector3.zero) 
            transform.rotation = Quaternion.LookRotation(direction);
    }
}
// public class FollowSolver : MonoBehaviour
// {
//     public enum OrientationType
//     {
//         Unmodified,
//         FollowTrackedObject,
//         FaceTrackedObject,
//         YawOnly,
//         CameraFacing,
//         CameraAligned
//     }
//     
//     //TODO Added
//     [System.Flags]
//     public enum AxisFlags
//     {
//         None = 0,
//         XAxis = 1 << 0,
//         YAxis = 1 << 1,
//         ZAxis = 1 << 2
//     }
//
//     public enum AngularClampType
//     {
//         None,
//         ViewDegrees,
//         AngleStepping,
//         RendererBounds,
//         ColliderBounds
//     }
//
//     [SerializeField] private float moveLerpTime = 0.1f;
//     [SerializeField] private float rotateLerpTime = 0.1f;
//     // [SerializeField] private float scaleLerpTime = 0f;
//     // [SerializeField] private bool maintainScaleOnInitialization = true;
//     [SerializeField] private bool smoothing = true;
//     // [SerializeField] private float lifetime = 0;
//
//     // [SerializeField] private AxisFlags pivotAxis = AxisFlags.XAxis | AxisFlags.YAxis | AxisFlags.ZAxis; ??
//     
//     // Begin of Follow
//     [Header("Orientation")]
//     [SerializeField] private bool faceUserDefinedTargetTransform = false;
//     [SerializeField] private OrientationType orientationType = OrientationType.FaceTrackedObject;
//     [SerializeField] private bool faceTrackedObjectWhileClamped = true;
//     [SerializeField] private Transform targetToFace = null;
//     [SerializeField] private bool reorientWhenOutsideParameters = true;
//     [SerializeField] private float orientToControllerDeadZoneDegrees = 60f;
//
//     [Header("Distance")]
//     [SerializeField] private bool ignoreDistanceClamp = false;
//     [SerializeField] private float minDistance = 0.3f;
//     [SerializeField] private float maxDistance = 0.9f;
//     [SerializeField] private float defaultDistance = 0.7f;
//     [SerializeField] private float verticalMaxDistance = 0.0f;
//
//     [Header("Direction")]
//     [SerializeField] private bool ignoreAngleClamp = false;
//     [SerializeField] private bool ignoreReferencePitchAndRoll = false;
//     [SerializeField] private AngularClampType angularClampMode = AngularClampType.ViewDegrees;
//     [SerializeField] private float maxViewHorizontalDegrees = 30f;
//     [SerializeField] private float maxViewVerticalDegrees = 20f;
//     [SerializeField] private float pitchOffset = 0f;
//     [SerializeField, Range(2, 24)] private int tetherAngleSteps = 6; // ??
//     // [SerializeField] private float boundsScaler = 1.0f; // ??
//
//    
//
//
//     private Transform trackedObject;
//     private Quaternion previousGoalRotation;
//     private bool recenterNextUpdate = true;
//     private float lastUpdateTime;
//     private float deltaTime;
//
//     // from SolverHandler  
//     [Header("Solver Handler")]
//     // [SerializeField] private Vector3 localOffset = new Vector3(0, -1, 1);
//     // [SerializeField] private Vector3 worldOffset = Vector3.zero;
//     [SerializeField] private Vector3 additionalRotation;
//
//         void Awake()
//     {
//         lastUpdateTime = Time.realtimeSinceStartup;
//     }
//
//     void Start()
//     {
//         if (Camera.main != null) trackedObject = Camera.main.transform;
//         previousGoalRotation = transform.rotation;
//     }
//
//     void Update()
//     {
//         deltaTime = Time.realtimeSinceStartup - lastUpdateTime;
//         lastUpdateTime = Time.realtimeSinceStartup;
//     }
//
//     void LateUpdate()
//     {
//         if (trackedObject == null) return;
//
//         Vector3 refPosition = trackedObject.position;
//         Quaternion refRotation = trackedObject.rotation;
//         Vector3 refForward = refRotation * Vector3.forward;
//
//         // Optionally ignore pitch/roll on reference
//         if (ignoreReferencePitchAndRoll)
//         {
//             Vector3 forwardFlat = refForward;
//             forwardFlat.y = 0f;
//             if (forwardFlat.sqrMagnitude < 1e-6f) forwardFlat = Vector3.forward;
//             refRotation = Quaternion.LookRotation(forwardFlat.normalized);
//             if (!Mathf.Approximately(pitchOffset, 0f))
//             {
//                 Vector3 right = refRotation * Vector3.right;
//                 Vector3 pitched = Quaternion.AngleAxis(pitchOffset, right) * (refRotation * Vector3.forward);
//                 refRotation = Quaternion.LookRotation(pitched.normalized);
//             }
//             refForward = refRotation * Vector3.forward;
//         }
//
//         // Starting position for clamping calculations
//         Vector3 currentPosition = transform.position;
//         if (recenterNextUpdate)
//         {
//             currentPosition = refPosition + refForward * defaultDistance;
//         }
//
//         bool wasClamped = false;
//         Vector3 goalDirection = refForward;
//
//         if (!ignoreAngleClamp && !recenterNextUpdate && angularClampMode != AngularClampType.None)
//         {
//             wasClamped |= AngularClamp(refPosition, refRotation, currentPosition, ref goalDirection);
//         }
//
//         Vector3 goalPosition = currentPosition;
//         if (!ignoreDistanceClamp && !recenterNextUpdate)
//         {
//             wasClamped |= DistanceClamp(currentPosition, refPosition, goalDirection, ref goalPosition);
//         }
//
//         Quaternion goalRotation = Quaternion.identity;
//         ComputeOrientation(goalPosition, wasClamped || recenterNextUpdate, ref goalRotation);
//
//         if (recenterNextUpdate)
//         {
//             previousGoalRotation = goalRotation;
//             transform.position = goalPosition;
//             transform.rotation = goalRotation;
//             recenterNextUpdate = false;
//             return;
//         }
//
//         // If not clamped, don't force position change (prevents drift)
//         if (wasClamped)
//         {
//             transform.position = smoothing ? SmoothTo(transform.position, goalPosition, deltaTime, moveLerpTime) : goalPosition;
//         }
//
//         transform.rotation = smoothing ? SmoothTo(transform.rotation, goalRotation, deltaTime, rotateLerpTime) : goalRotation;
//         previousGoalRotation = goalRotation;
//     }
//
//     // --- Core clamp / helper functions ---
//
//     // A more robust angular clamp using planar projections and signed angles.
//     // This treats the configured maxView...Degrees as the FULL allowed angle threshold (not half).
//     private bool AngularClamp(Vector3 refPosition, Quaternion refRotation, Vector3 currentPosition, ref Vector3 refForward)
//     {
//         Vector3 toTarget = currentPosition - refPosition;
//         float currentDistance = toTarget.magnitude;
//         if (currentDistance <= 0.0001f) return false;
//         toTarget /= currentDistance;
//
//         // Start off with the rotation looking at the target
//         Quaternion rotation = Quaternion.LookRotation(toTarget, Vector3.up);
//
//         Vector3 currentRefForward = refRotation * Vector3.forward;
//         Vector3 refRight = refRotation * Vector3.right;
//
//         bool angularClamped = false;
//
//         // --- Vertical clamp (around refRight) ---
//         // Project both vectors onto a plane whose normal is refRight, then get the signed angle around refRight.
//         Vector3 toPlaneTarget = Vector3.ProjectOnPlane(toTarget, refRight).normalized;
//         Vector3 refPlaneForward = Vector3.ProjectOnPlane(currentRefForward, refRight).normalized;
//         if (toPlaneTarget.sqrMagnitude > 0.000001f && refPlaneForward.sqrMagnitude > 0.000001f)
//         {
//             float verticalAngle = Vector3.SignedAngle(refPlaneForward, toPlaneTarget, refRight);
//             float vLimit = maxViewVerticalDegrees; // using full-angle threshold (change to *0.5f to get MRTK original behavior)
//             if (verticalAngle < -vLimit)
//             {
//                 float delta = -vLimit - verticalAngle;
//                 rotation = Quaternion.AngleAxis(delta, refRight) * rotation;
//                 angularClamped = true;
//             }
//             else if (verticalAngle > vLimit)
//             {
//                 float delta = vLimit - verticalAngle;
//                 rotation = Quaternion.AngleAxis(delta, refRight) * rotation;
//                 angularClamped = true;
//             }
//         }
//
//         // --- Horizontal clamp / angle stepping (around world up) ---
//         if (angularClampMode == AngularClampType.AngleStepping)
//         {
//             float stepAngle = 360f / tetherAngleSteps;
//             int numberOfSteps = Mathf.RoundToInt(trackedObject.eulerAngles.y / stepAngle);
//             float newAngle = stepAngle * numberOfSteps;
//             rotation = Quaternion.Euler(rotation.eulerAngles.x, newAngle, rotation.eulerAngles.z);
//             angularClamped = true;
//         }
//         else if (angularClampMode == AngularClampType.ViewDegrees)
//         {
//             Vector3 toXZ = Vector3.ProjectOnPlane(toTarget, Vector3.up).normalized;
//             Vector3 refXZ = Vector3.ProjectOnPlane(currentRefForward, Vector3.up).normalized;
//
//             if (toXZ.sqrMagnitude > 0.000001f && refXZ.sqrMagnitude > 0.000001f)
//             {
//                 float horizontalAngle = Vector3.SignedAngle(refXZ, toXZ, Vector3.up);
//                 float hLimit = maxViewHorizontalDegrees; // using full-angle threshold (change to *0.5f for MRTK)
//                 if (horizontalAngle < -hLimit)
//                 {
//                     float delta = -hLimit - horizontalAngle;
//                     rotation = Quaternion.AngleAxis(delta, Vector3.up) * rotation;
//                     angularClamped = true;
//                 }
//                 else if (horizontalAngle > hLimit)
//                 {
//                     float delta = hLimit - horizontalAngle;
//                     rotation = Quaternion.AngleAxis(delta, Vector3.up) * rotation;
//                     angularClamped = true;
//                 }
//             }
//         }
//
//         refForward = rotation * Vector3.forward;
//         return angularClamped;
//     }
//
//     private bool DistanceClamp(Vector3 currentPosition, Vector3 refPosition, Vector3 refForward, ref Vector3 clampedPosition)
//     {
//         float currentDistance = Vector3.Distance(currentPosition, refPosition);
//         float clampedDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
//
//         // straightforward clamping along the refForward direction
//         clampedPosition = refPosition + refForward * clampedDistance;
//
//         if (verticalMaxDistance > 0f)
//         {
//             clampedPosition.y = Mathf.Clamp(clampedPosition.y, refPosition.y - verticalMaxDistance, refPosition.y + verticalMaxDistance);
//         }
//
//         return !Mathf.Approximately(currentDistance, clampedDistance);
//     }
//
//     private void ComputeOrientation(Vector3 goalPosition, bool needsRefresh, ref Quaternion orientation)
//     {
//         // Deadzone behavior: if small rotate changes versus previous, keep previous rotation to avoid jitter
//         if (!needsRefresh && reorientWhenOutsideParameters)
//         {
//             Vector3 nodeToCamera = goalPosition - trackedObject.position;
//             float angle = Mathf.Abs(AngleBetweenOnPlane(transform.forward, nodeToCamera, Vector3.up));
//             if (angle < orientToControllerDeadZoneDegrees)
//             {
//                 orientation = previousGoalRotation;
//                 return;
//             }
//         }
//
//         if (faceUserDefinedTargetTransform && targetToFace != null)
//         {
//             Vector3 directionToTarget = goalPosition - targetToFace.position;
//             if (directionToTarget.sqrMagnitude == 0f)
//             {
//                 orientation = Quaternion.identity;
//             }
//             else
//             {
//                 orientation = Quaternion.LookRotation(directionToTarget);
//             }
//             return;
//         }
//
//         OrientationType defaultOrientationType = orientationType;
//         if (needsRefresh && faceTrackedObjectWhileClamped)
//             defaultOrientationType = OrientationType.FaceTrackedObject;
//
//         switch (defaultOrientationType)
//         {
//             case OrientationType.YawOnly:
//                 orientation = Quaternion.Euler(0f, trackedObject.eulerAngles.y, 0f);
//                 break;
//             case OrientationType.Unmodified:
//                 orientation = transform.rotation;
//                 break;
//             case OrientationType.CameraAligned:
//                 if (Camera.main != null) orientation = Camera.main.transform.rotation;
//                 break;
//             case OrientationType.FaceTrackedObject:
//                 orientation = Quaternion.LookRotation(goalPosition - trackedObject.position);
//                 break;
//             case OrientationType.CameraFacing:
//                 if (Camera.main != null) orientation = Quaternion.LookRotation(goalPosition - Camera.main.transform.position);
//                 break;
//             case OrientationType.FollowTrackedObject:
//                 orientation = trackedObject.rotation;
//                 break;
//             default:
//                 orientation = transform.rotation;
//                 break;
//         }
//     }
//
//     // numeric helper like original: angle between vectors projected onto plane (returns degrees)
//     private float AngleBetweenOnPlane(Vector3 from, Vector3 to, Vector3 normal)
//     {
//         from.Normalize();
//         to.Normalize();
//         normal.Normalize();
//
//         Vector3 right = Vector3.Cross(normal, from);
//         Vector3 forward = Vector3.Cross(right, normal);
//
//         float angle = Mathf.Atan2(Vector3.Dot(to, right), Vector3.Dot(to, forward));
//         return angle * Mathf.Rad2Deg;
//     }
//
//     // smoothing helpers
//     public static Vector3 SmoothTo(Vector3 source, Vector3 goal, float deltaTime, float lerpTime)
//     {
//         return Vector3.Lerp(source, goal, Mathf.Approximately(lerpTime, 0f) ? 1f : deltaTime / lerpTime);
//     }
//
//     public static Quaternion SmoothTo(Quaternion source, Quaternion goal, float deltaTime, float lerpTime)
//     {
//         return Quaternion.Slerp(source, goal, Mathf.Approximately(lerpTime, 0f) ? 1f : deltaTime / lerpTime);
//     }
// }
