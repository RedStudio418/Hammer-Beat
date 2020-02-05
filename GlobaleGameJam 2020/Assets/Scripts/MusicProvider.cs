using NaughtyAttributes;
using SynchronizerData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Partition;

public class MusicProvider : BeatObserver
{
    [SerializeField, BoxGroup("Managers"), Required] MidiExtrator _midiToSheet;
    [SerializeField, BoxGroup("Managers"), Required] BeatCounter _beatCounter;
    [SerializeField, BoxGroup("BasicConf")] AudioSource _audioSource;
    [SerializeField, BoxGroup("Sheet")] Partition _sheet;

    float _atomicInterval;
    float _nextBeat;
    int _tickNumber;
    IEnumerator<(VirtualNote note, int tick)> _currentSheet;
    Trigger _next;

    public Partition CurrentSheet => _sheet;

    public event Action OnLaunchMusic;
    public event Action OnFinishMusic;
    public UnityEvent OnPrepareMusic;


    private void Awake()
    {
        _next = new Trigger();
    }

    public void LaunchMusic(Partition p)
    {
        _sheet = p;
        OnPrepareMusic.Invoke();

        _audioSource.clip = _sheet.Music;
        _beatCounter.Init();
        _currentSheet = _midiToSheet.ExtractMidi(_sheet.FileName).GetEnumerator();
        _currentSheet.MoveNext();
        _tickNumber = -1;
        OnLaunchMusic?.Invoke();
        return;
    }

    public override void BeatNotify(BeatType beatType)
    {
        base.BeatNotify(beatType);
        _tickNumber++;
        // Debug.Log($"Tick N°{_tickNumber}");
        _next.Activate();
    }

    int _noteNumber;
    internal VirtualNote NextNote()
    {
        if(_next.IsActivated())
        {
            if(_currentSheet.Current.tick == _tickNumber)
            {
                _noteNumber++;
                Debug.Log("note number " + _noteNumber);
                var valueToReturn = _currentSheet.Current.note;
                if (!_currentSheet.MoveNext()) OnFinishMusic?.Invoke();

                while(_currentSheet.Current.tick == _tickNumber && _currentSheet.MoveNext())
                {
                    
                }
                
                return valueToReturn;
            }
        }

        return VirtualNote.None;
    }

}
