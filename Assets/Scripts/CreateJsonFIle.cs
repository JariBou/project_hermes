using System;
using System.Collections;
using System.Collections.Generic;
/*using UnityEditor.U2D.Aseprite;*/
using UnityEngine;
using System.IO;

public class CreateJsonFIle : MonoBehaviour
{
    public string filePath = "Assets/Json/tags.json"; // Spécifiez le chemin du fichier JSON.

    private void Start()
    {
        // Vérifiez si le fichier JSON existe déjà.
        if (!File.Exists(filePath))
        {
            CreateTagsFile();
        }
        else
        {
            /*Debug.Log("Le fichier JSON existe déjà.");*/
        }
    }

    public void CreateTagsFile()
    {
        TagData tagData = new TagData();
        tagData.tags = new List<string> { "Tag1", "Tag2", "Tag3" }; // Ajoutez les tags initiaux ici.

        string jsonData = JsonUtility.ToJson(tagData, true); // La deuxième argument est pour l'indentation.

        try
        {
            File.WriteAllText(filePath, jsonData);
            Debug.Log("Fichier JSON de tags créé avec succès !");
        }
        catch (Exception e)
        {
            Debug.LogError("Erreur lors de la création du fichier JSON : " + e.Message);
        }
    }
}
