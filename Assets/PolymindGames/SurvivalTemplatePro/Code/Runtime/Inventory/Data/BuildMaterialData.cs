using PolymindGames.BuildingSystem;
using UnityEngine;

namespace PolymindGames.InventorySystem
{
    public class BuildMaterialData : ItemData
    {
        public DataIdReference<BuildMaterialDefinition> BuildMaterial => m_BuildMaterial;
        public int MaterialCount => m_MaterialCount;

        [SerializeField]
        private DataIdReference<BuildMaterialDefinition> m_BuildMaterial;

        [SerializeField, Range(1, 100)]
        private int m_MaterialCount = 1;
    }
}
