using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;


namespace Mobcast.CoffeeEditor.Tool
{
	/// <summary>エディタービルトイン一覧ウィンドウ.</summary>
	partial class EditorBuiltin : EditorWindow
	{

		/// <summary>テクスチャサイズ.</summary>
		const float kTextureSizeMax = 64f;

		/// <summary>ラベルサイズ.</summary>
		const float kLabelSize = 200f;

		/// <summary>スタイル表示オプション.</summary>
		enum StyleOption : int
		{
			/// <summary>アクティブか.</summary>
			Active = 1 << 0,
			/// <summary>コンテンツを表示するか.</summary>
			Content = 1 << 1,
			/// <summary>リッチテキストを有効化するか.</summary>
			RitchText = 1 << 2,
			/// <summary>水平伸長を有効化するか.</summary>
			Expand = 1 << 3,
		}

		static Dictionary<Texture2D,string> editorIcons;
		static GUIStyle[] editorStyles;
		static Texture2D[] filteredIcons;
		static GUIStyle[] filteredStyles;

		static GUIContent[] categoryContents = new GUIContent[]{ new GUIContent("Styles"), new GUIContent("Icons") };
		static GUIContent[] styleOptionContents;

		static GUIContent tmpContent = new GUIContent();


		/// <summary>使い方ボタンコンテンツ.</summary>
		static GUIContent contentHowToUse;

		/// <summary>検索テキスト.</summary>
		static string searchText = "";

		/// <summary>現在の表示カテゴリ.</summary>
		int category = 0;

		/// <summary>スタイル表示オプション.</summary>
		int styleOption = System.Enum.GetValues(typeof(StyleOption)).Cast<int>().Aggregate((a, b) => a | b);

		Vector2 scrollPos;

		/// <summary>エディタウィンドウを生成.</summary>
		[MenuItem("Coffee/Tool/Editor Builtin")]
		static void OpenWindowByMenu()
		{
			GetWindow<EditorBuiltin>();
		}

		void OnEnable()
		{
			titleContent.text = "Editor Builtin";

			if (contentHowToUse == null)
			{
				contentHowToUse = new GUIContent(EditorGUIUtility.FindTexture("_help"), "How to use");

				styleOptionContents = System.Enum.GetNames(typeof(StyleOption)).Select(x => new GUIContent(x)).ToArray();

				//Inspectorで適用可能なGUIStyleをすべて取得.
				editorStyles = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).customStyles
					.Select(x => new GUIStyle(x))
					.ToArray();
				foreach (GUIStyle style in editorStyles) style.stretchHeight = false;

				//Editorで利用可能なアイコンをすべて取得します.
				//EditorAssetBundleに含まれているTextureのうち、iconsディレクトリ配下のものだけを抽出します.
				AssetBundle bundle = typeof(EditorGUIUtility)
					.GetMethod("GetEditorAssetBundle", BindingFlags.Static | BindingFlags.NonPublic)
					.Invoke(null, null) as AssetBundle;

				Regex regIcon = new Regex("icons/(generated/)?(.*)(\\.png|\\.asset)");
				string[] assetNames = bundle.GetAllAssetNames();
				
				//Texture2Dかつ名前にiconsが含まれるアセットのみ抽出.
				editorIcons = Enumerable.Range(0, assetNames.Length)
					.Select(i => new {asset = bundle.LoadAsset(assetNames[i]), name = assetNames[i]})
					.Where(x => x.asset is Texture2D && regIcon.IsMatch(x.name))
					.ToDictionary(x => x.asset as Texture2D, x => regIcon.Replace(x.name, "$2"));
			}

			//初期検索テキストは空文字とします.
			searchText = "";
			filteredIcons = editorIcons.Keys.ToArray();
			filteredStyles = editorStyles;
		}

		void OnGUI()
		{
			//ヘッダー表示.
			using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
			{
				//カテゴリ選択ツールバー.
				Toolbar(ref category, categoryContents, false);

				//検索テキストフィールド.
				DrawSearchTextField();

				//使い方ボタン.
				if (GUILayout.Button(contentHowToUse, EditorStyles.label, GUILayout.Width(20)))
				{
					UnityEngine.Debug.Log(
						"<b><color=green>## How to use 'Editor Built-in GUIStyles & Icons' ##</color></b>\n"
						+ "Styles => In editor, get a specific GUIStyle by name. (A string value implicitly conversions to GUIStyle.)\n"
						+ "Icons => In editor, get a specific Texture by 'EditorGUIUtility.FindTexture' method with name.\n"
					);
				}
			}

			//現在のカテゴリで表示物を切り替え(GUIStyle/Icon).
			if (category == 0)
				DrawStyles();
			else
				DrawIcons();
		}


		/// <summary>
		/// ツールバー.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <param name="content">Content.</param>
		/// <param name="maskable">If set to <c>true</c> maskable.</param>
		void Toolbar(ref int value, GUIContent[] content, bool maskable)
		{
			using (new EditorGUILayout.HorizontalScope())
			{
				for (int i = 0; i < content.Length; i++)
				{
					int maskValue = 1 << i;
					bool flag = maskable ? (0 != (value & maskValue)) : (value == i);
					if (GUILayout.Toggle(flag, content[i], EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)) != flag)
						value = maskable ? (flag ? (value & ~maskValue) : (value | maskValue)) : i;
				}
			}
		}

		/// <summary>
		/// 検索テキストフィールドを表示します.
		/// </summary>
		/// <param name="searchText">検索文字列.</param>
		/// <param name="onChanged">変更コールバック.</param>
		void DrawSearchTextField()
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUI.BeginChangeCheck();
			searchText = GUILayout.TextField(searchText, "ToolbarSeachTextField");

			if (GUILayout.Button(GUIContent.none, "ToolbarSeachCancelButton"))
			{
				searchText = "";
				EditorGUIUtility.keyboardControl = 0;
			}

			//変更があった場合、フィルタを実行.
			if (EditorGUI.EndChangeCheck())
			{
				string lower = searchText.ToLower();
				bool isEmpty = (searchText.Length == 0);

				filteredIcons = isEmpty
					? editorIcons.Keys.ToArray()
					: editorIcons.Where(y => y.Value.ToLower().Contains(lower)).Select(y => y.Key).ToArray();
				
				filteredStyles = isEmpty
					? editorStyles
					: editorStyles.Where(y => y.name.ToLower().Contains(lower)).ToArray();
			}
			
			EditorGUILayout.EndHorizontal();
		}


		/// <summary>
		/// フィルタ済みスタイルを表示します.
		/// </summary>
		void DrawStyles()
		{
			//スタイル表示オプション（マスクツールバー）.
			using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
			{
				Toolbar(ref styleOption, styleOptionContents, true);
			}

			scrollPos = GUILayout.BeginScrollView(scrollPos);
			if (Event.current.type != EventType.ScrollWheel)
			{
				bool isContent = 0 != (styleOption & (int)StyleOption.Content);
				bool isRitchText = 0 != (styleOption & (int)StyleOption.RitchText);
				bool isExpand = 0 != (styleOption & (int)StyleOption.Expand);
				bool isActive = 0 != (styleOption & (int)StyleOption.Active);

				foreach (GUIStyle style in filteredStyles)
				{
					//スタイル名とスタイルサンプルの表示.
					EditorGUILayout.BeginHorizontal();
					tmpContent.text = style.name;
					if (GUILayout.Button(tmpContent, EditorStyles.label, GUILayout.MaxWidth(kLabelSize)))
					{
						UnityEngine.Debug.Log(style.name);
						EditorGUIUtility.keyboardControl = 0;
					}

					tmpContent.text = isContent ? isRitchText ? "<b>Content</b>" : "Content" : "";
					GUILayout.Toggle(isActive, tmpContent, style, GUILayout.ExpandWidth(isExpand));
					EditorGUILayout.EndHorizontal();
				}
			}
			GUILayout.EndScrollView();
		}

		/// <summary>
		/// フィルタ済みアイコンを表示します.
		/// </summary>
		void DrawIcons()
		{
			scrollPos = GUILayout.BeginScrollView(scrollPos, true, true);
			if (Event.current.type != EventType.ScrollWheel)
			{
				//スクロール描画領域内のテクスチャのみを描画.
				float yPosition = 0;
				foreach (var texture in filteredIcons)
				{
					float displayHeight = Mathf.Clamp(texture.height, 18f, kTextureSizeMax);

					//スクロール範囲内なら描画.
					if (scrollPos.y - kTextureSizeMax < yPosition && yPosition < scrollPos.y + Screen.height)
					{
						EditorGUILayout.BeginHorizontal(GUILayout.MaxHeight(displayHeight));
						tmpContent.text = editorIcons[texture];
						if (GUILayout.Button(tmpContent, EditorStyles.label, GUILayout.MaxHeight(displayHeight), GUILayout.Width(kLabelSize)))
						{
							UnityEngine.Debug.Log(editorIcons[texture]);
							EditorGUIUtility.keyboardControl = 0;
						}

						GUILayout.Label(texture, GUILayout.MaxHeight(kTextureSizeMax));
						EditorGUILayout.EndHorizontal();
					}
					//スクロール範囲外ならスキップ.
					else
						GUILayout.Space(displayHeight);
					
					yPosition += displayHeight;
				}
			}
			GUILayout.EndScrollView();
		}
	}
}