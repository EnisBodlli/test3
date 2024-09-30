#if XWIZARD_GAMES_STP_MP
using Mirror;
using PolymindGames;
using PolymindGames.InventorySystem;
using PolymindGames.WieldableSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace XWizardGames.STP_MP
{

    public class ThirdPersonWeapon : NetworkBehaviour
    {
        public DataIdReference<ItemDefinition> ReferencedWieldableItem => m_ReferencedWieldableItem;
        public DataIdReference<CarriableDefinition> ReferencedCarriableItem => m_ReferencedCarriableItem;

        public string UpperBodyStateName { get => m_UpperBodyStateName; }

        [FormerlySerializedAs("m_CorrespondingItem")]
        [FormerlySerializedAs("m_DefaultItemDefinition")]

        [SerializeField, DataReferenceDetails(HasLabel = true, HasIcon = true, HasAssetReference = true)]
        private DataIdReference<ItemDefinition> m_ReferencedWieldableItem;


        [SerializeField, DataReferenceDetails(HasLabel = true, HasIcon = true, HasAssetReference = true)]
        private DataIdReference<CarriableDefinition> m_ReferencedCarriableItem;

        [SerializeField] private string m_UpperBodyStateName;
    }
}
#endif