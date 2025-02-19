using UnityEngine;

namespace PolymindGames.UISystem
{
    [AddComponentMenu("PolymindGames/UserInterface/Panels/Audio Panel")]
    public class AudioPanelUI : PanelUI
    {
        [Title("Audio")]

        [SerializeField]
        private SoundPlayerWithCooldown m_ShowAudio;

        [SerializeField]
        private SoundPlayerWithCooldown m_HideAudio;


        protected override void ShowPanel() => m_ShowAudio.Play2D();
        protected override void HidePanel() => m_HideAudio.Play2D();

        protected override void Start()
        {
            m_ShowAudio.SetCooldown(1f);
            m_HideAudio.SetCooldown(1f);

            base.Start();
        }
    }
}
