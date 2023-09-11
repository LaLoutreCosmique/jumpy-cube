using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishColliders : MonoBehaviour
{
    [SerializeField] Rigidbody2D cube;

    [SerializeField] float timeToSkipLevel = 1;
    public bool finished;
    
    void OnCollisionStay2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Player") || !(cube.velocity.magnitude < 0.01f) || finished) return;
        
        finished = true;
        cube.GetComponent<SpriteRenderer>().color = Color.green;
        StartCoroutine(GoToNextLevel());
    }

    IEnumerator GoToNextLevel()
    {
        yield return new WaitForSeconds(timeToSkipLevel);
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}