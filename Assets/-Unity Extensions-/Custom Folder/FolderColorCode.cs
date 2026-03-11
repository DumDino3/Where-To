using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace FolderIcons
{
    // ─────────────────────────────────────────
    // DATA CLASS — holds icon name + image path
    // ─────────────────────────────────────────
    [System.Serializable]
    public class IconEntry
    {
        public string Name;
        public string ImagePath;

        public IconEntry(string name, string imagePath)
        {
            Name = name;
            ImagePath = imagePath;
        }

        public Texture2D LoadTexture() =>
            AssetDatabase.LoadAssetAtPath<Texture2D>(ImagePath);
    }

    // ─────────────────────────────────────────
    // DATABASE — your list of available icons
    // ─────────────────────────────────────────
    public static class IconDatabase
    {
        public static readonly List<IconEntry> Icons = new()
        {
            new IconEntry("Scripts",   "Assets/-Unity Extensions-/Custom Folder/Icons/1.png"),
            new IconEntry("Prefabs",   "Assets/-Unity Extensions-/Custom Folder/Icons/2.png"),
            new IconEntry("Scenes",    "Assets/-Unity Extensions-/Custom Folder/Icons/3.png"),
            new IconEntry("Audio",     "Assets/-Unity Extensions-/Custom Folder/Icons/4.pngg"),
        };
    }

    // ─────────────────────────────────────────
    // MAP — stores folder path → icon name
    // ─────────────────────────────────────────
    public static class FolderIconMap
    {
        private static Dictionary<string, string> _map = new();
        private const string PrefKey = "FolderIconMap";

        static FolderIconMap() => Load();

        public static void Set(string folderPath, string iconName)
        {
            _map[folderPath] = iconName;
            Save();
        }

        public static string Get(string folderPath) =>
            _map.TryGetValue(folderPath, out var name) ? name : null;

        public static void Clear(string folderPath)
        {
            _map.Remove(folderPath);
            Save();
        }

        private static void Save()
        {
            var sb = new StringBuilder();
            foreach (var kvp in _map)
                sb.Append($"{kvp.Key}|{kvp.Value},");
            EditorPrefs.SetString(PrefKey, sb.ToString());
        }

        private static void Load()
        {
            _map = new();
            var raw = EditorPrefs.GetString(PrefKey, "");
            if (string.IsNullOrEmpty(raw)) return;

            foreach (var entry in raw.Split(','))
            {
                var parts = entry.Split('|');
                if (parts.Length == 2)
                    _map[parts[0]] = parts[1];
            }
        }
    }

    // ─────────────────────────────────────────
    // RIGHT-CLICK MENU
    // ─────────────────────────────────────────
    public static class FolderIconMenu
    {
        [MenuItem("Assets/Folder Icon/Set Icon", false, 1000)]
        private static void SetIcon()
        {
            var path = GetSelectedFolderPath();
            if (path == null) return;
            IconPickerWindow.Show(path);
        }

        [MenuItem("Assets/Folder Icon/Clear Icon", false, 1001)]
        private static void ClearIcon()
        {
            var path = GetSelectedFolderPath();
            if (path == null) return;
            FolderIconMap.Clear(path);
            EditorApplication.RepaintProjectWindow();
        }

        [MenuItem("Assets/Folder Icon/Set Icon", true)]
        [MenuItem("Assets/Folder Icon/Clear Icon", true)]
        private static bool ValidateFolder() =>
            GetSelectedFolderPath() != null;

        private static string GetSelectedFolderPath()
        {
            if (Selection.activeObject == null) return null;
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            return Directory.Exists(path) ? path : null;
        }
    }

    // ─────────────────────────────────────────
    // ICON PICKER POPUP WINDOW
    // ──────────────────────────────���──────────
    public class IconPickerWindow : EditorWindow
    {
        private string _folderPath;
        private Vector2 _scroll;

        public static void Show(string folderPath)
        {
            var window = GetWindow<IconPickerWindow>("Pick Folder Icon");
            window._folderPath = folderPath;
            window.minSize = new Vector2(200, 300);
            window.ShowUtility();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField($"Folder: {_folderPath}", EditorStyles.miniLabel);
            EditorGUILayout.Space();

            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            foreach (var entry in IconDatabase.Icons)
            {
                EditorGUILayout.BeginHorizontal();

                var tex = entry.LoadTexture();
                if (tex != null)
                    GUILayout.Label(tex, GUILayout.Width(32), GUILayout.Height(32));
                else
                    GUILayout.Label("?", GUILayout.Width(32), GUILayout.Height(32));

                if (GUILayout.Button(entry.Name, GUILayout.Height(32)))
                {
                    FolderIconMap.Set(_folderPath, entry.Name);
                    EditorApplication.RepaintProjectWindow();
                    Close();
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }
    }

    // ─────────────────────────────────────────
    // DRAWER — hooks into project window
    // ─────────────────────────────────────────
    [InitializeOnLoad]
    public class FolderIconDrawer
    {
        private static Dictionary<string, Texture2D> _cache = new();

        static FolderIconDrawer() =>
            EditorApplication.projectWindowItemOnGUI += OnGUI;

        static void OnGUI(string guid, Rect rect)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (!Directory.Exists(path)) return;
            if (Event.current.type != EventType.Repaint) return;

            var iconName = FolderIconMap.Get(path);
            if (iconName == null) return;

            var texture = GetCachedTexture(iconName);
            if (texture == null) return;

            Rect iconRect = rect.height > 20
                ? new Rect(rect.x, rect.y, rect.width, rect.width)    // grid view
                : new Rect(rect.x, rect.y, rect.height, rect.height); // list view

            GUI.DrawTexture(iconRect, texture);
        }

        private static Texture2D GetCachedTexture(string iconName)
        {
            if (_cache.TryGetValue(iconName, out var cached))
                return cached;

            var entry = IconDatabase.Icons.Find(e => e.Name == iconName);
            if (entry == null) return null;

            var tex = entry.LoadTexture();
            if (tex != null) _cache[iconName] = tex;
            return tex;
        }
    }
}