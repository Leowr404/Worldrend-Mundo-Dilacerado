using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string savePath = Application.persistentDataPath + "/save.json";

    // 🟢 Salvar dados
    public static void Save(string json)
    {
        File.WriteAllText(savePath, json);
        Debug.Log("💾 Jogo salvo em: " + savePath);
    }

    // 🟡 Carregar dados
    public static string Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            Debug.Log("📂 Jogo carregado de: " + savePath);
            return json;
        }

        Debug.LogWarning("⚠️ Nenhum save encontrado!");
        return null;
    }

    // 🔴 Deletar save (opcional)
    public static void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("🗑️ Save deletado!");
        }
    }
}
