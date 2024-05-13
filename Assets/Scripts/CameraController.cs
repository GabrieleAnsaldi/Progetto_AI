using System;
using System.Net.Sockets;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] GameObject Manager;
    public float smoothTime = 0.3f;
    public Vector3 offset;
    public GameObject objtofollow;
    private Vector3 velocity = Vector3.zero;

    public void Start()
    {
        Manager.GetComponent<CarsManager>().CameraUpdateHandler += OnCameraUpdate;
    }

    private void Update()
    {
        if (objtofollow != null)
        {
            //Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, objtofollow.transform.position, ref velocity, smoothTime);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, objtofollow.transform.position + offset, Time.deltaTime * smoothTime);
            transform.position = smoothedPosition;
            transform.LookAt(objtofollow.transform);
        }
    }

    void OnCameraUpdate(object sender, EventArgs e)
    {
        if (sender != null)
        {
            objtofollow = sender as GameObject;
            Vector3 desiredPosition = Vector3.Lerp(transform.position, (sender as GameObject).transform.position + offset, Time.deltaTime * smoothTime);
            transform.position = desiredPosition;
            transform.LookAt((sender as GameObject).transform);
        }
    }
}