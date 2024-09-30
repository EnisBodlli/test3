using PolymindGames.InputSystem.Behaviours;
using PolymindGames.UISystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using XWizardGames.STP_MP;

namespace PolymindGames
{
    public class Player : Character
    {
        public static Player LocalPlayer
        {
            get => s_LocalPlayer;
            private set
            {
                if (s_LocalPlayer == value)
                    return;
                
                s_LocalPlayer = value;
                LocalPlayerChanged?.Invoke(s_LocalPlayer);
            }
        }

        /// <summary>
        /// This message will be sent after the first initialized action.
        /// </summary>
        public event UnityAction AfterInitialized;
        
        /// <summary>
        ///  Player: Current Player
        /// </summary>
        public static event PlayerChangedDelegate LocalPlayerChanged;
        public delegate void PlayerChangedDelegate(Player player);

        private static Player s_LocalPlayer;

        [SerializeField] private List<Camera> m_Camera = new List<Camera>();
        [SerializeField] private GameObject m_PlayerVisuals;
        [SerializeField] private PlayerUIManager m_PlayerUIManagerPrefab;
        [SerializeField] private BuildablesControllerNetworked m_BuildablesControllerNetworked;

        public BuildablesControllerNetworked BuildablesControllerNetworked => m_BuildablesControllerNetworked;
        private PlayerUIManager m_RuntimeUIManager;

        private void SpawnLocalUI()
        {
            if (isLocalPlayer == false) return;

            m_RuntimeUIManager = Instantiate(m_PlayerUIManagerPrefab);
            m_RuntimeUIManager.AttachToPlayer(this);
        }
        protected void DisableCameraProxy()
        {
            foreach(Camera c in m_Camera)
            {
                c.enabled = isLocalPlayer;

                AudioListener audioListener = c.GetComponent<AudioListener>();
                if(audioListener != null )
                {
                    audioListener.enabled = isLocalPlayer;
                }
            }
        }

        protected void SetInputs(bool value)
        {
            InputBehaviour[] behaviours = GetComponentsInChildren<InputBehaviour>(true);
            foreach(InputBehaviour behaviour in behaviours) {
                behaviour.enabled = value;
                if(isLocalPlayer == false)
                {
                    //Destroy(behaviour);
                }
            }
        }
        [SerializeField] protected List<MonoBehaviour> m_ScriptsToDisableOnProxies = new List<MonoBehaviour>();
        protected void DisableMonoBehaviours()
        {
            foreach (MonoBehaviour behaviour in m_ScriptsToDisableOnProxies)
            {
                behaviour.enabled = isLocalPlayer;
            }
        }

        protected void DisableVisualsLocalPlayer()
        {
            m_PlayerVisuals.SetActive(!isLocalPlayer);
        }
        private void Awake()
        {
            SetInputs(false);
        }
        protected override void Start()
        {
            SetInputs(isLocalPlayer);
            DisableMonoBehaviours();
            DisableCameraProxy();
            DisableVisualsLocalPlayer();
            SpawnLocalUI();

            base.Start();

            if (!isLocalPlayer) return;

            if (LocalPlayer != null)
                Destroy(this);
            else
            {
                LocalPlayer = this;
            }
            //if (!isLocalPlayer) return;

            CursorLocker.ForceLockCursor();
            AfterInitialized?.Invoke();
        }

        private void OnDestroy()
        {
            if (!isLocalPlayer) return;

            if (LocalPlayer == this)
                LocalPlayer = null;
        }
    }
}