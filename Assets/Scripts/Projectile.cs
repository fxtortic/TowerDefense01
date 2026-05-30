using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Enemy target;
    private TowerData towerData;
    private SpriteRenderer sr;
    private float speed;

    public void Initialize(Enemy targetEnemy, TowerData data)
    {
        target = targetEnemy;
        towerData = data;
        speed = data.projectileSpeed;

        sr = GetComponent<SpriteRenderer>();
        if (sr != null && data.projectileSprite != null)
            sr.sprite = data.projectileSprite;
        if (sr != null) sr.sortingOrder = 4;
    }

    void Update()
    {
        if (target == null || !target.gameObject.activeSelf)
        {
            ReturnToPool();
            return;
        }

        Vector3 dir = target.transform.position - transform.position;
        transform.position += dir.normalized * speed * Time.deltaTime;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (dir.magnitude < 0.2f)
        {
            HitTarget();
        }
    }

    void HitTarget()
    {
        AudioManager.Instance?.Play(AudioManager.Instance.hitSound);
        if (target == null) { ReturnToPool(); return; }

        target.TakeDamage(towerData.damage);

        if (towerData.isAoE)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, towerData.aoeRadius);
            foreach (var hit in hits)
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null && enemy != target && enemy.gameObject.activeSelf)
                {
                    enemy.TakeDamage(towerData.aoeDamage);
                }
            }
        }

        if (towerData.isFreezer)
        {
            target.ApplySlow(towerData.slowAmount, towerData.slowDuration);
        }

        ReturnToPool();
    }

    void ReturnToPool()
    {
        ObjectPool.Instance.Return("Projectile", gameObject);
    }
}
