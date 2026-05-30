using UnityEngine;

public class DeathEffect : MonoBehaviour
{
    public static DeathEffect Instance { get; private set; }
    public GameObject deathParticlePrefab;

    void Awake() { Instance = this; }

    public void SpawnAt(Vector3 position)
    {
        if (deathParticlePrefab == null) return;
        GameObject fx = Instantiate(deathParticlePrefab, position, Quaternion.identity);
        Destroy(fx, 1f);
    }
}