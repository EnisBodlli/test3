using Toolbox.Editor;
using UnityEditor;

namespace PolymindGames
{
    [CustomEditor(typeof(DataDefinitionBase), true)]
    public class DataDefinitionBaseEditor : ToolboxEditor
    {
        protected override bool DrawScriptProperty => !ToolbarEditorWindowBase.HasActiveWindows;
    }
}
