using Mirror;
using PolymindGames;
using UnityEngine;
using UnityEngine.AI;
using XWizardGames.STP_MP;

namespace XWizardGames.AI
{
    public class SimpleAI : SimpleAIBase, IDamageReceiver
    {
        [SerializeField] private float m_MaxHealth;
        [SerializeField] private float m_RotateSpeed = 2;

        [SerializeField] private float m_AttackDistance = 1f;
        [SerializeField] private float m_AttackCooldown = 1f;

        [SerializeField] private float m_MinDamage = 2;
        [SerializeField] private float m_MaxDamage = 4;

        [SerializeField] private float m_LookForNewTargetTime = 1;
        private float m_CurrentLookForNewTargetTimer = 0;

        [SerializeField] private float m_WanderRadius = 10f;
        [SerializeField] private float m_WanderCooldown = 5f;
        private float m_CurrentWanderCooldown;

        private NavMeshAgent m_Agent;
        private NetworkAnimator m_Animator;
        private Player m_Target;
        private float m_CurretAttackCooldown;

        private STP_MP_NetworkManager m_NetworkManager;
        [SyncVar] private float m_CurrentHealth;

        private void Start()
        {
            m_NetworkManager = STP_MP_NetworkManager.singleton as STP_MP_NetworkManager;
            m_Animator = GetComponent<NetworkAnimator>();
            m_Agent = GetComponent<NavMeshAgent>();

            m_CurrentHealth = m_MaxHealth;
            m_CurrentWanderCooldown = m_WanderCooldown;
        }

        public override void FindTarget()
        {
            m_CurrentLookForNewTargetTimer -= Time.deltaTime;

            if (m_CurrentLookForNewTargetTimer < 0)
            {
                m_CurrentLookForNewTargetTimer = m_LookForNewTargetTime;

                Player closestPlayer = null;
                float closestDistance = float.MaxValue;

                foreach (Player player in m_NetworkManager.Players)
                {
                    float distance = Vector3.Distance(transform.position, player.transform.position);

                    if (distance < closestDistance)
                    {
                        closestPlayer = player;
                        closestDistance = distance;
                    }
                }

                if (closestPlayer != null)
                {
                    m_Target = closestPlayer;
                }
            }
        }

        public override void Attack()
        {
            if (m_Target == null) return;

            float distance = Vector3.Distance(transform.position, m_Target.transform.position);

            if (distance < m_AttackDistance)
            {
                m_CurretAttackCooldown -= Time.deltaTime;
                if (m_CurretAttackCooldown <= 0)
                {
                    m_Animator.SetTrigger("Attack");
                    Invoke(nameof(DamageTarget), 0.4f);
                    m_CurretAttackCooldown = m_AttackCooldown;
                }
            }
        }

        private void DamageTarget()
        {
            float damage = UnityEngine.Random.Range(m_MinDamage, m_MaxDamage);
            m_Target.HealthManager.ReceiveDamageCmd(damage);
        }

        public override void MoveToTarget()
        {
            if (m_Target != null)
            {
                m_Agent.SetDestination(m_Target.transform.position);
            }
        }

        private void Wander()
        {
            m_CurrentWanderCooldown -= Time.deltaTime;

            if (m_CurrentWanderCooldown <= 0)
            {
                m_CurrentWanderCooldown = m_WanderCooldown;

                Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * m_WanderRadius;
                randomDirection += transform.position;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomDirection, out hit, m_WanderRadius, 1))
                {
                    m_Agent.SetDestination(hit.position);
                }
            }
        }

        public override void Update()
        {
            if (!isServer) return;

            FindTarget();
            UpdateAnimator();

            if (m_Target != null)
            {
                MoveToTarget();
                Attack();
                HandleRotation();
            }
            else
            {
                Wander();
            }
        }

        private void HandleRotation()
        {
            if (m_Target == null) return;

            Vector3 direction = m_Target.transform.position - transform.position;
            direction.y = 0f;

            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * m_RotateSpeed);
            }
        }

        private void UpdateAnimator()
        {
            float maxSpeed = m_Agent.speed;
            float currentSpeed = m_Agent.velocity.magnitude;
            float normalizedSpeed = currentSpeed / maxSpeed;

            m_Animator.animator.SetFloat("Movement", normalizedSpeed);
        }

        public DamageResult HandleDamage(float damage, DamageContext context = default)
        {
            m_CurrentHealth -= damage;

            if (m_CurrentHealth <= 0)
            {
                NetworkServer.Destroy(gameObject);
            }

            return DamageResult.Default;
        }
    }
}
