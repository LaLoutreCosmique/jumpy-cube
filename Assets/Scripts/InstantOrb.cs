using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class InstantOrb : MonoBehaviour
{
    [SerializeField] Transform cube;
    JumpManager _jumpManager;
    [SerializeField] Transform dot; // To make it bigger (base scale = 0.1, max scale = 0.7)
    
    float _distanceToCube;
    Vector3 _maxDotScale, _minDotScale;
    [SerializeField] float detectionDist = 4f;
    [SerializeField] float activationTolerance = 60f;
    
    public bool orbUsed;

    void Awake()
    {
        _jumpManager = cube.GetComponent<JumpManager>();
        _minDotScale = dot.localScale;
        _maxDotScale = new Vector3(0.7f, 0.7f, 0.7f);
    }

    void FixedUpdate()
    {
        _distanceToCube = Vector3.Distance(cube.position, transform.position);
        
        // 100% = at the point, 0% and - = at detectionDist or farther
        float proximityPercentage = (detectionDist - _distanceToCube) / detectionDist * 100;
        
        if (_distanceToCube <= detectionDist)
        {
            Vector3 newDotScale = proximityPercentage * (_maxDotScale / activationTolerance);
            
            // Limit scale of the dot
            if (newDotScale.x > _minDotScale.x && newDotScale.x < _maxDotScale.x)
                dot.localScale = newDotScale;

            if (proximityPercentage >= activationTolerance && 
                _jumpManager.currentJumpStyle != JumpManager.JumpStyles.InstantJump &&
                !orbUsed)
            {
                // GIVE INSTANT JUMP
                _jumpManager.GetInstantJump();
            }
            else if (_jumpManager.currentJumpStyle == JumpManager.JumpStyles.InstantJump)
            {
                // REMOVE INSTANT JUMP
                _jumpManager.RemoveInstantJump();
            }
        }
    }
}
