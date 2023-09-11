using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] GameObject cube;
    Camera _cam;

    [SerializeField] float zoomSpeed;
    [SerializeField] float zoomOutSpeed;
    [SerializeField] float zoomSize;
    
    Vector3 _posOrigin;
    float _zoomSizeOrigin;
    public bool zoom = false; // SMOOTH ZOOM TO THE CUBE (DURING AERIAL CHARGE)
    public bool zoomOut = false;

    void Awake()
    {
        _cam = GetComponent<Camera>();
        _posOrigin = transform.position;
        _zoomSizeOrigin = _cam.orthographicSize;
    }

    void Update()
    {
        if (zoomOut)
        {
            if (_cam.orthographicSize >= _zoomSizeOrigin - 0.2)
                zoomOut = false;
            
            zoom = false;
            transform.position = Vector3.Lerp(transform.position, _posOrigin, Time.deltaTime * zoomOutSpeed);
            _cam.orthographicSize = Mathf.Lerp(_cam.orthographicSize, _zoomSizeOrigin, Time.deltaTime * zoomOutSpeed);
        }
        if (zoom)
        {
            if (_cam.orthographicSize <= zoomSize + 0.2f)
                zoom = false;
            
            transform.position = Vector3.Lerp(transform.position, cube.transform.position, Time.deltaTime * zoomSpeed);
            _cam.orthographicSize = Mathf.Lerp(_cam.orthographicSize, zoomSize, Time.deltaTime * zoomSpeed);
        }
    }
}
