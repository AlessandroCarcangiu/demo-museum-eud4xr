using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class MovementUtils
{
    /* AGILE NOTES
 
 * Position: Restituisce la posizione del pivot dell'oggetto. NOTA: Questo non è detto che sia sempre al centro dell'oggetto, dipende da come è stato costruito il modello
 * [Collider]
 * Extents: E' sempre la metà della dimensione del bounding box.
 * Center: Rappresenta il
 * Size: E' l'altezza del bounding box. Corrisponde a 2*bounds.extents
 * Max: E' la somma bounds.center + bounds.extents. E' il punto in alto a destra (topRight)
 * Min: E' la differenza bounds.center - bounds.extents. E' il punto in basso a sinistra (bottomLeft)
 *
 * bounds.center = transform.position +  BoxCollider.position !! Attenzione: bounds.center != da collider.center!!!!!!!!!!!!!!
 * bounds.center.y == center.y * transform.localScale.y + transform.position.y;
 */

    public enum Direction
    {
        Above,
        Below,
        Left,
        Right,
        Front,
        Back
    }

    public static Vector3 GetPositionBTouchesA(BoxCollider A, BoxCollider B, Direction direction)
    {
        switch (direction)
        {
            case Direction.Above:
                return GetPositionBTouchesA_Above(A, B);
            case Direction.Below:
                return GetPositionBTouchesA_Below(A, B);
            case Direction.Left:
                return GetPositionBTouchesA_Left(A, B);
            case Direction.Right:
                return GetPositionBTouchesA_Right(A, B);
            case Direction.Front:
                return GetPositionBTouchesA_InFronOf(A, B);
            case Direction.Back:
                return GetPositionBTouchesA_BackOf(A, B);
            default:
                throw new System.NotImplementedException();
        }
    }

    private static Vector3 GetPositionBTouchesA_Above(BoxCollider A, BoxCollider B)
    {
        var bBounds = B.bounds;
        var newY = bBounds.max.y - A.center.y * A.gameObject.transform.localScale.y + A.bounds.extents.y;
        var newPosition = new Vector3(bBounds.center.x,newY,bBounds.center.z);
        return newPosition;
    }

    private static Vector3 GetPositionBTouchesA_Below(BoxCollider A, BoxCollider B)
    {
        var bounds = B.bounds;
        var newY = bounds.min.y - A.center.y * A.gameObject.transform.localScale.y - A.bounds.extents.y;
        var newPosition = new Vector3(bounds.center.x,newY,bounds.center.z);
        return newPosition;
    }

    private static Vector3 GetPositionBTouchesA_InFronOf(BoxCollider A, BoxCollider B)
    {
        var bBounds = B.bounds;
        var newZ = bBounds.max.z - A.center.z * A.gameObject.transform.localScale.z + A.bounds.extents.z;
        var newPosition = new Vector3(bBounds.center.x,bBounds.center.y,newZ);
        return newPosition;
    }

    private static Vector3 GetPositionBTouchesA_BackOf(BoxCollider A, BoxCollider B)
    {
        var bBounds = B.bounds;

        var newZ = bBounds.min.z - A.center.z * A.gameObject.transform.localScale.z - A.bounds.extents.z;
        var newPosition = new Vector3(bBounds.center.x,bBounds.center.y,newZ);
        return newPosition;
    }

    private static Vector3 GetPositionBTouchesA_Left(BoxCollider A, BoxCollider B)
    {
        var bBounds = B.bounds;
        var newX = bBounds.min.x - A.center.x * A.gameObject.transform.localScale.x -A.bounds.extents.x;
        var newPosition = new Vector3(newX,bBounds.center.y,bBounds.center.z);
        return newPosition;
    }

    private static Vector3 GetPositionBTouchesA_Right(BoxCollider A, BoxCollider B)
    {
        var bBounds = B.bounds;
        var newX = bBounds.max.x - A.center.x * A.gameObject.transform.localScale.x +A.bounds.extents.x;
        var newPosition = new Vector3(newX,bBounds.center.y,bBounds.center.z);
        return newPosition;
    }
}