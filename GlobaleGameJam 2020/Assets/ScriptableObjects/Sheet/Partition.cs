using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName ="GGJ/Sheet")]
public class Partition : ScriptableObject
{
    #region InternalType
    public enum VirtualNote { None = -1, Up=0, Down=1, Left=2, Right=3}
    #endregion

    [SerializeField] int _bpm = 60;
    [SerializeField] AudioClip _music;
    [SerializeField] string _midiFileName;
    [SerializeField] int _offset = 1000;

    public AudioClip Music => _music;
    public int BPM => _bpm;
    public int Offset => _offset;
    public string FileName => _midiFileName;
    

}
