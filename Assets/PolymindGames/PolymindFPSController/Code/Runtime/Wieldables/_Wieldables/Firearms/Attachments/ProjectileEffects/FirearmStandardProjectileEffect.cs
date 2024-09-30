using PolymindGames.Surfaces;
using UnityEngine;

namespace PolymindGames.WieldableSystem
{
    [AddComponentMenu("PolymindGames/Wieldables/Firearms/BulletEffects/Standard Projectile Effect")]
    public class FirearmStandardProjectileEffect : FirearmProjectileEffectBehaviour
    {
        private enum FalloffType
        {
            Distance,
            Speed
        }

        [SerializeField, Range(0f, 1000f)]
        [Tooltip("The maximum damage at close range.")]
        private float m_Damage = 15f;
        
        [SerializeField, Range(0f, 1000f)]
        [Tooltip("The impact impulse that will be transferred to the rigidbodies at contact.")]
        private float m_Force = 15f;

        [Title("Falloff")]
        
        [SerializeField]
        private FalloffType m_FalloffType;

        [SerializeField, Range(0f, 1000f)]
        private float m_MinFalloffThreshold = 20f;

        [SerializeField, Range(0f, 1000f)]
        private float m_MaxFalloffThreshold = 100f;
        
        
        public override void DoHitEffect(RaycastHit hit, Vector3 hitDirection, float speed, float travelledDistance, ICharacter source)
        {
            // Calculate the falloff damage and impulse mod.
            float falloffMod = m_FalloffType switch
            {
                FalloffType.Distance => (travelledDistance - m_MinFalloffThreshold) / (m_MaxFalloffThreshold - m_MinFalloffThreshold),
                FalloffType.Speed => (speed - m_MinFalloffThreshold) / (m_MaxFalloffThreshold - m_MinFalloffThreshold),
                _ => 1f
            };

            falloffMod = 1 - Mathf.Clamp01(falloffMod);

            float impulse = m_Force * falloffMod;
            float damage = m_Damage * falloffMod;
            
            // Apply an impact impulse.
            if (hit.rigidbody != null)
                hit.rigidbody.AddForceAtPosition(hitDirection * impulse, hit.point, ForceMode.Impulse);

            // Apply damage to any found receiver.



            IDamageReceiver receiver = hit.collider.GetComponentInChildren<IDamageReceiver>();
            Debug.Log(hit.collider);
            Debug.Log(receiver);

            if (receiver != null)
                receiver.HandleDamage(m_Damage, new DamageContext(m_DamageType, hit.point, hitDirection * impulse, hit.normal, Wieldable.Character));

            
            SurfaceManager.SpawnEffect(hit, SurfaceEffects.BulletHit, 1f, true);
        }
    }
}
