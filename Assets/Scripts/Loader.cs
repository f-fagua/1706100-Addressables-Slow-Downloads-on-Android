using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class Loader : MonoBehaviour
{
    
    private static int s_MaxDownloads = 0;
    
    [SerializeField]
    private LoadData m_LoadData;

    [SerializeField]
    private float m_StartTime = 2.5f;

    private int m_MaxDownloads;

    private bool m_DownloadStarted;
    
    private int m_CurrentActiveDownloads;

    private int m_CurrentDownloadedURL;

    private Stopwatch m_MainStopWatch;
    
    private Stopwatch[] m_WebRequestStopWatch;

    private WebRequestWrapper[] m_Downloaders;
    
    private int m_CurrentDownload = 0;

    private int m_DownloadsCompleted = 0;

    private DownloadResults m_DownloadResults;

    private void Awake()
    {
        m_MaxDownloads = s_MaxDownloads;
        if (s_MaxDownloads == 0)
            s_MaxDownloads = 2;
        else
            s_MaxDownloads *= 2;
        
    }

    private void Start()
    {
        Caching.ClearCache();
        
        InitDownloaders();

        if (m_MaxDownloads == 0)
            m_MaxDownloads = m_LoadData.URLCount;
        
        m_DownloadResults = new DownloadResults(m_MaxDownloads);

        Invoke("StartDownload", m_StartTime);
    }

    private void InitDownloaders()
    {
        m_Downloaders = new WebRequestWrapper[m_LoadData.URLCount];
        for (var i = 0; i < m_LoadData.URLCount; i++)
        {
            var wrapper = gameObject.AddComponent<WebRequestWrapper>();
            wrapper.OnCompleted += OnDownloadCompleted;
            m_Downloaders[i] = wrapper;
        }
    }

    private void OnDownloadCompleted(string url, ulong downloadedBytes, long milliseconds)
    {
        m_DownloadResults.AddResult(url, downloadedBytes, milliseconds);
        
        Debug.Log($"Download Completed:\turl: {url}, downloadedBytes: {downloadedBytes}, time: {milliseconds}");
        
        m_CurrentActiveDownloads--;
        m_DownloadsCompleted++;
        
        if (m_DownloadsCompleted >= m_LoadData.URLCount)
        {
            m_MainStopWatch.Stop();
            m_DownloadResults.Miliseconds = m_MainStopWatch.ElapsedMilliseconds;
            
            Debug.Log(m_DownloadResults);

            CreateResultsFile();

            if (s_MaxDownloads < m_LoadData.URLCount)
                SceneManager.LoadScene(0);
                
            return;
        }
        
        if (m_CurrentDownload < m_LoadData.URLCount && m_CurrentActiveDownloads <= m_MaxDownloads)
            DownloadNextItem();
    }

    private void CreateResultsFile()
    {
        var now = DateTime.Now;
        var timeStamp = now.ToString("yyyyMMdd_HHmmss");
        var fileName = "Download Report_" + timeStamp + ".txt";
        var fullPath = "";
#if UNITY_EDITOR
        fullPath = Path.Combine(Application.dataPath, fileName);
#else
        fullPath = Path.Combine(Application.persistentDataPath, filename);
#endif
        Debug.Log($"###### File created at {fullPath}");
        File.WriteAllText(fullPath, m_DownloadResults.ToString());
    }

    private void StartDownload()
    {
        m_DownloadStarted = true;
        m_MainStopWatch = new Stopwatch();
        m_MainStopWatch.Start();
        for (int i = 0; i < m_LoadData.URLCount && m_CurrentActiveDownloads < m_MaxDownloads; i++)
        {
            DownloadNextItem();
        }
    }

    private void Update()
    {   
        if (!m_DownloadStarted || 
            m_CurrentDownload >= m_LoadData.URLCount || 
            m_CurrentActiveDownloads >= m_MaxDownloads)
            return;

        DownloadNextItem();
    }

    private void DownloadNextItem()
    {
        StartCoroutine(m_Downloaders[m_CurrentDownload].StartDownload(m_LoadData[m_CurrentDownload]));
        m_CurrentDownload++;
        m_CurrentActiveDownloads++;
    }
}

