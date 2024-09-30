using UnityEngine;

namespace PolymindGames.Surfaces
{
    [CreateAssetMenu(menuName = "Polymind Games/Surfaces/Surface Textures Set", fileName = "(SurfaceTexturesSet) ")]
    public class SurfaceTexturesSet : ScriptableObject
    {
        public SurfaceDefinition Surface
        {
            get
            {
                if (m_Surface.IsNull)
                {
                    Debug.LogError($"The surface field on ''{name}'' cannot be null, assign it or delete this set.");
                    return null;
                }

                return m_Surface.Def;
            }
        }

        public Texture[] Textures => m_Textures;

        [SerializeField]
        private DataNameReference<SurfaceDefinition> m_Surface;

        [SpaceArea]

        [SerializeField, ReorderableList(HasLabels = false)]
        private Texture[] m_Textures;
    }
}
