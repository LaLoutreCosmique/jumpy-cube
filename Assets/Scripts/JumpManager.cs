using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class JumpManager : MonoBehaviour
{
    [SerializeField] CameraManager cam;
    [SerializeField] LayerMask platformLayerMask;
    
    Rigidbody2D _rb2d;
    BoxCollider2D _collider2D;
    Animator _animator;
    
    [SerializeField] float jumpChargeSpeed;
    [SerializeField] float maxJumpForce;
    [SerializeField] float instantJumpForce;
    [SerializeField] float aerialSlowMotionTimeScale;

    const float JumpCooldown = 0.5f;
    
    public JumpDirections currentJumpDirection;
    float _currentJumpForce, _currentJumpCooldown;
    Vector2 _jumpVector;
    public JumpStyles currentJumpStyle;
    bool _onCooldown, _isGrounded, _inAir, _haveAerial, _instantJumpPressed;
    
    // Animation variables
    static readonly int GroundChargeAnim = Animator.StringToHash("groundCharge");
    static readonly int FloatAnim = Animator.StringToHash("Floating");
    static readonly int LandingAnim = Animator.StringToHash("Landing");

    public enum JumpDirections
    {
        Up,
        Down,
        Left,
        Right
    }

    public enum JumpStyles
    {
        Null, // Can't jump
        GroundJump,
        AerialJump,
        InstantJump
    }

    void Awake()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<BoxCollider2D>();
        _animator = GetComponent<Animator>();
        currentJumpDirection = JumpDirections.Up;
        currentJumpStyle = JumpStyles.GroundJump;
        _haveAerial = true;
    }

    void Update()
    {
        // RESET LEVEL
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Remove slow motion effect
            ResetTimeScale();
            // Reload scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
            
        _isGrounded = IsGrounded();
        
        // JUMP COOLDOWN
        if (_currentJumpCooldown < JumpCooldown)
        {
            _currentJumpCooldown += Time.deltaTime;
            _onCooldown = true;
        }
        else
            _onCooldown = false;
        
        
        // ON THE GROUND
        if (_isGrounded && !_onCooldown)
        {
            // LANDING
            if (_inAir)
            {
                _inAir = false;
                _haveAerial = true;
                // Reset jump to ground
                currentJumpStyle = JumpStyles.GroundJump;
                _currentJumpForce = 0f;
                
                // Landing animation
                _animator.SetBool(FloatAnim, false); // reset floating anim
                _animator.SetTrigger(LandingAnim);
                
                // Remove slow motion effect
                ResetTimeScale();
            }
        }
        // IN AIR
        else if (_inAir)
        {
            if (_rb2d.velocity.y <= 4f)
                _animator.SetBool(FloatAnim, true);
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentJumpStyle == JumpStyles.GroundJump)
            {
                // Animate ground charging
                _animator.SetBool(GroundChargeAnim, true);
            }
            
            // START CHARGING AERIAL JUMP
            else if (currentJumpStyle == JumpStyles.AerialJump)
            {
                // Slow motion when charging aerial jump
                Time.timeScale = aerialSlowMotionTimeScale;
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
                
                // Camera zooming to the cube
                cam.zoom = true;
                
                // Animate aerial charging
                //_animator.SetBool(AerialChargeAnim, true);
            }
            
            // DO INSTANT JUMP
            else if (currentJumpStyle == JumpStyles.InstantJump && !_onCooldown)
            {
                _currentJumpForce = instantJumpForce;
                currentJumpDirection = JumpDirections.Up;
                Jump();
                _instantJumpPressed = true;
            }
        }
        if (Input.GetKey(KeyCode.Space))
        {
            // CHARGE JUMP
            if (currentJumpStyle != JumpStyles.GroundJump &&
                currentJumpStyle != JumpStyles.AerialJump ||
                _instantJumpPressed ||
                _onCooldown) return;
            
            Debug.Log(_currentJumpForce);
            
            _currentJumpForce += Time.deltaTime * jumpChargeSpeed;
            // Limit jump force
            if (_currentJumpForce > maxJumpForce) _currentJumpForce = maxJumpForce;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (_instantJumpPressed)
            {
                _instantJumpPressed = false;
            }
            
            // JUMP
            switch (currentJumpStyle)
            {
                case JumpStyles.Null:
                    break;
                
                case JumpStyles.GroundJump:
                    if (!_isGrounded)
                    {
                        _currentJumpForce = 0f;
                        return;
                    }
                    
                    currentJumpDirection = JumpDirections.Up;
                    _inAir = true;
                    
                    // Animate ground jump
                    _animator.SetBool(GroundChargeAnim, false);
                    
                    // Set next jump style
                    currentJumpStyle = JumpStyles.AerialJump;
                    break;
                
                case JumpStyles.AerialJump:
                    if (_isGrounded)
                    {
                        _currentJumpForce = 0f;
                        return;
                    }
                    
                    // Multiply jump force
                    _currentJumpForce *= 2;
                    
                    // Remove slow motion effect
                    ResetTimeScale();
                    
                    // Set direction to jump
                    currentJumpDirection = JumpDirections.Right;
                    
                    // Set next jump style
                    currentJumpStyle = JumpStyles.Null;

                    _haveAerial = false;
                    break;
                
                case JumpStyles.InstantJump:
                    ResetTimeScale();
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            Jump();
        }
    }

    void SetJumpVector()
    {
        if (_currentJumpForce < 4f)
            _currentJumpForce = 4f;
        
        switch (currentJumpDirection)
        {
            case JumpDirections.Up:
                _jumpVector.y = _currentJumpForce;
                break;
            case JumpDirections.Down:
                _jumpVector.y = -_currentJumpForce;
                break;
            case JumpDirections.Right:
                _jumpVector.x = _currentJumpForce;
                break;
            case JumpDirections.Left:
                _jumpVector.x = -_currentJumpForce;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    void Jump()
    {
        SetJumpVector();
        _rb2d.AddForce(_jumpVector, ForceMode2D.Impulse);
        _currentJumpForce = 0f;
        _currentJumpCooldown = 0f;
        _jumpVector = new Vector2();
    }

    bool IsGrounded()
    {
        var bounds = _collider2D.bounds;
        float extraHeightTest = .1f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(
            bounds.center, 
            bounds.size, 
            0f, 
            Vector2.down, 
            extraHeightTest, 
            platformLayerMask);
        
        return raycastHit.collider != null;
    }

    void ResetTimeScale()
    {
        // Remove slow motion effect
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;
        cam.zoomOut = true;
        cam.zoom = false;
    }

    public void GetInstantJump()
    {
        currentJumpStyle = JumpStyles.InstantJump;
    }

    public void RemoveInstantJump()
    {
        currentJumpStyle = _haveAerial ? JumpStyles.AerialJump : JumpStyles.Null;
    }
}