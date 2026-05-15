using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private static string GetPath(int slot) =>
        Path.Combine(Application.persistentDataPath, $"save_slot_{slot}.json");

    public static void Save(int slot, string json) =>
        File.WriteAllText(GetPath(slot), json);

    public static string Load(int slot)
    {
        string path = GetPath(slot);
        return File.Exists(path) ? File.ReadAllText(path) : null;
    }

    public static void Delete(int slot)
    {
        string path = GetPath(slot);
        if (File.Exists(path)) File.Delete(path);
    }

    public static bool Exists(int slot) => File.Exists(GetPath(slot));
}
