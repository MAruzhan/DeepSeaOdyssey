using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyStats", menuName = "DeepSea/Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    public float moveSpeed = 2.5f;
    public float chaseSpeed = 4f;
    public float detectionRadius = 5f;
    public float windUpTime = 0.6f;
    public float cooldownTime = 1.6f;
    public int damage = 1;
}