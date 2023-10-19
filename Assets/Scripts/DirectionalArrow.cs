using UnityEngine;

public class DirectionalArrow : MonoBehaviour
{
    [SerializeField] Transform cube;

    Camera _mainCam;
    Vector3 _mousePos;
    Renderer _renderer;

    public Vector3 rotation;

    bool _isActive;

    void Awake()
    {
        _mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        _renderer = GetComponentInChildren<Renderer>();
    }

    void Update()
    {
        if (!_isActive) return;
        
        if (cube.position != transform.position)
        {
            transform.position = cube.position;
        }

        _mousePos = _mainCam.ScreenToWorldPoint(Input.mousePosition);
        
        rotation = _mousePos - transform.position;

        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        
        transform.rotation = Quaternion.Euler(0,0,rotZ);
    }

    public void Active(bool value)
    {
        switch (value)
        {
            case true when !_isActive:
                _isActive = true;
                _renderer.enabled = true;
                break;
            case false when _isActive:
                _isActive = false;
                _renderer.enabled = false;
                break;
        }
    }
}
