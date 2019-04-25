using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    public int PanningMouseButton = 2;    
    private Vector3 panOrigin;
    private Vector3 panOffset;
    private bool isDragging;

    public float zoomMin = 20f, zoomMax = 50f;     

    private void LateUpdate()
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * (10f * Camera.main.orthographicSize * 0.1f), zoomMin, zoomMax);
        if (Input.GetMouseButton(PanningMouseButton))
        {
            panOffset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            if (!isDragging)
            {
                isDragging = true;
                panOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else
        {
            isDragging = false;
        }

        if (isDragging)
        {
            transform.position = panOrigin - panOffset;
        }
    }
}
