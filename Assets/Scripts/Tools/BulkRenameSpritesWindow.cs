using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class BulkRenameSpritesWindow : EditorWindow
{
    private string folderPath = "Assets/";
    private string prefix = "";
    private string suffix = "";
    private int startNumber = 1;
    private int numberDigits = 2;
    private bool includeSubfolders = false;
    private bool renameMultipleMode = false;
    private string namePattern = "sprite_{0}";

    [MenuItem("Tools/Bulk Rename Sprites")]
    public static void ShowWindow()
    {
        GetWindow<BulkRenameSpritesWindow>("Bulk Rename Sprites");
    }

    void OnGUI()
    {
        GUILayout.Label("Bulk Rename Sprite Settings", EditorStyles.boldLabel);

        // 文件夹选择
        EditorGUILayout.BeginHorizontal();
        folderPath = EditorGUILayout.TextField("Folder Path", folderPath);
        if (GUILayout.Button("Browse", GUILayout.Width(80)))
        {
            string path = EditorUtility.OpenFolderPanel("Select Folder", folderPath, "");
            if (!string.IsNullOrEmpty(path))
            {
                folderPath = "Assets" + path.Replace(Application.dataPath, "");
            }
        }
        EditorGUILayout.EndHorizontal();

        // 重命名模式选择
        renameMultipleMode = EditorGUILayout.Toggle("Multiple Sprites per Texture", renameMultipleMode);

        if (renameMultipleMode)
        {
            // 多Sprite模式设置
            namePattern = EditorGUILayout.TextField("Name Pattern", namePattern);
            EditorGUILayout.HelpBox("Use {0} as placeholder for number (e.g. 'sprite_{0}' becomes 'sprite_01')", MessageType.Info);
        }
        else
        {
            // 单Sprite模式设置
            prefix = EditorGUILayout.TextField("Prefix", prefix);
            suffix = EditorGUILayout.TextField("Suffix", suffix);
            startNumber = EditorGUILayout.IntField("Start Number", startNumber);
            numberDigits = EditorGUILayout.IntSlider("Number Digits", numberDigits, 1, 5);
        }

        includeSubfolders = EditorGUILayout.Toggle("Include Subfolders", includeSubfolders);

        EditorGUILayout.Space();

        if (GUILayout.Button("Rename Sprites", GUILayout.Height(40)))
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                EditorUtility.DisplayDialog("Error", "Please select a folder first!", "OK");
                return;
            }

            if (EditorUtility.DisplayDialog("Confirm",
                "Are you sure you want to rename all sprites in this folder?",
                "Yes", "No"))
            {
                RenameSprites();
            }
        }
    }

    private void RenameSprites()
    {
        string[] textureGUIDs = AssetDatabase.FindAssets("t:Texture2D",
            includeSubfolders ? new[] { folderPath } : new[] { folderPath });

        if (textureGUIDs.Length == 0)
        {
            EditorUtility.DisplayDialog("Info", "No textures found in the selected folder.", "OK");
            return;
        }

        int renamedCount = 0;
        int currentNumber = startNumber;

        AssetDatabase.StartAssetEditing();

        try
        {
            foreach (string guid in textureGUIDs)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

                if (importer == null || importer.textureType != TextureImporterType.Sprite)
                    continue;

                // 处理单Sprite和多Sprite模式
                if (importer.spriteImportMode == SpriteImportMode.Single)
                {
                    // 单Sprite重命名
                    string newName = $"{prefix}{currentNumber.ToString().PadLeft(numberDigits, '0')}{suffix}";
                    importer.spritePackingTag = "";
                    importer.spritePixelsPerUnit = 100;
                    importer.SaveAndReimport();

                    // 获取Sprite子对象并重命名
                    Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
                    foreach (Object asset in assets)
                    {
                        if (asset is Sprite && asset.name == Path.GetFileNameWithoutExtension(path))
                        {
                            asset.name = newName;
                            EditorUtility.SetDirty(asset);
                            renamedCount++;
                            currentNumber++;
                            break;
                        }
                    }
                }
                else if (importer.spriteImportMode == SpriteImportMode.Multiple)
                {
                    // 多Sprite重命名
                    SpriteMetaData[] spritesheet = importer.spritesheet;
                    bool changed = false;

                    for (int i = 0; i < spritesheet.Length; i++)
                    {
                        string oldName = spritesheet[i].name;
                        string newName = renameMultipleMode
                            ? string.Format(namePattern, (currentNumber + i).ToString().PadLeft(numberDigits, '0'))
                            : $"{prefix}{(currentNumber + i).ToString().PadLeft(numberDigits, '0')}{suffix}";

                        if (oldName != newName)
                        {
                            spritesheet[i].name = newName;
                            changed = true;
                        }
                    }

                    if (changed)
                    {
                        importer.spritesheet = spritesheet;
                        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                        renamedCount += spritesheet.Length;
                        currentNumber += spritesheet.Length;
                    }
                }
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh();
        }

        EditorUtility.DisplayDialog("Complete",
            $"Successfully renamed {renamedCount} sprites.", "OK");
    }
}