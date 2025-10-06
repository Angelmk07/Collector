using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public string name;
    public int levelsPassed;
}

public class FirebaseSimple : MonoBehaviour
{

    string firebaseURL = "https://collector-debt-game-default-rtdb.firebaseio.com/leaderboard.json";

    [System.Obsolete]
    public void SendScore(string playerName, int levels)
    {
        PlayerData data = new PlayerData { name = playerName, levelsPassed = levels };
        string json = JsonUtility.ToJson(data);

        StartCoroutine(PostScoreCoroutine(json));
    }

    [System.Obsolete]
    IEnumerator PostScoreCoroutine(string json)
    {
        using (UnityWebRequest request = UnityWebRequest.Post(firebaseURL, json))
        {
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
                Debug.Log("✅ Score uploaded!");
            else
                Debug.LogError("❌ Upload failed: " + request.error);
        }
    }

    // -------------------- ЗАГРУЗКА ТОП-5 --------------------
    public void GetTop5(System.Action<List<PlayerData>> callback)
    {
        StartCoroutine(GetTop5Coroutine(callback));
    }

    IEnumerator GetTop5Coroutine(System.Action<List<PlayerData>> callback)
    {
        // Параметры Firebase REST:
        // orderBy="levelsPassed" → сортировка по полю
        // limitToLast=5 → берем 5 с наибольшим значением
        string url = firebaseURL + "?orderBy=\"levelsPassed\"&limitToLast=5";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                Debug.Log("JSON response: " + json);

                // Преобразуем JSON в словарь вручную без сторонних библиотек
                var list = new List<PlayerData>();
                if (!string.IsNullOrEmpty(json) && json != "null")
                {
                    // Убираем внешние { }
                    json = json.TrimStart('{').TrimEnd('}');
                    string[] entries = json.Split(new string[] { "}," }, System.StringSplitOptions.None);

                    foreach (var entry in entries)
                    {
                        string line = entry;
                        if (!line.EndsWith("}")) line += "}";

                        int colonIndex = line.IndexOf(':');
                        if (colonIndex < 0) continue;

                        string value = line.Substring(colonIndex + 1).Trim();
                        try
                        {
                            PlayerData player = JsonUtility.FromJson<PlayerData>(value);
                            list.Add(player);
                        }
                        catch
                        {
                            Debug.LogWarning("Failed to parse JSON entry: " + value);
                        }
                    }

                    // Сортировка по убыванию
                    list.Sort((a, b) => b.levelsPassed.CompareTo(a.levelsPassed));
                }

                callback?.Invoke(list);
            }
            else
            {
                Debug.LogError("Error getting top 5: " + request.error);
                callback?.Invoke(new List<PlayerData>());
            }
        }
    }
}
