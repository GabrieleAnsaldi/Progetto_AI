using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject target;
    public float smoothTime = 0.3f;
    public Vector3 offset;

    private Vector3 velocity = Vector3.zero;

    void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.transform.position + offset;
            Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
            transform.position = smoothedPosition;
            transform.LookAt(target.transform);
        }
    }
}