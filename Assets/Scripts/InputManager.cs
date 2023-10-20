using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    [SerializeField] Texture2D cursorTexture;
    public JumpManager cube;
    Vector2 _cursorHotSpot;
    bool _lockInput;

    void Awake()
    {
        _cursorHotSpot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
        Cursor.SetCursor(cursorTexture, _cursorHotSpot, CursorMode.ForceSoftware);
    }

    void Update()
    {
        if (_lockInput) return;
        
        // RESET LEVEL
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Remove slow motion effect
            cube.StopAerialChargeEffect();
            PlayerManager.Instance.KillPlayer();
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            cube.KeyPressed();
            return;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            cube.KeyReleased();
            return;
        }
    }

    public void LockInput(bool value)
    {
        _lockInput = value;
    }
}
