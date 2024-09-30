using Toolbox.Editor;
using UnityEditor;
using UnityEngine;

namespace PolymindGames.InventorySystem
{
    [CustomEditor(typeof(ItemData), editorForChildClasses: true)]
    public class ItemDataEditor : ToolboxEditor
    {
        protected override bool DrawScriptProperty => false;
    }
}