using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Coffee.Development
{
    internal static class FindReferencesByGitGrep
    {
        private static readonly StringBuilder s_Result = new StringBuilder();

        [MenuItem("Assets/Find References With Git-Grep %&R", false, 1500)]
        private static void Run()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(Selection.activeObject, out var guid, out long localId);
            if (string.IsNullOrEmpty(guid))
            {
                Debug.LogWarning("Selected asset is not found in the database.");
                return;
            }

            var entries = new List<string>();
            var isMainAsset = AssetDatabase.IsMainAsset(Selection.activeObject);
            if (!isMainAsset)
            {
                Grep($"{{fileID: {localId}}}", path, entries);
            }

            GitGrep(isMainAsset ? $"guid: {guid}" : $"\\{{fileID: {localId}, guid: {guid}",
                "':!*.meta' Assets/ Packages/ ProjectSettings/", entries,
                () => { Log(path, guid, localId, entries); });
        }

        private static void Grep(string search, string path, List<string> entries)
        {
            try
            {
                foreach (var line in File.ReadLines(path, Encoding.UTF8))
                {
                    if (line.Contains(search))
                    {
                        entries.Add($"{path} (same file)");
                        break;
                    }
                }
            }
            catch (Exception)
            {
                ;
            }
        }

        private static void GitGrep(string search, string targets, List<string> entries, Action onExit)
        {
            var p = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    Arguments = $"grep -I --name-only --recurse-submodules '{search}' -- {targets}",
                    CreateNoWindow = true,
                    FileName = "git",
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    WorkingDirectory = $"{Application.dataPath}/..",
                    UseShellExecute = false
                },
                EnableRaisingEvents = true
            };

            p.Exited += (_, __) =>
            {
                if (p.ExitCode == 0)
                {
                    entries.AddRange(p.StandardOutput.ReadToEnd().Split('\n'));
                    entries.RemoveAll(string.IsNullOrEmpty);
                }

                EditorApplication.delayCall += onExit.Invoke;
            };

            p.Start();
        }

        private static void Log(string path, string guid, long fileId, List<string> entries)
        {
            s_Result.Length = 0;
            if (entries.Count == 0)
            {
                s_Result.Append("<b><color=green>No references</color></b> ");
            }
            else
            {
                s_Result.Append($"<b><color=orange>{entries.Count} references</color></b> ");
            }

            s_Result.AppendLine($"found for <b>{path}</b> (guid: {guid}, fileId: {fileId})");
            entries.ForEach(e => s_Result.AppendLine($"-> {e}"));
            Debug.Log(s_Result);
        }
    }
}
