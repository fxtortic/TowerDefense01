using UnityEngine;
using System.Collections.Generic;

public class Tower : MonoBehaviour
{
    public TowerData data;

    private float fireCooldown;
    private SpriteRenderer sr;
    private Transform firePoint;

    public void Initialize(TowerData towerData)
    {
        data = towerData;
        fireCooldown = 0f;

        sr = GetComponent<SpriteRenderer>();
        if (sr != null && data.sprite != null)
            sr.sprite = data.sprite;
        sr.sortingOrder = 3;

        firePoint = transform;
    }

    void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.Battle) return;

        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f)
        {
            Enemy target = FindTarget();
            if (target != null)
            {
                Fire(target);
                fireCooldown = 1f / data.fireRate;
            }
        }
    }

    Enemy FindTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, data.range);
        Enemy bestTarget = null;
        float bestProgress = -1f;

        foreach (var hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null && enemy.gameObject.activeSelf)
            {
                if (enemy.PathProgress > bestProgress)
                {
                    bestProgress = enemy.PathProgress;
                    bestTarget = enemy;
                }
            }
        }

        return bestTarget;
    }

    void Fire(Enemy target)
    {
        GameObject projGo = ObjectPool.Instance.Get("Projectile", firePoint.position, Quaternion.identity);
        if (projGo != null)
        {
            Projectile proj = projGo.GetComponent<Projectile>();
            if (proj != null)
            {
                proj.Initialize(target, data);
            }
        }
        AudioManager.Instance?.Play(AudioManager.Instance.shootSound);
    }

    void OnDrawGizmosSelected()
    {
        if (data != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, data.range);
        }
    }
}
