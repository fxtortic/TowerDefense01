using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "TowerDefense/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public Sprite sprite;
    public float maxHealth;
    public float moveSpeed;       
    public int cost;            
    public bool immuneToFreeze; 
    public Color healthBarColor = Color.red;
}
