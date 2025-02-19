using Mirror;
using PolymindGames.PoolingSystem;
using PolymindGames.WieldableSystem.Effects;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using XWizardGames.STP_MP;

namespace PolymindGames.WieldableSystem
{
    [AddComponentMenu("PolymindGames/Wieldables/Firearms/Shooters/Parabolic Shooter")]
    public class FirearmParabolicShooter : FirearmShooterBehaviour
    {
        public override int AmmoPerShot => m_AmmoPerShot;
        protected ParabolicProjectileBase ProjectilePrefab => m_Projectile;
        protected LayerMask HitMask => m_HitMask;
        protected float Gravity => m_Gravity;
        protected float Speed => m_Speed;

        [Title("Ammo")]
        
        [SerializeField, Range(0, 10)]
        private int m_AmmoPerShot = 1;

        [Title("Projectile")]

        [SerializeField]
        private LayerMask m_HitMask = Physics.DefaultRaycastLayers;
        
        [SerializeField, NotNull]
        private ParabolicProjectileBase m_Projectile;

        [SpaceArea]
        
        [SerializeField, Range(1, 30)] 
        [Tooltip("The amount of projectiles that will be spawned in the world")]
        private int m_Count = 1;

        [SerializeField, Range(1f, 1000f)]
        private float m_Speed = 75f;

        [SerializeField, Range(0f, 100f)]
        [Tooltip("The gravity for the projectile.")]
        private float m_Gravity = 9.8f;

        [SerializeField, Range(0f, 100f)]
        private float m_MinSpread = 0.75f;

        [SerializeField, Range(0f, 100f)]
        private float m_MaxSpread = 1.5f;
        
        [Title("Effects")]

        [SerializeField]
        private DynamicEffectCollection m_ShootEffects;

        [SpaceArea]

        [SerializeField, ReorderableList(HasLabels = false)]
        private WieldableVFXBehaviour[] m_VisualEffects;

        [SerializeField] private FirearmParabolicShooterNetworked m_NetworkedParabolicShooter;

        public override void Shoot(float accuracy, IProjectileEffect effect, float value)
        {
            ICharacter character = Wieldable.Character;
            
            // Spawn Projectile(s).
            float spread = Mathf.Lerp(m_MinSpread, m_MaxSpread, 1f - accuracy);
            for (int i = 0; i < m_Count; i++)
            {
                Ray ray = PhysicsUtils.GenerateRay(character.ViewTransform, spread);
                //SpawnProjectile(ray, effect, value);
                m_NetworkedParabolicShooter.SpawnProjectile(ray.origin, ray.direction, value, spread);
            }

            // Visual Effects.
            for (int i = 0; i < m_VisualEffects.Length; i++)
                m_VisualEffects[i].PlayEffect();

            // Effects.
            m_ShootEffects.PlayEffects(Wieldable, value);
        }

        protected virtual void SpawnProjectile(Ray ray, IProjectileEffect effect, float speedMod)
        {
            var projectile = PoolingManager.GetObject(m_Projectile.gameObject, ray.origin, Quaternion.LookRotation(ray.direction)).GetComponent<IParabolicProjectile>();
            projectile.Launch(Wieldable.Character, ray.origin, ray.direction * (m_Speed * speedMod), effect, m_HitMask, m_Gravity);
        }

        public void SpawnProjectileNetwork(Vector3 rayorigin, Vector3 direction, float speedMod, float spread)
        {
            ICharacter character = Wieldable.Character;

            //Ray ray = PhysicsUtils.GenerateRay(character.ViewTransform, spread);
            IParabolicProjectile projectile = Instantiate(m_Projectile, rayorigin, Quaternion.LookRotation(direction)).GetComponent<IParabolicProjectile>();
            NetworkServer.Spawn(projectile.gameObject, character.gameObject);
            Debug.Log(GetComponent<IProjectileEffect>());
            projectile.Launch(Wieldable.Character, rayorigin, direction * (m_Speed * speedMod), GetComponent<IProjectileEffect>(), m_HitMask, m_Gravity);

        }

        protected override void Awake()
        {
            base.Awake();
            PoolingManager.CreatePool(m_Projectile.gameObject, 3, 10, 120f);
        }
    }
}
