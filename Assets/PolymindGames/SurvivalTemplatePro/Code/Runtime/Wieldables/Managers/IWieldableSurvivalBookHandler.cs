using PolymindGames.BuildingSystem;
using PolymindGames.WieldableSystem;
using UnityEngine;
using UnityEngine.Events;

namespace PolymindGames
{
    public interface IWieldableSurvivalBookHandler : ICharacterModule
    {
        bool InspectionActive { get; }
        IWieldable AttachedWieldable { get; }

        Transform LeftPages { get; }
        Transform RightPages { get; }

        event UnityAction InspectionStarted;
        event UnityAction InspectionEnded;

        bool TryStartInspection();

        /// <summary>
        /// Stops inspecting the book and selects a buildable.
        /// </summary>
        /// <param name="buildableDef"> Buildable to select, can be null. </param>
        /// <returns> True if succesfull</returns>
        bool TryStopInspection(BuildableDefinition buildableDef);
    }
}