using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
/* Imports all Nathans sprites at the 'path' path, interprets the meaning in line with our accepted namesystem, 
and incorporates them into the game as defined. */

public class ImportSprite : EditorWindow {

    // Get access to the different types of thing to deal with
    ImportHead importHead = new ImportHead();


    // Default path used to take in files
    string path = "Assets/SpritesAll";

    // Creates a toolbar, when selected implicitly calls ShowWindow()
    [MenuItem("DansCustomTools/ImportNathSprites")]
    private static void ShowWindow()
    {
        // Creates the main window, implicitly takes the content from OnGUI()
        var window = GetWindow<ImportSprite>();
        window.titleContent = new GUIContent("ImportSprite");
        window.Show();
    }

    // Defines shown content in window
    private void OnGUI()
    {
        // Default path/sprite size set, but can be changed.
        path = EditorGUILayout.TextField("path", path);

        // Once happy, pressing this will begin the import process.
        if (GUILayout.Button("Do the thing"))
            this.ModTexture();
       
        if (GUILayout.Button("Close"))
            this.Close();
    }

    // Use the TextureImporter to modify the texture to the correct settings
    private void ModTexture()
    {
        // Get all of the textures in the path folder
        string[] allTextures = AssetDatabase.FindAssets("t:texture2D", new[] {path});

        // For each texture selected this way make the default changes
        foreach (string newTexture in allTextures)
        {
            string[] parsedWords = parseWords(newTexture);
            if (parsedWords[0] == "head")
            {
                importHead.Process(newTexture, parsedWords);
            }
            /*
            else if (firstword == "body")
            {
                importBody.Process(newTexture);
            }
            else if (firstword == "hand")
            {
                importBody.Process(newTexture);
            }
            */
            else
            {
                Debug.Log("NOT RECOGNISED: ALEEEEEEERRRTT");
            }
        }
    }


    // Take the path name and process it into the set of strings to pass to processing
    private string[] parseWords(string fullString)
    {
        fullString = AssetDatabase.GUIDToAssetPath(fullString);
        // Strip the proper string from the asset path
        fullString = fullString.Remove(0, path.Length + 1);
        fullString = fullString.Remove(fullString.Length - 4, 4);

        string[] parsed = fullString.Split('_');

        return parsed;
    }
}