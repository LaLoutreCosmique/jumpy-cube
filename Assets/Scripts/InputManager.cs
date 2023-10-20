using System;
using System.Collections;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] Texture2D cursorTexture;
    Vector2 _cursorHotSpot;

    void Awake()
    {
        _cursorHotSpot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
        Cursor.SetCursor(cursorTexture, _cursorHotSpot, CursorMode.ForceSoftware);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            return;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            return;
        }
    }
}
