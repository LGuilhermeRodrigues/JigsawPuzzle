using System;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

public class FileManager : MonoBehaviour
{
    public static string jigsawPath;
    public static string brainnPath;

    void Awake()
    {
        CreateStartDirectories();
        CreateExampleFiles();
        LoadConfigurationFile();
        CreateWatcher();
    }

    private void LoadConfigurationFile()
    {
        Debug.Log("Loading the configuration file");
        // Check if Config.txt exists in the documents/BRAINN_XR_Data folder
        string currentPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BRAINN_XR_Data", "Config.txt");
        if (File.Exists(currentPath))
        {
            // Read the file
            string[] lines = File.ReadAllLines(currentPath);
            
            // Each line have the format: "gameName configName Value" (e.g. "ePuzzle HorizontalSize 3")
            foreach (string line in lines)
            {
                string[] words = line.Split(' ');
                if (words.Length == 3)
                {
                    // Concat the first two words to get the variable name
                    string variableName = words[0] + words[1];
                    // Check if the variable name is in the list of variables
                    // Set the value of the variable
                    switch (variableName)
                    {
                        case "ePuzzleColumns":
                            Configuration.ePuzzleColumns = int.Parse(words[2]);
                            break;
                        case "ePuzzleRows":
                            Configuration.ePuzzleRows = int.Parse(words[2]);
                            break;
                        case "ePuzzleAspectRatio":
                            // parse string to enum
                            Debug.Log("Parsing string to enum");
                            Enum.TryParse(words[2], out Configuration.AspectRatio aspectRatio);
                            Debug.Log($"Parsed to {aspectRatio.ToString()}");
                            Configuration.ePuzzleAspectRatio = aspectRatio;
                            break;
                        case "ePuzzleImage":
                            Configuration.ePuzzleImage = words[2];
                            break;
                        case "ePuzzleImageTransparency":
                            Configuration.ePuzzleImageTransparency = float.Parse(words[2]);
                            break;
                        case "ePuzzleBackgroundColor":
                            // parse r,g,b string to color
                            string[] rgb = words[2].Split(',');
                            Configuration.ePuzzleBackgroundColor = new Color(float.Parse(rgb[0])/255, float.Parse(rgb[1])/255, float.Parse(rgb[2])/255);
                            break;
                    }
                }
            }
        }
    }

    private void CreateWatcher()
    {
        Debug.Log("Create a watcher for the configuration file");
        // Create a system watcher for the file (if it is modified, the game will reload the configuration)
        FileSystemWatcher watcher = new FileSystemWatcher();
        watcher.Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BRAINN_XR_Data");
        watcher.NotifyFilter = NotifyFilters.LastWrite;
        watcher.Filter = "Config.txt";
        watcher.Changed += OnChanged;
        watcher.EnableRaisingEvents = true;
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        Debug.Log("Configuration file has been modified");
        LoadConfigurationFile();
    }

    private void CreateStartDirectories()
    {
        // Check if the documents folder of windows is accessible
        string currentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        if (Directory.Exists(currentPath))
        {
            Debug.Log("Documents folder is accessible");
            // Create a new folder (BRAINN_XR_Data) in the documents folder if it doesn't exist already
            currentPath = Path.Combine(currentPath, "BRAINN_XR_Data");
            if (!Directory.Exists(currentPath))
                Directory.CreateDirectory(currentPath);
            brainnPath = currentPath;
            // Also, create a subfolder for the Jigsaw Puzzle
            currentPath = Path.Combine(currentPath, "Images");
            if (!Directory.Exists(currentPath))
                Directory.CreateDirectory(currentPath);
            jigsawPath = currentPath;
        }
        else
        {
            Debug.LogError("Documents folder is not accessible");
        }
    }

    private void CreateExampleFiles()
    {
        // Move all example files from the Resources/ePuzzle folder to the Documents/BRAINN_XR_Data folder
        string[] files = { "beach", "cat", "bike", "rainbow", "bob" };
        foreach (string file in files)
        {
            // load the image resource
            Texture2D texture = Resources.Load(Path.Combine("ePuzzle", file)) as Texture2D;
            // Check if the directory jigsawExamplePath is accessible
            if (jigsawPath!=null && Directory.Exists(jigsawPath))
            {
                // Check if the file already exists in the destination folder
                if (!File.Exists(Path.Combine(jigsawPath, $"{file}.png")))
                {
                    // Copy the file to the destination folder
                    File.WriteAllBytes(Path.Combine(jigsawPath, $"{file}.png"), texture.EncodeToPNG());
                }
            }
        }
        // Load Config.txt file from the Resources folder and copy it to the Documents/BRAINN_XR_Data folder
        var configuration = Resources.Load("Config") as TextAsset;
        if (configuration != null)
            File.WriteAllText(Path.Combine(brainnPath, "ConfigDefaultValues.txt"), configuration.text);
        if (!File.Exists(Path.Combine(brainnPath, "Config.txt")))
        {
            if (configuration != null)
                File.WriteAllText(Path.Combine(brainnPath, "Config.txt"), configuration.text);
        }
    }
}
