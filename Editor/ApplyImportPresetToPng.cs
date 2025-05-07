using UnityEditor;
using UnityEngine;
using System.IO;

public class ApplyImportSettingsFromReference : EditorWindow
{
    // Reference texture from which we will copy the import settings
    public static string referenceTexturePath = "Assets/Resources/Sprites/ReferenceTexture.png"; // Adjust this path

    [MenuItem("Tools/Apply Import Settings from Reference to PNGs")]
    static void ApplyPresetToPngFiles()
    {
        // Load the reference texture's importer settings
        string referenceAssetPath = referenceTexturePath;
        TextureImporter referenceTextureImporter = (TextureImporter)AssetImporter.GetAtPath(referenceAssetPath);

        if (referenceTextureImporter == null)
        {
            Debug.LogError("Reference texture not found or is not a texture.");
            return;
        }

        // Log reference texture settings to confirm
        Debug.Log($"Reference Texture Settings: Readable={referenceTextureImporter.isReadable}, FilterMode={referenceTextureImporter.filterMode}, WrapMode={referenceTextureImporter.wrapMode}, Compression={referenceTextureImporter.textureCompression}, MipmapEnabled={referenceTextureImporter.mipmapEnabled}, MaxTextureSize={referenceTextureImporter.maxTextureSize}, SpriteMode={referenceTextureImporter.spriteImportMode}");

        // Directory to scan for PNG files

        // string directoryPath = "Assets/Resources/Sprites/Characters/PriestessHorizontalFireball";
        // string directoryPath = "Assets/Resources/Sprites/Characters/Priestess/Smears";
        string directoryPath = "Assets/Resources/Sprites/Characters/Priestess/FrameGroups/_5H";
        // string directoryPath = "Assets/Resources/Sprites/AnimationEntities";
        
        string[] pngFiles = Directory.GetFiles(directoryPath, "*.png", SearchOption.AllDirectories);

        // Loop through each PNG file
        foreach (string filePath in pngFiles)
        {
            // Convert the file path to a Unity asset path
            string assetPath = filePath.Replace(Application.dataPath, "Assets");

            // Get the texture importer for this PNG file
            TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(assetPath);

            if (textureImporter != null)
            {
                // Log current settings for the texture
                Debug.Log($"Applying settings to {assetPath}: Readable={textureImporter.isReadable}, FilterMode={textureImporter.filterMode}, WrapMode={textureImporter.wrapMode}, Compression={textureImporter.textureCompression}, MipmapEnabled={textureImporter.mipmapEnabled}, MaxTextureSize={textureImporter.maxTextureSize}, SpriteMode={textureImporter.spriteImportMode}");

                // Apply the settings from the reference texture
                textureImporter.isReadable = referenceTextureImporter.isReadable;
                textureImporter.filterMode = referenceTextureImporter.filterMode;
                textureImporter.wrapMode = referenceTextureImporter.wrapMode;
                textureImporter.textureCompression = referenceTextureImporter.textureCompression;
                textureImporter.mipmapEnabled = referenceTextureImporter.mipmapEnabled;
                textureImporter.maxTextureSize = referenceTextureImporter.maxTextureSize;
                textureImporter.textureType = referenceTextureImporter.textureType;
                textureImporter.spriteImportMode = referenceTextureImporter.spriteImportMode; // Set sprite mode

                // Log the changes
                Debug.Log($"Updated settings for {assetPath}: Readable={textureImporter.isReadable}, FilterMode={textureImporter.filterMode}, WrapMode={textureImporter.wrapMode}, Compression={textureImporter.textureCompression}, MipmapEnabled={textureImporter.mipmapEnabled}, MaxTextureSize={textureImporter.maxTextureSize}, SpriteMode={textureImporter.spriteImportMode}");

                // Apply the changes to the asset
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
                Debug.Log($"Applied settings from reference to: {assetPath}");
            }
            else
            {
                Debug.LogError($"Failed to get TextureImporter for: {assetPath}");
            }
        }

        // Refresh the Asset Database to reflect changes
        AssetDatabase.Refresh();
        Debug.Log("Asset import process completed, and database refreshed.");
    }
}
