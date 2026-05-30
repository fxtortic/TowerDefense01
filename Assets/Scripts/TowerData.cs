using UnityEngine;

[CreateAssetMenu(fileName = "NewTower", menuName = "TowerDefense/TowerData")]
public class TowerData : ScriptableObject
{
    public string towerName;
    public Sprite sprite;
    public Sprite projectileSprite;
    public int price;
    public float damage;
    public float fireRate;         
    public float range;            
    public bool isAoE;             
    public float aoeDamage;        
    public float aoeRadius;        
    public bool isFreezer;         
    public float slowAmount;       
    public float slowDuration;     
    public float projectileSpeed = 8f;
    public Color rangeColor = new Color(1f, 1f, 1f, 0.2f);
}
