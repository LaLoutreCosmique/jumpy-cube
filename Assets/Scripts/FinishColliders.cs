using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishColliders : MonoBehaviour
{
    JumpManager cube;
    [SerializeField] float timeToSkipLevel = 1;
    public bool finished;
    
    void OnCollisionStay2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Player") || finished) return;

        Rigidbody2D rb2d = other.gameObject.GetComponent<Rigidbody2D>();
        cube = other.gameObject.GetComponent<JumpManager>();

        if (!cube._isGrounded)
        {
            if (cube._lockedGroundRecovery)
                cube._lockedGroundRecovery = false;
            return;
        }
        
        cube.currentJumpStyle = JumpManager.JumpStyles.Null;
        cube._lockedGroundRecovery = true;
        
        if (rb2d.velocity.magnitude >= 0.01f) return;
        
        finished = true;
        other.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
        StartCoroutine(GoToNextLevel());
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        cube.currentJumpStyle = JumpManager.JumpStyles.AerialJump;
        cube._lockedGroundRecovery = false;
        cube._currentJumpForce = 0f;
    }

    IEnumerator GoToNextLevel()
    {
        yield return new WaitForSeconds(timeToSkipLevel);
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}