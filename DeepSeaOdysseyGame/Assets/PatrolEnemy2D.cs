using UnityEngine;

public class PatrolEnemy2D : MonoBehaviour
{
    public enum State { Patrol, Chase, Attack }
    [Header("Текущее состояние (смотри в игре)")]
    public State currentState = State.Patrol;

    [Header("Настройки врага")]
    [SerializeField] private Transform[] waypoints; // Точки, между которыми он плавает
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 3.5f;
    [SerializeField] private float detectionRadius = 5f; // Дальность зрения
    [SerializeField] private float attackRadius = 1.2f;  // Дистанция удара
    [SerializeField] private LayerMask playerLayer;      // Слой игрока
    [SerializeField] private LayerMask wallLayer;        // Слой стен

    private int currentWaypointIndex = 0;
    private Transform player;

    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Patrol:
                PatrolLogic();
                CheckForPlayerWithRaycast();
                break;
            case State.Chase:
                ChaseLogic();
                break;
            case State.Attack:
                AttackLogic();
                break;
        }
    }

    void PatrolLogic()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        Transform target = waypoints[currentWaypointIndex];
        transform.position = Vector2.MoveTowards(transform.position, target.position, patrolSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.position) < 0.2f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    void CheckForPlayerWithRaycast()
    {
        if (player == null) return;
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRadius)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            // Луч зрения: видит игрока, но упирается в стены
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, detectionRadius, playerLayer | wallLayer);

            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                currentState = State.Chase; // Увидел игрока — переходим в погоню!
            }
        }
    }

    void ChaseLogic()
    {
        if (player == null) return;
        transform.position = Vector2.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= attackRadius)
        {
            currentState = State.Attack;
        }
        else if (distance > detectionRadius * 1.4f)
        {
            currentState = State.Patrol; // Игрок уплыл далеко — возвращаемся в патруль
        }
    }

    void AttackLogic()
    {
        Debug.Log("Patroller: Удар по батискафу!");
        currentState = State.Patrol;
    }

    // Рисует круги обзора в окне Scene (для наглядности)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}