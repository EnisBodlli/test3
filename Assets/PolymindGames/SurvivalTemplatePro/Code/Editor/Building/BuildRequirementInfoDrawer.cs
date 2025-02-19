﻿using PolymindGames.BuildingSystem;
using UnityEditor;
using UnityEngine;

namespace PolymindGames
{
    [CustomPropertyDrawer(typeof(BuildRequirementInfo))]
    public class BuildRequirementInfoDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.IndentedRect(position);

            EditorGUI.MultiPropertyField(position, new GUIContent[] { new GUIContent(""), new GUIContent("") }, property.FindPropertyRelative("m_BuildMaterial"));
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUIUtility.singleLineHeight;
    }
}