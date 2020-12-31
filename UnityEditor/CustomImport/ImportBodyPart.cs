using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ImportBodyPart {

    //&&& Allow overwrite with specific head sizes
    int spriteHeight = 50;
    int spriteWidth = 70;
    int framerate = 15;

    // Access the Texture Importer, make necessary modifications and set up animations.
    public void Process(string theTexture, string[] parsedWords)
    {

        string texturePath = AssetDatabase.GUIDToAssetPath(theTexture);
        TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(texturePath);
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.filterMode = FilterMode.Point;


        // Figure out show many parts of the sprite sheet should exist, then split them up
        //&&& add debug check for no remainders
        Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D));
        int colCount = texture.width / spriteWidth;
        int rowCount = texture.height / spriteHeight;

        // Create the new sprites as metadata attached to the texture
        List<SpriteMetaData> metas = new List<SpriteMetaData>();
        for (int r = 0; r < rowCount; ++r)
        {
            for (int c = 0; c < colCount; ++c)
            {
                SpriteMetaData meta = new SpriteMetaData();
                meta.rect = new Rect(c * spriteWidth, r * spriteHeight, spriteWidth, spriteHeight);
                meta.name = c + "-" + r;
                metas.Add(meta);
            }
        }

        // Set the new meta-sprites, then call an update to save.
        importer.spritesheet = metas.ToArray();
        AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);

        //Get all the sprites from the texture into an array to use for building
        Object[] dataToParse = AssetDatabase.LoadAllAssetsAtPath(texturePath);
        Sprite[] spritesToUse = new Sprite[rowCount * colCount];
        int i = 0;
        foreach (Object o in dataToParse)
        {
            if (o.GetType().ToString() == "UnityEngine.Sprite")
            {
               spritesToUse[i] = o as Sprite;
               i++;
            } 
        }

        //Move the Asset from the import folder into the Texture Folder

        if (!AssetDatabase.IsValidFolder("Assets/GameAssets/Textures"))
        {
            AssetDatabase.CreateFolder("Assets/GameAssets", "Textures");
        }

        if (!AssetDatabase.IsValidFolder("Assets/GameAssets/Textures/" + parsedWords[0]))
        {
            AssetDatabase.CreateFolder("Assets/GameAssets/Textures", parsedWords[0]);
        }

        if (!AssetDatabase.IsValidFolder("Assets/GameAssets/Textures/" + parsedWords[0] + "/" + parsedWords[1]))
        { 
            string preFolderMap = AssetDatabase.CreateFolder("Assets/GameAssets/Textures/" + parsedWords[0], parsedWords[1]);
        }

        AssetDatabase.MoveAsset(texturePath, "Assets/GameAssets/Textures/" + parsedWords[0] + "/" + parsedWords[1] + "/" + parsedWords[0] + "_" + parsedWords[1] + "_" + parsedWords[2] + ".png");

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%            GAP                  %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        // Build out Animations folders if they don't exist

        if (!AssetDatabase.IsValidFolder("Assets/GameAssets/Animations"))
        {
            AssetDatabase.CreateFolder("Assets/GameAssets", "Animations");
        }

        if (!AssetDatabase.IsValidFolder("Assets/GameAssets/Animations/" + parsedWords[0]))
        {
            AssetDatabase.CreateFolder("Assets/GameAssets/Animations", parsedWords[0]);
        }

        if (!AssetDatabase.IsValidFolder("Assets/GameAssets/Animations/" + parsedWords[0] + "/" + parsedWords[1]))
        { 
            string assFolderMap = AssetDatabase.CreateFolder("Assets/GameAssets/Animations/" + parsedWords[0], parsedWords[1]);
        }

        // Create a new animation and up it into an AnimatorController
        AnimationClip animation = new AnimationClip();
        animation.wrapMode= WrapMode.Loop;
        animation.name = parsedWords[1] + "_" + parsedWords[2];
        animation.frameRate = framerate;

        EditorCurveBinding spriteBinding = new EditorCurveBinding();
        spriteBinding.type = typeof(SpriteRenderer);
        spriteBinding.path = "";
        spriteBinding.propertyName = "m_Sprite"; 

        ObjectReferenceKeyframe[] spriteKeyFrames = new ObjectReferenceKeyframe[spritesToUse.Length];
        for(i = 0; i < (spritesToUse.Length); i++)
        {
            spriteKeyFrames[i] = new ObjectReferenceKeyframe();
            float timingi = (float)i;
            spriteKeyFrames[i].time = (timingi/framerate);
            spriteKeyFrames[i].value = spritesToUse[i];
        }
        AnimationUtility.SetObjectReferenceCurve(animation, spriteBinding, spriteKeyFrames);

        RuntimeAnimatorController controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPathWithClip("Assets/GameAssets/Animations/" + parsedWords[0] + "/" + parsedWords[1] + "/" + parsedWords[1] + "_" + parsedWords[2] + "_control.controller", animation);
        AssetDatabase.CreateAsset(animation, "Assets/GameAssets/Animations/" + parsedWords[0] + "/" + parsedWords[1] + "/" + parsedWords[1] + "_" + parsedWords[2] + "_anim.anim");
        // Build a GameObject to house these things, later to use as a prefab
        GameObject objectToBuild = new GameObject ();
        objectToBuild.name = parsedWords[0] + "_" + parsedWords[1] + "_" + parsedWords[2];

        // Add one of the sprites as the placeholder sprite.
        SpriteRenderer renderer = objectToBuild.AddComponent<SpriteRenderer>();
        renderer.sprite = spritesToUse[0];

        // Add our Animation controller
        Animator animator = objectToBuild.AddComponent<Animator>();
        animator.runtimeAnimatorController = controller;

        // Save the finished object as a prefab

        if (!AssetDatabase.IsValidFolder("Assets/GameAssets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets/GameAssets", "Prefabs");
        }

        if (!AssetDatabase.IsValidFolder("Assets/GameAssets/Prefabs/" + parsedWords[0]))
        {
            AssetDatabase.CreateFolder("Assets/GameAssets/Prefabs", parsedWords[0]);
        }

        if (!AssetDatabase.IsValidFolder("Assets/GameAssets/Prefabs/" + parsedWords[0] + "/" + parsedWords[1]))
        { 
            string preFolderMap = AssetDatabase.CreateFolder("Assets/GameAssets/Prefabs/" + parsedWords[0], parsedWords[1]);
        }

        PrefabUtility.SaveAsPrefabAssetAndConnect(objectToBuild, "Assets/GameAssets/Prefabs/" + parsedWords[0] + "/" + parsedWords[1] + "/" + parsedWords[0] + "_" + parsedWords[1] + "_" + parsedWords[2] + ".prefab", InteractionMode.UserAction);
    
        // Remove the GameObject and Original Raws and leave the Prefab and processed files
        Undo.DestroyObjectImmediate(objectToBuild);

        AssetDatabase.DeleteAsset(texturePath);
    }
}