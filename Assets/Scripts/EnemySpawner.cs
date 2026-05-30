using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    private Coroutine spawnCoroutine;

    public void SpawnWave(List<EnemyData> wave, float interval)
    {
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);
        spawnCoroutine = StartCoroutine(SpawnSequence(wave, interval));
    }

    IEnumerator SpawnSequence(List<EnemyData> wave, float interval)
    {
        WaypointPath path = GameManager.Instance.waypointPath;
        Vector3 spawnPos = path.waypoints[0].position;

        for (int i = 0; i < wave.Count; i++)
        {
            GameObject go = ObjectPool.Instance.Get("Enemy", spawnPos, Quaternion.identity);
            if (go != null)
            {
                Enemy enemy = go.GetComponent<Enemy>();
                if (enemy != null)
                    enemy.Initialize(wave[i], path);
            }

            float delay = interval + Random.Range(-0.2f, 0.2f);
            yield return new WaitForSeconds(Mathf.Max(0.3f, delay));
        }
    }

    public void ClearAll()
    {
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);
        ObjectPool.Instance?.ReturnAll("Enemy");
    }
}
