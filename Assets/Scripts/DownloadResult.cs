using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DownloadResult
{
    public string URL { get; set; }
    public ulong DownloadedBytes { get; set; }
    public long DownloadTime { get; set; }

    public override string ToString()
    {
        var url = URL.Substring(URL.LastIndexOf('/'));

        return (DownloadedBytes/1024/1024) + " MB,\t" + DownloadTime + " mS,\t" + url;
    }
}
