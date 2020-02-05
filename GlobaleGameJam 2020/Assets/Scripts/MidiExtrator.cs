using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Partition;

public class MidiExtrator : MonoBehaviour
{
    #region InternalTypes
    [Serializable] public class NoteToInput
    {
        public NoteName NoteName;
        public int Octave;
        public VirtualNote NoteInGame;
    }
    #endregion

    [SerializeField] int _midiClock = 24;
    [SerializeField, ReadOnly] string _extension = ".mid";
    [SerializeField] NoteToInput[] _noteConf;

    public IEnumerable<(VirtualNote note, int tick)> ExtractMidi(string fileName, bool debug=false)
    {

#if UNITY_EDITOR
        var midiFile = MidiFile.Read("Assets/StreamingAssets/" + fileName + _extension);
#else
        var midiFile = MidiFile.Read(Application.streamingAssetsPath +"/"+ fileName + _extension);
#endif

        var tempoMap = midiFile.GetTempoMap();

        using (var notesManager = midiFile.GetTrackChunks().Where(i => i.Events.Count > 10).First().ManageNotes())
        {
            var data = notesManager.Notes
                .Select(i => (_noteConf.FirstOrDefault(j => j.NoteName == i.NoteName)?.NoteInGame ?? VirtualNote.None, (int)i.Time / _midiClock));

            if (debug) Debug.Log(data.ToFlatString(true));
            return data;
        }
    }


#region Editor
    [SerializeField, BoxGroup("Editor")] string _fileName;
    [Button("Test Midi file")]
    void TryExtractFile()
    {
        var data = ExtractMidi(_fileName, true);
    }

#endregion


}
