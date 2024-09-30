#if XWIZARD_GAMES_STP_MP
using UnityEngine;
using Mirror;
using PolymindGames;
using PolymindGames.InventorySystem;

namespace XWizardGames.STP_MP {
    public class ThirdPersonClothesController : NetworkCharacterBehaviour
    {
        [SerializeField] private BodyClothing m_BodyClothing;

        [SyncVar(hook = nameof(OnHeadEquipmentChangedCallback))] private int m_HeadId;
        [SyncVar(hook = nameof(OnTorsoEquipmentChangedCallback))] private int m_TorsoId;
        [SyncVar(hook = nameof(OnLegsEquipmentChangedCallback))] private int m_LegsId;
        [SyncVar(hook = nameof(OnFeetEquipmentChangedCallback))] private int m_FeetId;

        protected override void OnBehaviourEnabled()
        {
            base.OnBehaviourEnabled();
            SetupClothingSlotListeners();
        }

        private void SetupClothingSlotListeners()
        {
            var inventory = Character.Inventory;

            var headContainer = inventory.GetContainerWithName("Head");
            var torsoContainer = inventory.GetContainerWithName("Torso");
            var legsContainer = inventory.GetContainerWithName("Legs");
            var feetContainer = inventory.GetContainerWithName("Feet");

            headContainer.SlotChanged += context => OnClothingSlotChanged(context.Slot, ClothingType.Head);
            torsoContainer.SlotChanged += context => OnClothingSlotChanged(context.Slot, ClothingType.Torso);
            legsContainer.SlotChanged += context => OnClothingSlotChanged(context.Slot, ClothingType.Legs);
            feetContainer.SlotChanged += context => OnClothingSlotChanged(context.Slot, ClothingType.Feet);
        }

        private void OnClothingSlotChanged(ItemSlot itemSlot, ClothingType type)
        {
            if (itemSlot == null || !itemSlot.HasItem) return;

            switch (type)
            {
                case ClothingType.Head:
                    m_HeadId = itemSlot.Item.Id;
                    break;
                case ClothingType.Torso:
                    m_TorsoId = itemSlot.Item.Id;
                    break;
                case ClothingType.Legs:
                    m_LegsId = itemSlot.Item.Id;
                    break;
                case ClothingType.Feet:
                    m_FeetId = itemSlot.Item.Id;
                    break;
            }
        }

        private void OnHeadEquipmentChangedCallback(int oldEquipment, int newEquipment)
        {
            UpdateClothingVisibility(newEquipment, ClothingType.Head);
        }

        private void OnTorsoEquipmentChangedCallback(int oldEquipment, int newEquipment)
        {
            UpdateClothingVisibility(newEquipment, ClothingType.Torso);
        }

        private void OnLegsEquipmentChangedCallback(int oldEquipment, int newEquipment)
        {
            UpdateClothingVisibility(newEquipment, ClothingType.Legs);
        }

        private void OnFeetEquipmentChangedCallback(int oldEquipment, int newEquipment)
        {
            UpdateClothingVisibility(newEquipment, ClothingType.Feet);
        }

        private void UpdateClothingVisibility(int equipmentId, ClothingType clothingType)
        {
            if (equipmentId == 0)
            {
                m_BodyClothing.HideClothing(clothingType);
            }
            else
            {
                m_BodyClothing.ShowClothing(equipmentId);
            }
        }
    }
}
#endif