using UnityEngine;

public class OrbsManager : MonoBehaviour
{
    [SerializeField] InstantOrb[] instantOrbs;

    void Awake()
    {
        instantOrbs = GetComponentsInChildren<InstantOrb>();
    }

    public void UpdateCubeObject(Transform cube)
    {
        foreach (var orb in instantOrbs)
        {
            orb.SetTarget(cube);
        }
    }
}
