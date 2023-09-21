using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class JumpManager : MonoBehaviour
{
    [SerializeField] CameraManager cam;
    [SerializeField] LayerMask platformLayerMask; 
    [SerializeField] DirectionalArrow directionalArrow;
    
    Rigidbody2D _rb2d;
    BoxCollider2D _collider2D;
    Animator _animator;
    
    [SerializeField] float jumpChargeSpeed;
    [SerializeField] float maxJumpForce;
    [SerializeField] float instantJumpForce;
    [SerializeField] float aerialSlowMotionTimeScale;

    const float JumpCooldown = 0.05f;
    
    float _currentJumpForce, _currentJumpCooldown;
    Vector2 _jumpVector;
    public JumpStyles currentJumpStyle;
    bool _onCooldown, _isGrounded, _haveAerial, _instantJumpPressed, _lockedCharge;
    
    // Animation variables
    static readonly int GroundChargeAnim = Animator.StringToHash("groundCharge");
    static readonly int FloatAnim = Animator.StringToHash("Floating");
    static readonly int LandingAnim = Animator.StringToHash("Landing");

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
        if (_isGrounded)
        {
            // LANDING
            if (currentJumpStyle != JumpStyles.GroundJump)
            {
                _haveAerial = true;
                // Reset jump to ground
                currentJumpStyle = JumpStyles.GroundJump;
                _currentJumpForce = 0f;
                
                // Landing animation
                _animator.SetBool(FloatAnim, false); // reset floating anim
                _animator.SetTrigger(LandingAnim);
                // Remove slow motion effect
                ResetTimeScale();
                
                directionalArrow.Active(false);
            }
        }
        // IN AIR
        else if (!_isGrounded && !_onCooldown)
        {
            if (_rb2d.velocity.y <= 4f)
            {
                _animator.SetBool(FloatAnim, true);
                _animator.ResetTrigger(LandingAnim);
            }

            if (_haveAerial && currentJumpStyle != JumpStyles.AerialJump)
            {
                Debug.Log("PASOUC");

                if (_currentJumpForce > 0f)
                {
                    _currentJumpForce = 0f;
                    _lockedCharge = true; // False on key released
                    _animator.SetBool(GroundChargeAnim, false);
                }

                currentJumpStyle = JumpStyles.AerialJump;
            }
        }

        // ---- PRESS ---- //
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentJumpStyle == JumpStyles.GroundJump)
            {
                // Animate ground charging
                _animator.SetBool(GroundChargeAnim, true);

                directionalArrow.Active(false);
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
                
                directionalArrow.Active(true);
            }
            
            // DO INSTANT JUMP
            else if (currentJumpStyle == JumpStyles.InstantJump && !_onCooldown)
            {
                _currentJumpForce = instantJumpForce;
                Jump();
                _instantJumpPressed = true;
            }
        }
        
        // ---- CHARGE ---- //
        if (Input.GetKey(KeyCode.Space) && !_lockedCharge)
        {
            // CHARGE JUMP
            if (currentJumpStyle != JumpStyles.GroundJump &&
                currentJumpStyle != JumpStyles.AerialJump ||
                _instantJumpPressed ||
                _onCooldown) return;
            
            _currentJumpForce += Time.deltaTime * jumpChargeSpeed;
            // Limit jump force
            if (_currentJumpForce > maxJumpForce) _currentJumpForce = maxJumpForce;
        }
        
        // ---- RELEASE ---- //
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (_instantJumpPressed)
                _instantJumpPressed = false;

            if (_lockedCharge)
                _lockedCharge = false;
            
            // JUMP
            switch (currentJumpStyle)
            {
                case JumpStyles.Null:
                    break;
                
                // GROUND //
                case JumpStyles.GroundJump:
                    if (!_isGrounded)
                    {
                        _currentJumpForce = 0f;
                        return;
                    }

                    // Animate ground jump
                    _animator.SetBool(GroundChargeAnim, false);
                    
                    Jump();
                    
                    // Set next jump style
                    currentJumpStyle = JumpStyles.AerialJump;
                    break;
                
                // AERIAL //
                case JumpStyles.AerialJump:
                    if (_isGrounded)
                    {
                        _currentJumpForce = 0f;
                        return;
                    }
                    
                    // Multiply jump force
                    _currentJumpForce *= 5;
                    if (_currentJumpForce > 40)
                        _currentJumpForce = 40;
                    Debug.Log(_currentJumpForce);
                    
                    // Remove slow motion effect
                    ResetTimeScale();

                    Jump();
                    
                    // Set next jump style
                    currentJumpStyle = JumpStyles.Null;

                    _haveAerial = false;
                    directionalArrow.Active(false);
                    break;
                
                case JumpStyles.InstantJump:
                    ResetTimeScale();
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    void SetJumpVector()
    {
        if (_currentJumpForce < 6f)
            _currentJumpForce = 6f;
        
        if (currentJumpStyle == JumpStyles.GroundJump){}
            _jumpVector = new Vector2(0, _currentJumpForce);
        if (currentJumpStyle == JumpStyles.AerialJump)
            _jumpVector = directionalArrow.rotation.normalized * _currentJumpForce;
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
        const float extraHeightTest = .1f;
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