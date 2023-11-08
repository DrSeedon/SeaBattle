using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class WebRequestManager
{
    public static IEnumerator SendPostRequest(MonoBehaviour caller, string url, string jsonBody, Action<string> onSuccess, Action<string> onError, bool needAuth = false)
    {
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        if (needAuth)
        {
            string accessToken = PersistentData.Instance.Get<string>("AccessToken");
            request.SetRequestHeader("Authorization", "Bearer " + accessToken);
        }

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            var errorResponse = JsonUtility.FromJson<ErrorResponse>(request.downloadHandler.text);
            onError?.Invoke(errorResponse?.error ?? "Unknown error");
        }
        else
        {
            onSuccess?.Invoke(request.downloadHandler.text);
        }
    }

    public static IEnumerator SendGetRequest(MonoBehaviour caller, string url, Action<string> onSuccess, Action<string> onError, bool needAuth = true)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        if (needAuth)
        {
            string accessToken = PersistentData.Instance.Get<string>("AccessToken");
            request.SetRequestHeader("Authorization", "Bearer " + accessToken);
        }

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke(request.error);
        }
        else
        {
            onSuccess?.Invoke(request.downloadHandler.text);
        }
    }


    [Serializable]
    private class ErrorResponse
    {
        public string error;
    }

}
