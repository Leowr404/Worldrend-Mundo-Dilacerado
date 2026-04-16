// Assets/Editor/FindPackageDependencies.cs
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public class FindPackageDependencies : EditorWindow
{
    Vector2 scroll;
    bool scanAll = true;
    Dictionary<string, List<string>> results = new Dictionary<string, List<string>>();

    [MenuItem("Tools/Find Package Dependencies")]
    static void OpenWindow()
    {
        GetWindow<FindPackageDependencies>("Find Package Dependencies");
    }

    void OnGUI()
    {
        GUILayout.Label("Busca por dependências em 'Packages/'", EditorStyles.boldLabel);
        scanAll = EditorGUILayout.ToggleLeft("Scanar TODOS os assets (pasta Assets/) — pode demorar", scanAll);
        if (GUILayout.Button("Executar scan"))
        {
            Scan();
        }

        if (results == null || results.Count == 0)
        {
            EditorGUILayout.HelpBox("Nenhuma dependência encontrada (ainda) ou não executou o scan.", MessageType.Info);
            return;
        }

        EditorGUILayout.LabelField($"Assets com dependências em Packages/: {results.Count}");
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Logar no Console"))
            LogResults();
        if (GUILayout.Button("Exportar CSV (raiz do projeto)"))
            ExportCSV();
        EditorGUILayout.EndHorizontal();

        scroll = EditorGUILayout.BeginScrollView(scroll);
        foreach (var kvp in results.OrderBy(k => k.Key))
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(kvp.Key);
            foreach (var pkg in kvp.Value.Distinct())
                EditorGUILayout.LabelField("  • " + pkg);
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndScrollView();
    }

    void Scan()
    {
        results.Clear();
        string[] assetPaths;
        if (scanAll)
        {
            assetPaths = AssetDatabase.GetAllAssetPaths().Where(p => p.StartsWith("Assets/")).ToArray();
        }
        else
        {
            var sel = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets).Cast<Object>().ToArray();
            assetPaths = sel.Select(o => AssetDatabase.GetAssetPath(o)).Where(s => !string.IsNullOrEmpty(s)).Distinct().ToArray();
        }

        int scanned = 0;
        for (int i = 0; i < assetPaths.Length; i++)
        {
            var path = assetPaths[i];
            scanned++;
            var deps = AssetDatabase.GetDependencies(path, true);
            var pkgDeps = deps.Where(d => d.StartsWith("Packages/")).ToArray();
            if (pkgDeps.Length > 0)
            {
                results[path] = pkgDeps.Select(d => GetPackageNameFromPackagePath(d)).Distinct().ToList();
            }

            if (i % 200 == 0)
            {
                if (EditorUtility.DisplayCancelableProgressBar("Scanning assets", $"Scanned {scanned}/{assetPaths.Length}", (float)scanned / assetPaths.Length))
                {
                    EditorUtility.ClearProgressBar();
                    Debug.Log("Scan cancelado pelo usuário.");
                    break;
                }
            }
        }

        EditorUtility.ClearProgressBar();
        Repaint();
        Debug.Log($"Scan completo. Assets com dependências em Packages/: {results.Count}");
    }

    string GetPackageNameFromPackagePath(string packagePath)
    {
        // Ex.: "Packages/com.unity.textmeshpro/Runtime/TMP_Text.cs" -> "com.unity.textmeshpro"
        if (string.IsNullOrEmpty(packagePath)) return packagePath;
        var parts = packagePath.Split('/');
        if (parts.Length >= 2) return parts[1];
        return packagePath;
    }

    void LogResults()
    {
        var sb = new StringBuilder();
        foreach (var kvp in results)
        {
            sb.AppendLine(kvp.Key);
            foreach (var p in kvp.Value) sb.AppendLine("    " + p);
        }
        Debug.Log(sb.ToString());
    }

    void ExportCSV()
    {
        var projectRoot = Directory.GetParent(Application.dataPath).FullName;
        var outPath = Path.Combine(projectRoot, "PackageDependenciesReport.csv");
        var sb = new StringBuilder();
        sb.AppendLine("Asset,PackageDependency");
        foreach (var kvp in results)
            foreach (var p in kvp.Value)
                sb.AppendLine($"\"{kvp.Key}\",\"{p}\"");
        File.WriteAllText(outPath, sb.ToString(), Encoding.UTF8);
        AssetDatabase.Refresh();
        Debug.Log($"CSV gerado em: {outPath}");
    }
}
