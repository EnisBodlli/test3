using Mirror;
using PolymindGames.Surfaces;
using PolymindGames.WieldableSystem.Effects;
using UnityEngine;
using XWizardGames.STP_MP;


namespace PolymindGames.WieldableSystem
{
    [AddComponentMenu("PolymindGames/Wieldables/Melee/Basic Swing")]
	public class BasicMeleeSwing : MeleeSwingBehaviour
	{
		public override float SwingDuration => m_AttackThreeshold;
		public override float AttackEffort => m_AttackEffort;
		protected RaycastHit HitInfo => m_HitInfo;

		[Title("Attack")]

		[SerializeField]
		protected DamageType m_DamageType = DamageType.Hit;

		[SerializeField, Range(0f, 3f)]
		protected float m_AttackThreeshold = 0.3f;

		[SerializeField, Range(0f, 5f)]
		protected float m_AttackDelay = 0.2f;

		[SerializeField, Range(0f, 1f)]
		protected float m_AttackEffort = 0.05f;

		[Title("Object Detection")]

		[SerializeField]
		protected LayerMask m_HitMask = (LayerMask)172545;

		[SerializeField]
		protected QueryTriggerInteraction m_HitTriggers;

		[SerializeField]
		protected Vector3 m_HitOffset = Vector3.zero;
		
		[SerializeField, Range(0f, 1f)]
		protected float m_HitRadius = 0.1f;

		[SerializeField, Range(0f, 5f)]
		protected float m_HitDistance = 0.5f;

		[Title("Impact")]

		[SerializeField, Range(0f, 1000f)]
		protected float m_Damage = 15f;

		[SerializeField, Range(0f, 1000f)]
		protected float m_Force = 30f;

		[SerializeField, Range(0f, 100f)]
		protected float m_DurabilityRemove = 5f;

		[Title("Effects")]

		[SerializeField]
		protected EffectCollection m_SwingEffects;

		[SerializeField]
		protected EffectCollection m_HitEffects;

		protected RaycastHit m_HitInfo;

		[SerializeField] protected ThirdPersonPlayer m_TPSPlayer;
		[SerializeField] private TPSAttackAnimationType m_TPSAttackAnimationType;
		public override bool GetSwingValidity(float accuracy) => true;

		public override void DoSwing(float accuracy)
		{
			UpdateManager.InvokeDelayedAction(this, DoHit, m_AttackDelay);

			// Play Effects.
			m_SwingEffects.PlayEffects(Wieldable);

            // Mirror
            PlayEffectsCmd();

            if (m_TPSAttackAnimationType == TPSAttackAnimationType.Attack1)
				m_TPSPlayer?.PlayAttackAnimation1();
			else
                m_TPSPlayer?.PlayAttackAnimation2();
			// Mirror end

            // Remove Stamina.
            if (m_AttackEffort > 0.001f)
			{
				if (Wieldable.Character.TryGetModule(out IStaminaController stamina))
					stamina.Stamina -= m_AttackEffort;
			}

			void DoHit() => TryHit(GetUseRay(accuracy));
		}

		[ClientRpc]

		private void PlayEffectsRPC()
        {
            if (Wieldable.Character.gameObject.GetComponent<Character>().isLocalPlayer == false)
                m_SwingEffects.PlayEffects(Wieldable);

        }
        [Command]

        private void PlayEffectsCmd()
		{
				
			PlayEffectsRPC();

        }

		[Command]

		private void SpawnHitEffectsCmd(int surfaceId, Vector3 position, Quaternion rotation)
		{
			SpawnHitEffectsRPC(surfaceId, position,rotation);

        }

        [ClientRpc()]
		private void SpawnHitEffectsRPC(int surfaceId, Vector3 position, Quaternion rotation)
		{
			if (Wieldable.Character.gameObject.GetComponent<Character>().isLocalPlayer)
			{
				m_HitEffects.PlayEffects(Wieldable);
			}
			SurfaceManager.SpawnEffect(surfaceId, SurfaceEffects.Slash, 1, position, rotation);
        }

        public override void CancelSwing() => UpdateManager.StopAllDelayedActionsFor(this);

		protected virtual void OnHit(Ray ray)
		{
			bool isDynamicObject = false;

			// Apply an impact impulse
			if (m_HitInfo.rigidbody != null)
			{
				m_HitInfo.rigidbody.AddForceAtPosition(ray.direction * m_Force, m_HitInfo.point, ForceMode.Impulse);
				isDynamicObject = true;
			}
			Debug.Log(m_HitInfo.collider);

			IDamageReceiver receiver = m_HitInfo.collider.GetComponentInChildren<IDamageReceiver>();

			Debug.Log(receiver);

            if (receiver != null)
				receiver.HandleDamage(m_Damage, new DamageContext(m_DamageType, m_HitInfo.point, ray.direction * m_Force, m_HitInfo.normal, Wieldable.Character));

			// Surface effect
			SurfaceDefinition info = SurfaceManager.SpawnEffect(m_HitInfo, SurfaceEffects.Slash, 1f, isDynamicObject);

			m_HitEffects.PlayEffects(Wieldable);

            SpawnHitEffectsCmd(info.Id, m_HitInfo.point, Quaternion.LookRotation(m_HitInfo.normal));

            ConsumeDurability(m_DurabilityRemove);
		}

		protected Ray GetUseRay(float accuracy)
		{
			float spread = Mathf.Lerp(1f, 5f, 1 - accuracy);
			return PhysicsUtils.GenerateRay(Wieldable.Character.ViewTransform, spread, m_HitOffset);
		}

		private void TryHit(Ray ray)
		{
			if (PhysicsUtils.SphereCastNonAlloc(ray, m_HitRadius, m_HitDistance, out m_HitInfo, m_HitMask, Wieldable.Character.Colliders, m_HitTriggers))
				OnHit(ray);
		}
    }
}

public enum TPSAttackAnimationType
{
	Attack1,
	Attack2
}