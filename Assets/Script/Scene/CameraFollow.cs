using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;

    [Header("Clamp")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;
    void LateUpdate()
    {
        if (target == null) return;

        float clampX = Mathf.Clamp(
            target.position.x,
            minX,
            maxX
        );

        float clampY = Mathf.Clamp(
            target.position.y,
            minY,
            maxY
        );

        Vector3 desiredPos = new Vector3(
            clampX,
            clampY,
            transform.position.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPos,
            smoothSpeed * Time.deltaTime
        );
    }
}