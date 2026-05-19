using UnityEngine;

public class BackgroundFollow : MonoBehaviour
{
    public Transform cam;

    void LateUpdate()
    {
        transform.position = new Vector3(
            cam.position.x,
            cam.position.y,
            transform.position.z
        );
    }
}
