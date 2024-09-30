#if XWIZARD_GAMES_STP_MP
using PolymindGames;
using PolymindGames.MovementSystem;
using PolymindGames.WieldableSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using PolymindGames.InputSystem.Behaviours;

namespace XWizardGames.STP_MP
{
    public class ThirdPersonPlayer : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnActiveWeaponChanged))] private string m_ActiveTPSWeaponName;

        [SerializeField] private FPSMovementInput m_FPSMovementInput;

        [SerializeField] private CharacterControllerMotor m_Player;
        [SerializeField] private PlayerMovementController m_PlayerMovementController;

        [SerializeField] private List<ThirdPersonWeapon> m_WeaponVisuals = new List<ThirdPersonWeapon>();
        [SerializeField] private WieldablesController m_WieldableController;
        [SerializeField] protected string m_CurrentUpperBodyState;
        [SerializeField] private List<SkinnedMeshRenderer> m_Renderers = new List<SkinnedMeshRenderer>();
        [SerializeField] private List<MeshRenderer> m_WieldablesRenderers = new List<MeshRenderer>();
        [SerializeField] private List<GameObject> m_ParticlesRenderers = new List<GameObject>();
        [SerializeField] private float m_AnimatorUpdateSpeedRun = 3;
        [SerializeField] private float m_AnimatorUpdateSpeedWalk = 1.5f;
        [SerializeField] private Transform m_Target;
        [SyncVar] private Vector3 m_TargetRotationNetworked;

        private Animator m_Animator;
        private NetworkAnimator m_NAnimator;




        private float animatorX = 0;
        private float animatorY = 0;


        private const string VERTICAL = "Vertical";
        private const string HORIZONTAL = "Horizontal";
        private const string JUMP_PARA = "Jump";
        private const string HOLSTER_WIELDABLE_PARA = "HolsterWieldable";

        private void Start()
        {
            SetupRenderers();

            if (!isLocalPlayer) return;

            InitializeLocalPlayer();
        }

        private void SetupRenderers()
        {
            foreach (SkinnedMeshRenderer r in m_Renderers)
            {
                r.enabled = !isLocalPlayer;

            }
            
            foreach (MeshRenderer r in m_WieldablesRenderers)
            {
                r.enabled = !isLocalPlayer;
            }
            foreach (GameObject r in m_ParticlesRenderers)
            {
                r.gameObject.SetActive(!isLocalPlayer);
            }
        }

        private void InitializeLocalPlayer()
        {
            m_Animator = GetComponent<Animator>();
            m_NAnimator = GetComponent<NetworkAnimator>();

            m_WieldableController.WieldableEquipStarted += EquipWeapon;
            m_WieldableController.WieldableHolsterStarted += HolsterWeapon;
        }

        private void Update()
        {
            if (!isLocalPlayer)
            {
                UpdateIkTargetProxies();
                return;

            }
            if (!m_FPSMovementInput) return;

            UpdateAnimator();
            UpdateIkTargetLocally();
        }

        private void UpdateIkTargetLocally()
        {
            m_TargetRotationNetworked = m_Target.localEulerAngles;
        }

        private void UpdateIkTargetProxies()
        {
            m_Target.localEulerAngles = m_TargetRotationNetworked;
        }

        public void PlaySlideAnimation()
        {
            m_NAnimator.SetTrigger("Slide");
        }
        private void UpdateAnimator()
        {
            UpdateMovement();

            m_Animator.SetBool("Crouch", m_PlayerMovementController.ActiveState == MovementStateType.Crouch ? true : false);

            void UpdateMovement()
            {
                float speed = m_Player.CCSpeed;

                if (m_FPSMovementInput.RawMovementInput.y == 0 && m_FPSMovementInput.RawMovementInput.x == 0)
                {
                    animatorX = Mathf.Lerp(animatorX, 0, Time.deltaTime * m_AnimatorUpdateSpeedRun);
                    animatorY = Mathf.Lerp(animatorY, 0, Time.deltaTime * m_AnimatorUpdateSpeedRun);
                }
                else if (m_FPSMovementInput.RunInput)
                {
                    animatorX = Mathf.Lerp(animatorX, m_FPSMovementInput.RawMovementInput.x, Time.deltaTime * m_AnimatorUpdateSpeedRun);
                    animatorY = Mathf.Lerp(animatorY, m_FPSMovementInput.RawMovementInput.y, Time.deltaTime * m_AnimatorUpdateSpeedRun);
                }
                else
                {
                    float halfX = m_FPSMovementInput.RawMovementInput.x / 2;
                    float halfY = m_FPSMovementInput.RawMovementInput.y / 2;

                    animatorX = Mathf.Lerp(animatorX, halfX, Time.deltaTime * m_AnimatorUpdateSpeedWalk);
                    animatorY = Mathf.Lerp(animatorY, halfY, Time.deltaTime * m_AnimatorUpdateSpeedWalk);
                }

                m_Animator.SetFloat(VERTICAL, animatorY);
                m_Animator.SetFloat(HORIZONTAL, animatorX);
            }
        }

        private void EquipWeapon(IWieldable wieldable)
        {
            m_NAnimator.ResetTrigger(HOLSTER_WIELDABLE_PARA);

            if (wieldable == null || wieldable.gameObject == null) return;

            WieldableItem firearm = wieldable.gameObject.GetComponent<WieldableItem>();
            Carriable carriable = wieldable.gameObject.GetComponent<Carriable>();

            ThirdPersonWeapon activeWeapon = null;

            foreach (ThirdPersonWeapon weapon in m_WeaponVisuals)
            {
                if (firearm != null && weapon.ReferencedWieldableItem == firearm.DefaultItemDefinition)
                {
                    activeWeapon = weapon;
                    m_ActiveTPSWeaponName = weapon.ReferencedWieldableItem.Name;
                    break;
                }
                else if (carriable != null && weapon.ReferencedCarriableItem == carriable.Definition)
                {
                    activeWeapon = weapon;
                    m_ActiveTPSWeaponName = weapon.ReferencedCarriableItem.Name;
                    break;
                }
            }

            //if (activeWeapon == null) m_ActiveTPSWeaponName = null;

            if (m_ActiveTPSWeaponName != null)
            {
                m_Animator.CrossFade(activeWeapon.UpperBodyStateName, 0.5f, 1);
                m_CurrentUpperBodyState = activeWeapon.UpperBodyStateName;
            }
        }

        private void HolsterWeapon(IWieldable wieldable)
        {
            m_NAnimator.ResetTrigger(HOLSTER_WIELDABLE_PARA);

            if (wieldable == null || wieldable.gameObject == null) return;

            WieldableItem firearm = wieldable.gameObject.GetComponent<WieldableItem>();
            Carriable carriable = wieldable.gameObject.GetComponent<Carriable>();

            foreach (ThirdPersonWeapon weapon in m_WeaponVisuals)
            {
                if (firearm != null && weapon.ReferencedWieldableItem == firearm.DefaultItemDefinition)
                {
                    m_NAnimator.SetTrigger(HOLSTER_WIELDABLE_PARA);
                    m_ActiveTPSWeaponName = null;
                    break;
                }
                else if (carriable != null && weapon.ReferencedCarriableItem == carriable.Definition)
                {
                    m_NAnimator.SetTrigger(HOLSTER_WIELDABLE_PARA);
                    m_ActiveTPSWeaponName = null;
                    break;
                }
            }
        }

        private void OnActiveWeaponChanged(string oldWeapon, string newWeapon)
        {
            Debug.Log("Weapon was changed " + newWeapon);
            UpdateWeaponVisuals(newWeapon);
        }

        private void UpdateWeaponVisuals(string newWeapon)
        {
            foreach (var weapon in m_WeaponVisuals)
            {
                bool isActiveWeapon = weapon.ReferencedCarriableItem == newWeapon || weapon.ReferencedWieldableItem == newWeapon;
                weapon.gameObject.SetActive(isActiveWeapon);
            }
        }
        public void PlayAttackAnimation1()
        {
            m_NAnimator.SetTrigger("Attack1");
        }
        public void PlayAttackAnimation2()
        {
            m_NAnimator.SetTrigger("Attack2");
        }
    }
}
#endif