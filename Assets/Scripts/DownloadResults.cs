using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownloadResults
{
    public List<DownloadResult> Results { get; set; }

    public long Miliseconds { get; set; }

    public int ConcurrentDownloads { get; set; }
    
    public DownloadResults(int maxDownloads)
    {
        ConcurrentDownloads = maxDownloads;
    }

    public void AddResult(string url, ulong downloadedBytes, long time)
    {
        var result = new DownloadResult{DownloadedBytes = downloadedBytes, DownloadTime = time, URL = url};
        if (Results == null)
            Results = new List<DownloadResult>();
        Results.Add(result);
    }

    public override string ToString()
    {
        ulong downloadedBytes = 0;
        
        foreach (var downloadResult in Results)
            downloadedBytes += downloadResult.DownloadedBytes;

        var msj = $"Total Downloads {Results.Count}, downloaded bytes {downloadedBytes/1024/1024} MB, total time {Miliseconds} ms, concurrent downloads {ConcurrentDownloads}";
        
        foreach (var downloadResult in Results)
            msj += "\n- " + downloadResult;

        return msj;
    }
}
