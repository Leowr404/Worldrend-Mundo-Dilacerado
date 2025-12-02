using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private static string GetPath(int slot)
    {
        return Path.Combine(Application.persistentDataPath, $"save_slot_{slot}.json");
    }

    // ===============================
    // SALVAR
    // ===============================
    public static void Save(int slot, string json)
    {
        string path = GetPath(slot);
        File.WriteAllText(path, json);
        Debug.Log($"💾 Save criado no slot {slot} em:\n{path}");
    }

    // ===============================
    // CARREGAR
    // ===============================
    public static string Load(int slot)
    {
        string path = GetPath(slot);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            Debug.Log($"📂 Save carregado do slot {slot}.");
            return json;
        }

        Debug.LogWarning($"⚠ Nenhum save encontrado no slot {slot}.");
        return null;
    }

    // ===============================
    // EXCLUIR
    // ===============================
    public static void Delete(int slot)
    {
        string path = GetPath(slot);

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"🗑 Save do slot {slot} foi deletado.");
        }
        else
        {
            Debug.LogWarning($"⚠ Tentou deletar o slot {slot}, mas não existe arquivo.");
        }
    }

    // ===============================
    // VERIFICAR SE EXISTE SAVE
    // ===============================
    public static bool Exists(int slot)
    {
        return File.Exists(GetPath(slot));
    }
}
