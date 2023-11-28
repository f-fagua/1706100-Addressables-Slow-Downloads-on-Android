using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

public class WebRequestWrapper : MonoBehaviour
{

    private string m_URL;

    public event OnCompleted OnCompleted;
    
    public IEnumerator StartDownload(string URL)
    {
        Debug.Log("Starting " + URL + " download");
        m_URL = URL;
        var stopwatch = new Stopwatch(); 
        stopwatch.Start();

        using(var webRequest = UnityWebRequest.Get(m_URL))
        {
            yield return webRequest.SendWebRequest();

            stopwatch.Stop();
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                OnCompleted(m_URL, webRequest.downloadedBytes, stopwatch.ElapsedMilliseconds);
            }
        }

        stopwatch.Stop();
    }
}

public delegate void OnCompleted(string url, ulong downloadedBytes, long milliseconds);