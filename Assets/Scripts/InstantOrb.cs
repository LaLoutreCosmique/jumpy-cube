using UnityEngine;

public class InstantOrb : MonoBehaviour
{
    Transform _cube;
    JumpManager _jumpManager;
    [SerializeField] Transform dot; // To make it bigger (base scale = 0.1, max scale = 0.7)
    
    float _distanceToCube;
    Vector3 _maxDotScale, _minDotScale;
    [SerializeField] float detectionDist = 4f;
    [SerializeField] float activationTolerance = 60f;

    [SerializeField] float orbCooldown = 2f;
    public float currentOrbCooldown;

    SpriteRenderer[] _renderers;

    void Awake()
    {
        _minDotScale = dot.localScale;
        _maxDotScale = new Vector3(0.7f, 0.7f, 0.7f);
        currentOrbCooldown = orbCooldown;
        GetComponentsInChildren<Renderer>();
        _renderers = GetComponentsInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (currentOrbCooldown < orbCooldown)
            currentOrbCooldown += Time.deltaTime;
        else
        {
            foreach (var rend in _renderers)
            {
                var color = rend.color;
                rend.color = new Color(color.r, color.g, color.b, 1f);
            }
        }
        
        //Debug.Log(currentOrbCooldown);
        
        if (!_cube) return;
        
        _distanceToCube = Vector3.Distance(_cube.position, transform.position);
        
        // 100% = at the point, 0% and - = at detectionDist or farther
        float proximityPercentage = (detectionDist - _distanceToCube) / detectionDist * 100;
        
        if (_distanceToCube <= detectionDist)
        {
            Vector3 newDotScale = proximityPercentage * (_maxDotScale / activationTolerance);
            
            // Limit scale of the dot
            if (newDotScale.x > _minDotScale.x && newDotScale.x < _maxDotScale.x)
                dot.localScale = newDotScale;

            if (!(currentOrbCooldown >= orbCooldown)) return;
            
            if (proximityPercentage >= activationTolerance && 
                _jumpManager.currentJumpStyle != JumpManager.JumpStyles.InstantJump &&
                currentOrbCooldown >= orbCooldown)
            {
                // GIVE INSTANT JUMP
                _jumpManager.GetInstantJump(this);
            }
            else if (proximityPercentage < activationTolerance)
            {
                if (_jumpManager.currentJumpStyle == JumpManager.JumpStyles.InstantJump)
                {
                    // REMOVE INSTANT JUMP
                    _jumpManager.RemoveInstantJump();
                }
            }
        }
    }

    public void StartCooldown()
    {
        currentOrbCooldown = 0f;

        foreach (var rend in _renderers)
        {
            var color = rend.color;
            rend.color = new Color(color.r, color.g, color.b, 0.3f);
        }
    }

    public void SetTarget(Transform newCube)
    {
        _cube = newCube;
        _jumpManager = _cube.GetComponent<JumpManager>();
    }
}
