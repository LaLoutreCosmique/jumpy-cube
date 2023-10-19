using System.Collections;
using UnityEngine;

public class KillColliders : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        
        PlayerManager.Instance.KillPlayer();
    }
}
