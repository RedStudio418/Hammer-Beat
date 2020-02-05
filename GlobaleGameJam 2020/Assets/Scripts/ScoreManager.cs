using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static ValidationNote;

public class ScoreManager : MonoBehaviour
{
    #region InternalTypes
    [Serializable] class ScoreConf
    {
        public ValidationRank Rank;
        public int Score;
    }

    [Serializable] class ScoreResume
    {
        public ValidationRank Rank;
        public int CurrentCount;
    }
    #endregion

    [SerializeField, BoxGroup("External Manager")] Combo _combo;
    [SerializeField, BoxGroup("Score Conf")] ScoreConf[] _scoreConf;
    [SerializeField, BoxGroup("UI Ref"), Required] Text _text;

    ScoreResume[] _currentResumeScore;
    int _missedNotes;

    int _currentScore = 0;

    public event Action<InGameNote> OnScoreUp;
    public UnityEvent OnNoteAccepted;
    public UnityEvent OnNoteMissed;

    private void Awake()
    {
        InitScore();
    }

    public void InitScore()
    {
        _text.text = "";
        _currentScore = 0;

        _currentResumeScore = new ScoreResume[] {
            new ScoreResume() { Rank = ValidationRank.Boarf},
            new ScoreResume() { Rank = ValidationRank.Good},
            new ScoreResume() { Rank = ValidationRank.VeryGood},
            new ScoreResume() { Rank = ValidationRank.Perfect},
            new ScoreResume() { Rank = ValidationRank.Null},
        };
        _missedNotes = 0;

    }

    public void ScoreUp(InGameNote note)
    {
        if (note.CurrentRank == ValidationRank.Error) return;

        _currentResumeScore.First(i => i.Rank == note.CurrentRank).CurrentCount++;

        _currentScore += _scoreConf.First(i => i.Rank == note.CurrentRank).Score * _combo.CurrentCoef.Coef;
        _text.text = _currentScore.ToString();
        OnScoreUp?.Invoke(note);
        OnNoteAccepted?.Invoke();
    }

    public void NoteMissed(InGameNote ign)
    {
        _missedNotes++;
        OnScoreUp?.Invoke(ign);
        OnNoteMissed?.Invoke();
    }

}
