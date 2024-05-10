using UnityEngine;


public class Car : MonoBehaviour
{
    public NeuralNetwork NN;

    public float viewdistance = 10f;
    public float[] CreateRaycasts(int n_raycasts, float anglebetweenrays)
    {
        float[] distances = new float[n_raycasts];

        RaycastHit hit;
        for (int i = 0; i < n_raycasts; i++)
        {
            float angle = (2 * i + 1 - n_raycasts) * anglebetweenrays / 2;
            //ruota il raggio in base all'angolo
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 direction = rotation * transform.forward * -1;

            //incrementa il punto di partenza del raggio di 0.1 unità
            Vector3 origin = transform.position + transform.up * 0.1f;
            if (Physics.Raycast(origin, direction, out hit, viewdistance))
            {
                //disegna la linea del raggio
                Debug.DrawLine(origin, hit.point, Color.red);
                if (hit.transform.gameObject.tag == "border")
                    distances[i] = hit.distance;
                else
                    distances[i] = viewdistance;
            }
            else
            {
                distances[i] = viewdistance;
            }
        }
        return distances;
    }
}