using UnityEngine;
using UnityEngine.Events;

namespace PolymindGames
{
    /// <summary>
    /// TODO: Implement
    /// </summary>
    [HelpURL("https://polymindgames.gitbook.io/welcome-to-gitbook/qgUktTCVlUDA7CAODZfe/player/modules-and-behaviours/health#temperature-manager-module")]
    public sealed class TemperatureManager : MonoBehaviour, ITemperatureManager, ISaveableComponent
    {
        public float Temperature
        {
            get => m_Temperature;
            set
            {
                float clampedValue = Mathf.Clamp(value, 0f, 1f);

                if (value != m_Temperature && clampedValue != m_Temperature)
                {
                    m_Temperature = clampedValue;
                    TemperatureChanged?.Invoke(clampedValue);
                }
            }
        }

        public event UnityAction<float> TemperatureChanged;

#if UNITY_EDITOR
        [SerializeField, Disable, SpaceArea]
#endif
        private float m_Temperature;


        public void LoadMembers(object[] members)
        {
            m_Temperature = (float)members[0];
        }

        public object[] SaveMembers()
        {
            object[] members = new object[]
            {
                m_Temperature,
            };

            return members;
        }
    }
}