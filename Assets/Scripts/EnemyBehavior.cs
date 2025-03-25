using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBehavior : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float damageAmount = 10f;

    private Transform playerTransform;
    private NavMeshAgent agent;
    private float nextAttackTime;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.angularSpeed = rotationSpeed * 100;
        agent.stoppingDistance = attackRange;
    }

    private void Update()
    {
        if (playerTransform == null) return;

        // Move towards player
        agent.SetDestination(playerTransform.position);

        // Check if can attack
        if (Vector3.Distance(transform.position, playerTransform.position) <= attackRange)
        {
            if (Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    private void Attack()
    {
        PlayerHealth playerHealth = playerTransform.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damageAmount);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw attack range in editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
