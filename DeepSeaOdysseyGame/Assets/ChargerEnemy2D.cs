using UnityEngine;
using System.Collections;

public class ChargerEnemy2D : MonoBehaviour
{
    public enum State { Patrol, WindUp, Charge }
    public State currentState = State.Patrol;

    [Header("Настройки тарана")]
    [SerializeField] private float chargeSpeed = 9f;
    [SerializeField] private float windUpTime = 0.7f;    // Задержка перед рывком
    [SerializeField] private float detectionRadius = 5.5f;
    [SerializeField] private float cooldownTime = 1.5f;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Transform player;
    private Vector2 chargeDirection;
    private bool isCharging = false;

    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isCharging || player == null) return;

        if (Vector2.Distance(transform.position, player.position) <= detectionRadius)
        {
            StartCoroutine(ChargeRoutine());
        }
    }

    IEnumerator ChargeRoutine()
    {
        isCharging = true;

        // 1. Состояние: WindUp (Замирает и желтеет перед атакой)
        currentState = State.WindUp;
        chargeDirection = (player.position - transform.position).normalized;
        if (spriteRenderer != null) spriteRenderer.color = Color.yellow;
        yield return new WaitForSeconds(windUpTime);

        // 2. Состояние: Charge (Быстрый рывок по прямой)
        currentState = State.Charge;
        if (spriteRenderer != null) spriteRenderer.color = Color.magenta;

        float timer = 0f;
        while (timer < 0.45f)
        {
            transform.Translate(chargeDirection * chargeSpeed * Time.deltaTime, Space.World);
            timer += Time.deltaTime;
            yield return null;
        }

        // 3. Отдых после рывка
        if (spriteRenderer != null) spriteRenderer.color = Color.green;
        yield return new WaitForSeconds(cooldownTime);
        currentState = State.Patrol;
        isCharging = false;
    }
}