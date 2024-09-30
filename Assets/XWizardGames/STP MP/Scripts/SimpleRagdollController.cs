#if XWIZARD_GAMES_STP_MP
using PolymindGames;
using UnityEngine;

namespace XWizardGames.STP_MP
{
    public class SimpleRagdollController : MonoBehaviour
    {
        private Rigidbody[] m_RagdollRbs;
        private Collider[] m_Colliders;

        [SerializeField] private HealthManager m_HealthManager;
        [SerializeField] private Animator m_Animator;

        private void Start()
        {
            m_RagdollRbs = GetComponentsInChildren<Rigidbody>(true);
            m_Colliders = GetComponentsInChildren<Collider>(true);

            m_HealthManager.Death += EnableRagdoll;
            m_HealthManager.Respawn += DisableRagdoll;

            DisableRagdoll();
        }

        private void EnableRagdoll()
        {
            SetRagdoll(true);
        }
        private void DisableRagdoll()
        {
            SetRagdoll(false);
        }

        private void SetRagdoll(bool value)
        {
            foreach (Rigidbody r in m_RagdollRbs)
            {
                r.isKinematic = !value;
            }
            foreach (Collider c in m_Colliders)
            {
                c.enabled = value;
            }
            m_Animator.enabled = !value;
        }
    }
}
#endif