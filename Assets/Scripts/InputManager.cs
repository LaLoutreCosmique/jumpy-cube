using System;
using System.Collections;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] Texture2D[] cursorTextureArray;
    [SerializeField] float frameRate;
    
    int _currentFrame, _frameCount;
    float _frameTimer;
    Vector2 _cursorHotSpot;
    bool _animateCursorCharge;

    void Awake()
    {
        _frameCount = cursorTextureArray.Length - 1;
        _cursorHotSpot = new Vector2(cursorTextureArray[0].width / 2, cursorTextureArray[0].height / 2);
    }

    void Start()
    {
        if (!_animateCursorCharge)
            Cursor.SetCursor(cursorTextureArray[0], _cursorHotSpot, CursorMode.ForceSoftware);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            StartCursorChargeAnim();
        else if (Input.GetKeyUp(KeyCode.Space))
            StartCoroutine(StopCursorChargeAnim());
        
        if (_animateCursorCharge && _currentFrame < _frameCount)
        {
            _frameTimer -= Time.deltaTime;
            if (_frameTimer <= 0f)
            {
                _frameTimer += frameRate;
                _currentFrame++;
                Cursor.SetCursor(cursorTextureArray[_currentFrame], _cursorHotSpot, CursorMode.ForceSoftware);
            }
        }
    }

    public void StartCursorChargeAnim()
    {
        _currentFrame = 0;
        _animateCursorCharge = true;
    }

    public IEnumerator StopCursorChargeAnim()
    {
        _animateCursorCharge = false;
        yield return new WaitForSeconds(1);
        Start();
    }
}
