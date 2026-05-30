using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyData data;

    [Header("Health Bar (дочірні об'єкти)")]
    public Transform healthBarFill;
    public SpriteRenderer healthBarBg;

    private float currentHP;
    private float currentSpeed;
    private int currentWaypointIndex;
    private WaypointPath path;
    private SpriteRenderer sr;

    private float slowTimer;
    private float slowMultiplier = 1f;

    public float PathProgress { get; private set; }

    public void Initialize(EnemyData enemyData, WaypointPath waypointPath)
    {
        data = enemyData;
        path = waypointPath;
        currentHP = data.maxHealth;
        currentSpeed = data.moveSpeed;
        currentWaypointIndex = 0;
        slowTimer = 0f;
        slowMultiplier = 1f;
        PathProgress = 0f;

        sr = GetComponent<SpriteRenderer>();
        if (sr != null && data.sprite != null)
        {
            sr.sprite = data.sprite;
        }
        sr.sortingOrder = 2;

        transform.position = path.waypoints[0].position;
        UpdateHealthBar();
    }

    void Update()
    {
        if (path == null || path.waypoints.Count == 0) return;
        if (currentWaypointIndex >= path.waypoints.Count)
        {
            ReachBase();
            return;
        }

        if (slowTimer > 0f)
        {
            slowTimer -= Time.deltaTime;
            if (slowTimer <= 0f) slowMultiplier = 1f;
        }

        Vector3 target = path.waypoints[currentWaypointIndex].position;
        float speed = currentSpeed * slowMultiplier;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            currentWaypointIndex++;
        }

        PathProgress = path.GetProgress(transform.position, currentWaypointIndex);
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        UpdateHealthBar();

        if (currentHP <= 0f)
        {
            Die();
        }
    }

    public void ApplySlow(float multiplier, float duration)
    {
        if (data.immuneToFreeze) return;
        slowMultiplier = multiplier;
        slowTimer = duration;
    }

    void UpdateHealthBar()
    {
        if (healthBarFill == null) return;
        float ratio = Mathf.Clamp01(currentHP / data.maxHealth);
        healthBarFill.localScale = new Vector3(ratio, 1f, 1f);
    }

    void Die()
    {
        DeathEffect.Instance?.SpawnAt(transform.position);
        AudioManager.Instance?.Play(AudioManager.Instance.enemyDeathSound);
        GameManager.Instance.OnEnemyKilled(data);
        ReturnToPool();
    }

    void ReachBase()
    {
        AudioManager.Instance?.Play(AudioManager.Instance.enemyReachBaseSound);
        GameManager.Instance.OnEnemyReachedBase(data);
        ReturnToPool();
    }

    void ReturnToPool()
    {
        ObjectPool.Instance.Return("Enemy", gameObject);
    }
}
