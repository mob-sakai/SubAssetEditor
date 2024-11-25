using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Coffee.Development
{
    internal class EditorBuiltinWindow : EditorWindow
    {
        private static readonly Regex s_RegexIcon = new Regex("(icons/.*.asset|\\.png$)");
        private static readonly Regex s_RegexIgnoreIcon = new Regex("(@2x|@3x|@4x|@8x|/d_)");
        private static readonly GUILayoutOption[] s_Shrink = { GUILayout.ExpandWidth(false) };
        private static readonly GUILayoutOption[] s_Expand = { GUILayout.ExpandWidth(true) };
        private static readonly GUILayoutOption[] s_Width200 = { GUILayout.Width(200) };

        [SerializeField]
        private Category m_Category = Category.Styles;

        [SerializeField]
        private StyleOption m_StyleOption =
            StyleOption.Active | StyleOption.Content | StyleOption.RichText | StyleOption.Expand;

        private (GUIContent content, string name)[] _allIcons;
        private GUIStyle[] _allStyles;
        private (GUIContent content, string name)[] _filteredIcons;
        private GUIStyle[] _filteredStyles;
        private GUIStyle _scriptStyle;
        private Vector2 _scrollViewPosition;
        private string _searchText = "";

        private void OnGUI()
        {
            // Header
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                m_Category = (Category)Toolbar<Category>((int)m_Category);

                if (SearchTextField(ref _searchText))
                {
                    _filteredStyles = null;
                    _filteredIcons = null;
                }
            }

            EditorGUILayout.EndHorizontal();

            RefreshIfNeeded();

            if (m_Category == Category.Styles)
            {
                DrawStyles();
            }
            else if (m_Category == Category.Icons)
            {
                DrawIcons();
            }
            else if (m_Category == Category.Examples)
            {
                DrawExamples();
            }
        }

        [MenuItem("Development/Editor Builtin", false, 1500)]
        private static void OpenWindow()
        {
            GetWindow<EditorBuiltinWindow>("Editor Builtin");
        }

        private static bool SearchTextField(ref string text)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            text = GUILayout.TextField(text, "SearchTextField");

            if (GUILayout.Button(GUIContent.none, "SearchCancelButton"))
            {
                text = "";
                GUIUtility.keyboardControl = 0;
            }

            EditorGUILayout.EndHorizontal();
            return EditorGUI.EndChangeCheck();
        }

        private static int Toolbar<T>(int value) where T : Enum
        {
            var type = typeof(T);
            var contents = EditorGUIUtility.TrTempContent(Enum.GetNames(type));
            var flags = type.GetCustomAttribute(typeof(FlagsAttribute)) != null;
            EditorGUILayout.BeginHorizontal();

            for (var i = 0; i < contents.Length; i++)
            {
                var maskValue = 1 << i;
                var flag = flags ? 0 != (value & maskValue) : value == i;
                if (GUILayout.Toggle(flag, contents[i], EditorStyles.toolbarButton, s_Shrink) != flag)
                {
                    value = flags ? flag ? value & ~maskValue : value | maskValue : i;
                }
            }

            EditorGUILayout.EndHorizontal();
            return value;
        }

        private void RefreshIfNeeded()
        {
            // Get all styles in current editor skin.
            if (_allStyles == null)
            {
                _allStyles = GUI.skin.customStyles
                    .Select(x => new GUIStyle(x) { stretchHeight = false })
                    .ToArray();
            }

            // Filter styles by search text.
            if (_filteredStyles == null)
            {
                _filteredStyles = string.IsNullOrEmpty(_searchText)
                    ? _allStyles
                    : _allStyles
                        .Where(x => 0 <= x.name.IndexOf(_searchText, StringComparison.OrdinalIgnoreCase))
                        .ToArray();
            }

            // Get all icons available in EditorGUIUtility.IconContent().
            if (_allIcons == null)
            {
                var bundle = typeof(EditorGUIUtility)
                    .GetMethod("GetEditorAssetBundle", BindingFlags.Static | BindingFlags.NonPublic)
                    .Invoke(null, null) as AssetBundle;
                var assetNames = bundle.GetAllAssetNames();
                _allIcons = Enumerable.Range(0, assetNames.Length)
                    .Where(i => s_RegexIcon.IsMatch(assetNames[i]) && !s_RegexIgnoreIcon.IsMatch(assetNames[i]))
                    .Select(i => (content: EditorGUIUtility.IconContent(assetNames[i]), name: assetNames[i]))
                    .ToArray();
            }

            // Filter icons by search text.
            if (_filteredIcons == null)
            {
                _filteredIcons = string.IsNullOrEmpty(_searchText)
                    ? _allIcons
                    : _allIcons
                        .Where(x => 0 <= x.name.IndexOf(_searchText, StringComparison.OrdinalIgnoreCase))
                        .ToArray();
            }

            if (_scriptStyle == null)
            {
                _scriptStyle = new GUIStyle("ScriptText");
                _scriptStyle.stretchHeight = false;
                _scriptStyle.margin = new RectOffset(4, 4, 4, 4);
            }
        }

        private void DrawStyles()
        {
            // Header
            m_StyleOption = (StyleOption)Toolbar<StyleOption>((int)m_StyleOption);
            var active = (m_StyleOption & StyleOption.Active) != 0;
            var contentOn = (m_StyleOption & StyleOption.Content) != 0;
            var richText = (m_StyleOption & StyleOption.RichText) != 0;
            var expand = (m_StyleOption & StyleOption.Expand) != 0;

            // Scroll view
            _scrollViewPosition = GUILayout.BeginScrollView(_scrollViewPosition);
            EditorGUIUtility.labelWidth = 300;
            var y = 0f;
            foreach (var style in _filteredStyles)
            {
                // Display item in scroll view.
                var content = contentOn
                    ? richText
                        ? EditorGUIUtility.TrTempContent("<b>Content</b>")
                        : EditorGUIUtility.TrTempContent("Content")
                    : GUIContent.none;
                var displayHeight = Mathf.Max(20, style.CalcSize(content).y);
                if (_scrollViewPosition.y - 250 < y && y < _scrollViewPosition.y + Screen.height)
                {
                    var r = EditorGUILayout.GetControlRect(true, displayHeight, style, expand ? s_Expand : s_Shrink);
                    var rLabel = new Rect(r.x, r.y, 200, displayHeight);
                    var rStyle = new Rect(r.x + 200, r.y, r.width - 200, displayHeight);

                    //
                    if (GUI.Toggle(rStyle, active, content, style) != active
                        || GUI.Button(rLabel, EditorGUIUtility.TrTempContent(style.name), "label"))
                    {
                        Debug.Log(style.name);
                        GUIUtility.keyboardControl = 0;
                    }
                }
                // Skip invisible item.
                else
                {
                    GUILayout.Space(displayHeight);
                }

                y += displayHeight;
            }

            GUILayout.EndScrollView();
        }

        private void DrawIcons()
        {
            // Scroll view
            _scrollViewPosition = GUILayout.BeginScrollView(_scrollViewPosition);
            var y = 0f;
            foreach (var icon in _filteredIcons)
            {
                // Display item in scroll view.
                var displayHeight = Mathf.Clamp(icon.content.image.height, 20, 64);
                if (_scrollViewPosition.y - 64 < y && y < _scrollViewPosition.y + Screen.height)
                {
                    var r = GUILayoutUtility.GetRect(-1, displayHeight);
                    var rLabel = new Rect(r.x, r.y, 300, displayHeight);
                    var rIcon = new Rect(r.x + 300, r.y, displayHeight, displayHeight);

                    if (GUI.Button(rLabel, EditorGUIUtility.TrTempContent(icon.name), "label")
                        || GUI.Button(rIcon, icon.content, "label"))
                    {
                        Debug.Log(icon.name);
                        GUIUtility.keyboardControl = 0;
                    }
                }
                // Skip invisible item.
                else
                {
                    GUILayout.Space(displayHeight);
                }

                y += displayHeight;
            }

            GUILayout.EndScrollView();
        }

        private void DrawExamples()
        {
            GUILayout.Label("Style Usage", EditorStyles.boldLabel);
            EditorGUILayout.TextArea("GUILayout.Toggle(true, GUIContent.none, \"IN LockButton\");\n" +
                                     "// or" +
                                     "GUILayout.Toggle(true, GUIContent.none, \"in lockbutton\");", _scriptStyle);
            GUILayout.Toggle(true, GUIContent.none, "in lockbutton");

            EditorGUILayout.TextArea(
                "GUILayout.Button(\"Content\", \"sv_label_3\");",
                _scriptStyle);
            GUILayout.Button("Content", "sv_label_3", s_Width200);

            EditorGUILayout.TextArea(
                "EditorGUILayout.BeginHorizontal();\n" +
                "text = GUILayout.TextField(text, \"SearchTextField\");\n" +
                "if (GUILayout.Button(GUIContent.none, \"SearchCancelButton\"))\n" +
                "{\n" +
                "    text = \"\";\n" +
                "    GUIUtility.keyboardControl = 0;\n" +
                "}\n" +
                "EditorGUILayout.EndHorizontal();",
                _scriptStyle);
            var text = "text";
            EditorGUILayout.BeginHorizontal(s_Width200);
            text = GUILayout.TextField(text, "SearchTextField");
            if (GUILayout.Button(GUIContent.none, "SearchCancelButton"))
            {
                text = "";
                GUIUtility.keyboardControl = 0;
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(30);
            GUILayout.Label("Icon Usage", EditorStyles.boldLabel);
            var trash = EditorGUIUtility.IconContent("icons/treeeditor.trash.png");
            EditorGUILayout.TextArea(
                "GUIContent trash = EditorGUIUtility.IconContent(\"icons/treeeditor.trash.png\");\n" +
                "Texture2D trashIcon = EditorGUIUtility.FindTexture(\"treeeditor.trash\");\n" +
                "GUILayout.Button(trash);\n" +
                "GUILayout.Button(trash, \"IconButton\");",
                _scriptStyle);

            GUILayout.Button(trash, s_Width200);
            GUILayout.Button(trash, "IconButton");
        }

        private enum Category
        {
            Styles,
            Icons,
            Examples
        }

        [Flags]
        private enum StyleOption
        {
            Active = 1 << 0,
            Content = 1 << 1,
            RichText = 1 << 2,
            Expand = 1 << 3
        }
    }
}
