using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] GameObject Manager;
    public float smoothTime = 0.3f;
    public Vector3 offset;

    private Vector3 velocity = Vector3.zero;

    public void Start()
    {
        Manager.GetComponent<CarsManager>().CameraUpdateHandler += OnCameraUpdate;
    }

    void OnCameraUpdate(object sender, EventArgs e)
    {
        if (sender != null)
        {
            Vector3 desiredPosition = (sender as GameObject).transform.position + offset;
            //Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
            transform.position = desiredPosition;
            transform.LookAt((sender as GameObject).transform);
        }
    }
}