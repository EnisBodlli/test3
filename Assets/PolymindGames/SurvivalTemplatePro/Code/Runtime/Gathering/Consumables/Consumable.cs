using Mirror;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace PolymindGames
{
    [HelpURL("https://polymindgames.gitbook.io/welcome-to-gitbook/qgUktTCVlUDA7CAODZfe/interaction/interactable/demo-interactables")]
    public class Consumable : Interactable
    {
        public event UnityAction<Consumable> Consumed;

        [Title("Settings (Consumable)")]

        [SerializeField, Range(-100f, 100f)]
        [Tooltip("The minimum amount of hunger this consumable can restore.")]
        private int m_HungerRestoreMin = 15;

        [SerializeField, Range(-100f, 100f)]
        [Tooltip("The maximum amount of hunger this consumable can restore.")]
        private int m_HungerRestoreMax = 20;

        [SerializeField, Range(-100f, 100f)]
        [Tooltip("The minimum amount of thirst this consumable can restore.")]
        private int m_ThirstRestoreMin = 5;

        [SerializeField, Range(-100f, 100f)]
        [Tooltip("The maximum amount of thirst this consumable can restore.")]
        private int m_ThirstRestoreMax = 10;

        [SpaceArea]

        [SerializeField]
        [Tooltip("Audio that will be played after a character consumes this.")]
        private SoundPlayer m_ConsumeAudio;


        [SyncVar(hook = nameof(OnIsActiveChanged))] 
        public bool IsActive = false;

        [SerializeField] private Transform m_Mesh;
        [SerializeField] private SphereCollider m_SphereCollider;

        public override void OnStartServer()
        {
            base.OnStartServer();

            IsActive = true;
        }

        private void OnIsActiveChanged(bool oldValue, bool newValue)
        {
            m_Mesh.gameObject.SetActive(IsActive);
            m_SphereCollider.enabled = IsActive;
        }

        public override void OnInteract(Character character)
        {
            if (IsActive == false) return;

            base.OnInteract(character);

            bool consumed = false;

            if (character.TryGetModule(out IHungerManager hungerManager))
            {              
                hungerManager.Hunger += Random.Range(m_HungerRestoreMin, m_HungerRestoreMax);
                consumed = true;
            }

            if (character.TryGetModule(out IThirstManager thirstManager))
            {
                thirstManager.Thirst += Random.Range(m_ThirstRestoreMin, m_ThirstRestoreMax);
                consumed = true;
            }

            if (consumed)
            {
                m_ConsumeAudio.Play2D();
                ConsumedCmd();
                //gameObject.SetActive(false);
                //Consumed?.Invoke(this);
            }
        }

        [Command(requiresAuthority = false)]
        private void ConsumedCmd()
        {
            Consumed?.Invoke(this);
            IsActive = false;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            Description = $"Hunger: +{m_HungerRestoreMin}-{m_HungerRestoreMax}" + "\n" + $"Thirst: +{m_ThirstRestoreMin}-{m_ThirstRestoreMax}";
        }
#endif
    }
}
