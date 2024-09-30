using System.Linq;
using Toolbox.Editor;
using UnityEditor;
using UnityEngine;

namespace PolymindGames.Surfaces
{
    [CustomEditor(typeof(SurfaceDefinition))]
    public class SurfaceDefinitionEditor : ToolboxEditor
    {
        protected override bool DrawScriptProperty => false;

        private SurfaceDefinition m_Surface;
        private Editor[] m_SurfaceTexturesSetEditors;


        protected override void DrawCustomInspector()
        {
            if (SurfaceManagerWindow.IsActive)
            {
                GUI.enabled = false;
                GUILayout.BeginHorizontal("Box");
                EditorGUILayout.ObjectField(m_Surface, typeof(SurfaceDefinition), m_Surface);
                GUILayout.EndHorizontal();
                GUI.enabled = true;
            }

            GUILayout.Space(6f);

            foreach (var set in m_SurfaceTexturesSetEditors)
                set.OnInspectorGUI();

            GUILayout.Space(6f);

            base.DrawCustomInspector();
        }

        private void OnEnable()
        {
            m_Surface = target as SurfaceDefinition;

            var sets = Resources.LoadAll<SurfaceTexturesSet>("")
                .Where(t => t.Surface == m_Surface).ToArray();

            m_SurfaceTexturesSetEditors = new Editor[sets.Length];

            for (int i = 0; i < sets.Length; i++)
            {
                m_SurfaceTexturesSetEditors[i] = CreateEditor(sets[i]);
                ((SurfaceTexturesSetEditor)m_SurfaceTexturesSetEditors[i]).ShowSurfaceField(false);
            }
        }
    }
}
