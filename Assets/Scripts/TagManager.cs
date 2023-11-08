using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class TagData
{
    public List<string> tags =new();
}

public class TagManager : MonoBehaviour
{
    [SerializeField]private TagData tagData = new TagData();
    private string filePath = "Assets/Json/tags.json"; // Spécifiez le chemin du fichier JSON.

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        LoadTags();
    }

    public bool TagExists(string tagName)
    {
        return tagData.tags.Contains(tagName);
    }

    public void CreateTag(string tagName)
    {
        if (!TagExists(tagName))
        {
            tagData.tags.Add(tagName);
            SaveTags();
        }
    }

    private void LoadTags()
    {
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            tagData = JsonUtility.FromJson<TagData>(jsonData);
        }
        else
        {
            tagData = new TagData();
        }
    }

    private void SaveTags()
    {
        string jsonData = JsonUtility.ToJson(tagData);
        File.WriteAllText(filePath, jsonData);
    }
}
