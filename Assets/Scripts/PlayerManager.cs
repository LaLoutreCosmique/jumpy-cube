using System.Collections;
using UnityEngine;
using Unity.Mathematics;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }
    
    [SerializeField] GameObject bpPlayer;
    [SerializeField] GameObject playedCube;
    
    [SerializeField] CameraManager cam;
    [SerializeField] OrbsManager orbsManager;
    [SerializeField] InputManager inputManager;
    
    [SerializeField] ParticleSystem deathParticle;

    Vector3 _cubeSpawnPos;
    const float DelayToRespawn = 0.5f;

    void Awake()
    {
        // Set class as a singleton
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        _cubeSpawnPos = playedCube.transform.position;
        inputManager = GetComponent<InputManager>();
    }

    void Start()
    {
        if (orbsManager) orbsManager.UpdateCubeObject(playedCube.transform);
    }

    public void KillPlayer()
    {
        Destroy(playedCube.transform.parent.gameObject);
        // particle anim
        deathParticle.transform.position = playedCube.transform.position;
        deathParticle.Play();

        StartCoroutine(InstantiateNewPlayer(DelayToRespawn));
    }

    IEnumerator InstantiateNewPlayer(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject newPlayer = Instantiate(bpPlayer, new Vector3(), quaternion.identity);
        playedCube = newPlayer.GetComponentInChildren<JumpManager>().gameObject;
        playedCube.transform.position = _cubeSpawnPos;
        cam.cube = playedCube;
        playedCube.GetComponent<JumpManager>().cam = cam;

        if (orbsManager) orbsManager.UpdateCubeObject(playedCube.transform);
    }

    void StartCursorCharge()
    {
        return;
    }
}
