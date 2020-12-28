using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

namespace Coffee.Editors
{
    internal class SubAssetEditor : EditorWindow
    {
        private static GUIContent _contentNoRef;
        private static GUIContent _contentAdd;
        private static GUIContent _contentDelete;
        private static GUIContent _contentImport;
        private static GUIContent _contentExport;
        private static bool _cached = false;
        const float ICON_SIZE = 20;

        private bool _isLocked;
        private bool _isRenaming;
        private bool _hasSelectionChanged;
        private Object _current;
        private Vector2 _scrollPosition;
        private List<Object> _subAssets = new List<Object>();
        private readonly List<Object> _referencingAssets = new List<Object>();

        private static void CacheGUI()
        {
            if (_cached)
                return;
            _cached = true;

            _contentNoRef = new GUIContent(EditorGUIUtility.FindTexture("console.warnicon.sml"), "Not referenced by main asset");
            _contentAdd = new GUIContent(EditorGUIUtility.FindTexture("toolbar plus"), "Add to sub assets");
            _contentDelete = new GUIContent(EditorGUIUtility.FindTexture("treeeditor.trash"), "Delete asset");
            _contentImport = new GUIContent("Drag & Drop object to add as sub-asset.", EditorGUIUtility.FindTexture("toolbar plus"));
            _contentExport = new GUIContent(EditorGUIUtility.FindTexture("saveactive"), "Export asset");
        }

        [MenuItem("Assets/Sub Asset Editor")]
        public static void OnOpenFromMenu()
        {
            EditorWindow.GetWindow<SubAssetEditor>("Sub Asset");
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged()
        {
            // On select new asset.
            var active = Selection.activeObject;
            if (!_isLocked && active && _current != active && !(active is SceneAsset) && AssetDatabase.IsMainAsset(active))
            {
                OnSelectionChanged(active);
            }
        }

        private void OnSelectionChanged(Object active)
        {
            _current = active;

            // Find sub-assets.
            var assetPath = AssetDatabase.GetAssetPath(active);
            _subAssets = AssetDatabase.LoadAllAssetsAtPath(assetPath)
                .Where(x => x != _current && 0 == (x.hideFlags & HideFlags.HideInHierarchy))
                .Distinct()
                .ToList();

            // Find referencing assets.
            _referencingAssets.Clear();
            foreach (var o in AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(active)))
            {
                var sp = new SerializedObject(o).GetIterator();
                sp.Next(true);

                // Search referencing asset in SerializedProperties.
                while (sp.Next(true))
                {
                    if (sp.propertyType != SerializedPropertyType.ObjectReference || !sp.objectReferenceValue) continue;

                    var asset = sp.objectReferenceValue;
                    if (active != asset && o != asset && 0 == (asset.hideFlags & HideFlags.HideInHierarchy) && !_referencingAssets.Contains(asset))
                    {
                        _referencingAssets.Add(asset);
                    }
                }
            }

            // Refresh GUI.
            Repaint();
        }

        private void DeleteSubAsset(Object asset)
        {
            Object.DestroyImmediate(asset, true);
            _hasSelectionChanged = true;
        }

        private void AddSubAsset(Object asset)
        {
            AddSubAsset(new Object[] {asset});
        }

        /// <summary>
        /// Replace the specified obj, oldAsset and newAsset.
        /// </summary>
        private void AddSubAsset(IEnumerable<Object> assets)
        {
            // accepted object
            assets = assets
                .Where(x => x != _current && !(x is SceneAsset) && AssetDatabase.Contains(x) && !_subAssets.Contains(x));

            // Replace targets
            var replaceTargets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(_current))
                .Select(x => new SerializedObject(x))
                .ToArray();

            //
            foreach (var asset in assets)
            {
                var newInstance = Object.Instantiate(asset);
                newInstance.name = asset.name;
                AssetDatabase.AddObjectToAsset(newInstance, _current);

                // Find referencing assets.
                foreach (var so in replaceTargets)
                {
                    var sp = so.GetIterator();

                    // Search referencing asset in SerializedProperties.
                    while (sp.Next(true))
                    {
                        // Replace to new object.
                        if (sp.propertyType == SerializedPropertyType.ObjectReference && sp.objectReferenceValue == asset)
                        {
                            sp.objectReferenceValue = newInstance;
                        }
                    }

                    sp.serializedObject.ApplyModifiedProperties();
                }
            }

            _hasSelectionChanged = true;
        }

        private void OnGUI()
        {
            CacheGUI();
            if (!_current) return;

            using (new EditorGUILayout.HorizontalScope())
            {
                var rLabel = EditorGUILayout.GetControlRect(GUILayout.Width(80));
                GUI.Toggle(rLabel, true, "<b>Main Asset</b>", "IN Foldout");

                var rLock = EditorGUILayout.GetControlRect(GUILayout.Width(20));
                rLock.y += 2;
                if (GUI.Toggle(rLock, _isLocked, GUIContent.none, "IN LockButton") != _isLocked)
                {
                    _isLocked = !_isLocked;
                }

                GUILayout.FlexibleSpace();
            }

            EditorGUI.indentLevel++;
            using (new EditorGUILayout.HorizontalScope())
            {
                var r = EditorGUILayout.GetControlRect(true);

                r.width -= 20;
                EditorGUI.ObjectField(r, _current, _current.GetType(), false);

                r.x += r.width;
                r.width = 20;
                r.height = 20;
                if (GUI.Button(r, _contentDelete, EditorStyles.label))
                {
                    DeleteSubAsset(_current);
                }
            }

            EditorGUI.indentLevel--;

            GUILayout.Space(10);
            using (new EditorGUILayout.HorizontalScope())
            {
                var rLabel = EditorGUILayout.GetControlRect(GUILayout.Width(80));
                GUI.Toggle(rLabel, true, "<b>Sub Asset</b>", "IN Foldout");

                var rRename = EditorGUILayout.GetControlRect(GUILayout.Width(60));
                _isRenaming = GUI.Toggle(rRename, _isRenaming, "Rename", EditorStyles.miniButton);
                GUILayout.FlexibleSpace();
            }

            EditorGUI.indentLevel++;
            foreach (var asset in _subAssets)
            {
                var r = EditorGUILayout.GetControlRect(true);

                r.width -= 60;
                var rField = new Rect(r);
                if (_isRenaming)
                {
                    rField.width = 12;
                    rField.height = 12;
                    //Draw icon of current object.
                    EditorGUI.LabelField(r, new GUIContent(AssetPreview.GetMiniThumbnail(asset)));
                    EditorGUI.BeginChangeCheck();

                    rField.x += rField.width + 4;
                    rField.width = r.width - rField.width;
                    rField.height = r.height;
                    asset.name = EditorGUI.DelayedTextField(rField, asset.name);
                    if (EditorGUI.EndChangeCheck())
                    {
                        AssetDatabase.SaveAssets();
                    }
                }
                else
                {
                    EditorGUI.ObjectField(rField, asset, asset.GetType(), false);
                }

                r.x += r.width;
                r.width = 20;
                r.height = 20;
                if (!_referencingAssets.Contains(asset))
                {
                    GUI.Label(r, _contentNoRef);
                }

                r.x += r.width;
                if (GetFileExtension(asset).Length != 0 && GUI.Button(r, _contentExport, EditorStyles.label))
                {
                    ExportSubAsset(asset);
                }

                r.x += r.width;
                if (GUI.Button(r, _contentDelete, EditorStyles.label))
                {
                    DeleteSubAsset(asset);
                }
            }

            EditorGUI.indentLevel--;

            GUILayout.Space(10);
            GUILayout.Toggle(true, "<b>Referencing Objects</b>", "IN Foldout");
            EditorGUI.indentLevel++;
            EditorGUILayout.HelpBox("Sub assets are excluded.", MessageType.None);
            foreach (var asset in _referencingAssets.Except(_subAssets))
            {
                var r = EditorGUILayout.GetControlRect();

                r.width -= 20;
                EditorGUI.ObjectField(r, asset, asset.GetType(), false);

                r.x += r.width;
                r.y -= 1;
                r.width = 20;
                r.height = 20;

                // Add object to sub asset.
                if (GUI.Button(r, _contentAdd, EditorStyles.label))
                {
                    var addAsset = asset;
                    EditorApplication.delayCall += () => AddSubAsset(addAsset);
                }
            }

            EditorGUI.indentLevel--;

            DrawImportArea();

            if (!_hasSelectionChanged) return;

            _hasSelectionChanged = false;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            OnSelectionChanged(_current);
        }

        /// <summary>
        /// Draws an import area where assets can be added by drag & drop.
        /// </summary>
        private void DrawImportArea()
        {
            GUILayout.Space(5);
            var dropArea = GUILayoutUtility.GetRect(0, 20, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUI.Box(dropArea, _contentImport, EditorStyles.helpBox);

            var id = GUIUtility.GetControlID(FocusType.Passive);
            var evt = Event.current;
            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition))
                        break;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    DragAndDrop.activeControlID = id;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        DragAndDrop.activeControlID = 0;

                        AddSubAsset(DragAndDrop.objectReferences);
                    }

                    Event.current.Use();
                    break;
            }
        }

        /// <summary>
        /// Export sub-asset.
        /// </summary>
        private void ExportSubAsset(Object obj)
        {
            var exportDir = Path.GetDirectoryName(AssetDatabase.GetAssetPath(_current));
            var exportName = obj.name + " (Exported)." + GetFileExtension(obj);
            var uniquePath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(exportDir, exportName));
            AssetDatabase.CreateAsset(Object.Instantiate(obj), uniquePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static string GetFileExtension(Object obj)
        {
            if (obj is AnimationClip)
                return "anim";
            else if (obj is UnityEditor.Animations.AnimatorController)
                return "controller";
            else if (obj is AnimatorOverrideController)
                return "overrideController";
            else if (obj is Material)
                return "mat";
            else if (obj is Texture)
                return "png";
            else if (obj is ComputeShader)
                return "compute";
            else if (obj is Shader)
                return "shader";
            else if (obj is Cubemap)
                return "cubemap";
            else if (obj is Flare)
                return "flare";
            else if (obj is ShaderVariantCollection)
                return "shadervariants";
            else if (obj is LightmapParameters)
                return "giparams";
            else if (obj is GUISkin)
                return "guiskin";
            else if (obj is PhysicMaterial)
                return "physicMaterial";
            else if (obj is UnityEngine.Audio.AudioMixer)
                return "mixer";
            else if (obj is TextAsset)
                return "txt";
            else if (obj is GameObject)
                return "prefab";
            else if (obj is ScriptableObject)
                return "asset";
            return "";
        }
    }
}
