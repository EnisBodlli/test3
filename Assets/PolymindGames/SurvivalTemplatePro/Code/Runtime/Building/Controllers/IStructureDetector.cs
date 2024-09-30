using PolymindGames.BuildingSystem;
using UnityEngine.Events;

namespace PolymindGames
{
    public interface IStructureDetector : ICharacterModule
    {
        BuildablePreview StructureInView { get; }
        float CancelPreviewProgress { get; }
        bool DetectionEnabled { get; set; }

        event UnityAction<BuildablePreview> StructureChanged;
        event UnityAction<float> CancelPreviewProgressChanged;


        void StartCancellingPreview();
        void StopCancellingPreview();
    }
}