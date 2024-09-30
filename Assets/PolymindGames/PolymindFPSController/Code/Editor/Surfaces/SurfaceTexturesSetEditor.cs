using Toolbox.Editor;
using UnityEditor;

namespace PolymindGames.Surfaces
{
    [CustomEditor(typeof(SurfaceTexturesSet))]
    public class SurfaceTexturesSetEditor : ToolboxEditor
    {
        protected override bool DrawScriptProperty => m_DrawScriptProperty;

        private bool m_DrawScriptProperty = true;


        public void ShowSurfaceField(bool show)
        {
            if (!show)
            {
                IgnoreProperty("m_Surface");
                m_DrawScriptProperty = false;
            }
            else
            {
                RemoveIgnore("m_Surface");
                m_DrawScriptProperty = true;
            }
        }
    }
}
