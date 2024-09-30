using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

namespace PolymindGames
{
    public static class DataDefinitionUtility
    {
        /// <returns> The icons of all of the definitions.</returns>
        public static Sprite[] GetAllIcons<T>() where T : DataDefinition<T>
        {
            var definitions = DataDefinition<T>.Definitions;
            Sprite[] icons = new Sprite[definitions.Length];

            for (int i = 0; i < icons.Length; i++)
                icons[i] = definitions[i].Icon;

            return icons;
        }

        /// <returns> The names of all of the definitions.</returns>
        public static string[] GetAllNames<T>() where T : DataDefinition<T>
        {
            var definitions = DataDefinition<T>.Definitions;
            string[] names = new string[definitions.Length];

            for (int i = 0; i < names.Length; i++)
                names[i] = definitions[i].FullName;

            return names;
        }

        /// <returns> The descriptions of all of the definitions.</returns>
        public static string[] GetAllDescriptions<T>() where T : DataDefinition<T>
        {
            var definitions = DataDefinition<T>.Definitions;
            string[] descriptions = new string[definitions.Length];

            for (int i = 0; i < descriptions.Length; i++)
                descriptions[i] = definitions[i].Description;

            return descriptions;
        }

        /// <returns> All the ids of all of the definitions.</returns>
        public static List<int> GetAllIds<T>() where T : DataDefinition<T>
        {
            var definitions = DataDefinition<T>.Definitions;
            var ids = new List<int>();

            for (int i = 0; i < definitions.Length; i++)
                ids.Add(definitions[i].Id);

            return ids;
        }

        /// <returns> Index of given definition in the internal array.</returns>
        public static int GetIndexOfId<T>(int id) where T : DataDefinition<T>
        {
            var definitions = DataDefinition<T>.Definitions;

            for (int i = 0; i < definitions.Length; i++)
            {
                if (id == definitions[i].Id)
                    return i;
            }

            return -1;
        }

        /// <returns> The definition id at the given index.</returns>
        public static int GetIdAtIndex<T>(int index) where T : DataDefinition<T>
        {
            var definitions = DataDefinition<T>.Definitions;
            index = Mathf.Max(0, index);

            if (definitions.Length > 0)
                return definitions[index].Id;

            return -1;
        }

        /// <returns> Index of given definition in the given array.</returns>
        public static int GetIndexOfDefinition(this DataDefinitionBase[] defs, DataDefinitionBase def)
        {
            for (int i = 0; i < defs.Length; i++)
            {
                if (def == defs[i])
                    return i;
            }

            return -1;
        }

#if UNITY_EDITOR
        public static void ResetAllAssetDefinitionNames()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            foreach (var dataDef in Resources.LoadAll<DataDefinitionBase>(""))
                ResetDefinitionAssetName(dataDef);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <returns> The GUI contents (Name, Description and icon) of all of the definitions.</returns>
        public static GUIContent[] GetAllGUIContents<T>(bool name, bool tooltip, bool icon, GUIContent including = null) where T : DataDefinition<T>
        {
            bool hasExtraElement = including != null;

            var definitions = DataDefinition<T>.Definitions;
            GUIContent[] contents = new GUIContent[definitions.Length + (hasExtraElement ? 1 : 0)];

            if (hasExtraElement)
            {
                contents[0] = including;
                for (int i = 1; i < contents.Length; i++)
                {
                    var definition = definitions[i - 1];
                    contents[i] = new GUIContent()
                    {
                        text = name ? definition.FullName : string.Empty,
                        tooltip = tooltip ? definition.Description : string.Empty,
                        image = definition.Icon != null && icon ? AssetPreview.GetAssetPreview(definition.Icon) : null
                    };
                }
            }
            else
            {
                for (int i = 0; i < contents.Length; i++)
                {
                    var definition = definitions[i];
                    contents[i] = new GUIContent()
                    {
                        text = name ? definition.FullName : string.Empty,
                        tooltip = tooltip ? definition.Description : string.Empty,
                        image = definition.Icon != null && icon ? AssetPreview.GetAssetPreview(definition.Icon) : null
                    };
                }
            }

            return contents;
        }

        /// <summary>
        /// Resets the asset name to the Name of this definition + a prefix.
        /// </summary>
        public static void ResetDefinitionAssetName(DataDefinitionBase dataDef)
        {
            if (dataDef == null)
            {
                Debug.LogError("The passed definition cannot be null");
                return;
            }

            var assetPath = AssetDatabase.GetAssetPath(dataDef);

            if (assetPath != null && !string.IsNullOrEmpty(assetPath))
            {
                string prefix = GetAssetNamePrefix(dataDef.GetType());
                AssetDatabase.RenameAsset(assetPath, $"({prefix}) " + dataDef.Name);
                AssetDatabase.Refresh();
            }
        }

        public static string GetDefaultDefinitionName(DataDefinitionBase dataDef)
        {
            if (dataDef == null)
            {
                Debug.LogError("The passed definition cannot be null");
                return string.Empty;
            }

            string assetPath = AssetDatabase.GetAssetPath(dataDef);

            if (string.IsNullOrEmpty(assetPath))
                return dataDef.Name;

            int nameIndex = assetPath.LastIndexOf("/");

            assetPath = assetPath.Remove(0, nameIndex + 1);
            assetPath = assetPath.Remove(assetPath.IndexOf("."));

            if (assetPath.Contains("("))
            {
                int firstIndex = assetPath.IndexOf('(');
                int lastIndex = assetPath.IndexOf(')');

                if (assetPath.Length > lastIndex + 1)
                {
                    if (assetPath.Contains(" "))
                    {
                        assetPath = assetPath.Replace(" ", "");
                        lastIndex++;
                    }
                }

                assetPath = assetPath.Remove(firstIndex, lastIndex - firstIndex);
            }

            return assetPath.ToUnityLikeNameFormat();
        }

        public static string GetAssetNamePrefix(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            string name = type.Name;
            return name.Replace("Definition", "");
        }

        public static string GetAssetCreationPath<T>() where T : DataDefinition<T>
        {
            var definitions = DataDefinition<T>.Definitions;
            if (definitions.Length > 0)
            {
                string path = AssetDatabase.GetAssetPath(definitions[0]);
                int idx = path.LastIndexOf('/');
                return path.Remove(idx);
            }

            var resourceFolder = GetFirstFolder("Resources");
            if (resourceFolder != null)
            {
                var targets = @"\" + typeof(T).Name.Replace("Definition", "") + "s";

                if (!AssetDatabase.IsValidFolder(resourceFolder + targets))
                {
                    Debug.Log("CREATE");
                    AssetDatabase.CreateFolder(resourceFolder, typeof(T).Name.Replace("Definition", ""));
                    AssetDatabase.Refresh();
                }

                Debug.Log(resourceFolder + targets);

                return resourceFolder + targets;
            }

            var target = "Assets/PolymindGamesTEMP/Resources/" + typeof(T).Name.Replace("Definition", "");
            if (!AssetDatabase.IsValidFolder(target))
                AssetDatabase.CreateFolder("Assets/PolymindGamesTEMP/Resources/", typeof(T).Name.Replace("Definition", ""));
            return target;
        }

        public static List<string> GetFolders(string searchString)
        {
            var folders = new List<string>();
            ScanFolders(Application.dataPath, searchString, folders);
            return folders;
        }

        public static string GetFirstFolder(string searchString)
        {
            string[] folders = Directory.GetDirectories(Application.dataPath, "*" + searchString + "*", SearchOption.AllDirectories);
            return folders.Length > 0 ? folders[0] : null;
        }

        private static void ScanFolders(string path, string searchString, List<string> folders)
        {
            try
            {
                string[] directoryEntries = Directory.GetDirectories(path);

                foreach (string directory in directoryEntries)
                {
                    if (directory.Contains(searchString))
                    {
                        folders.Add(directory);
                    }

                    ScanFolders(directory, searchString, folders);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Error while scanning folders: " + e.Message);
            }
        }
#endif
    }
}
