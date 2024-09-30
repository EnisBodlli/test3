using UnityEditor;
using UnityEngine;
using Toolbox.Editor;

namespace PolymindGames.WieldableSystem
{
    [CustomEditor(typeof(LightEffect))]
    public class LightEffectEditor : ToolboxEditor
    {
        protected override void DrawCustomInspector()
        {
            base.DrawCustomInspector();

            EditorGUILayout.Space();

            if(!Application.isPlaying)
                GUI.enabled = false;

            if(GUILayout.Button("Play (fadeIn = true)"))
                (target as LightEffect).Play(true);

            if(GUILayout.Button("Play (fadeIn = false)"))
                (target as LightEffect).Play(false);

            if(!Application.isPlaying)
                GUI.enabled = true;
        }
    }
}