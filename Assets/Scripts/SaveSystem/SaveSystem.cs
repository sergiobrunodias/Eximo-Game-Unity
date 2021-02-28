using static utils.Constants;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem {

    /// <summary>
    /// Saves a match in storage to be loaded later.
    /// </summary>
    /// <param name="match">Match to save</param>
    public static void SaveMatch(Match match) {
        // If maximum number of saved matches is reached, the oldest is deleted.
        string[] savedMatches = System.IO.Directory.GetFiles(Application.persistentDataPath);
        if(savedMatches.Length > MaxSavedMatches) {
            File.Delete(savedMatches[MaxSavedMatches]);
        }
        BinaryFormatter formatter = new BinaryFormatter();
        string currentTime = (System.DateTime.Now + "").Replace("/", "-").Replace(":", ".");
        string filePath = Application.persistentDataPath + "/" + gameModes[match.gameMode] + " - " + currentTime;
        FileStream fileStream = new FileStream(filePath, FileMode.Create);
        formatter.Serialize(fileStream, match);
        Debug.Log(filePath);
        fileStream.Close();
    }

    /// <summary>
    /// Reads a match from a file and returns it.
    /// </summary>
    /// <param name="fileName">File to read the match from</param>
    /// <returns>The Match object</returns>
    public static Match LoadMatch(string fileName) {
        string filePath = Application.persistentDataPath + "/" + fileName;
        if(File.Exists(filePath)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(filePath, FileMode.Open);
            Match loadedMatch = formatter.Deserialize(fileStream) as Match;
            fileStream.Close();
            return loadedMatch;
        }
        Debug.LogError("Match \"" + fileName + "\" could not be found!");
        return null;
    }
}