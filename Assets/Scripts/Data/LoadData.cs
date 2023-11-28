using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(fileName = "Load Data", menuName = "Scriptable Objects/Load URLs")]
public class LoadData : ScriptableObject
{
    [SerializeField]
    private string[] m_URLs;

    public int URLCount => m_URLs.Length;

    public string this[int index] => m_URLs[index];
}
