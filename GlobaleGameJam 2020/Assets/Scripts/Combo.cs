using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static ValidationNote;

public class Combo : MonoBehaviour
{
    #region InternalTypes
    [Serializable] class ComboReceiver
    {
        public ValidationRank Rank;
        public int Score;
    }

    [Serializable] public class ComboLevel
    {
        public int Coef;
        public int MaxScore;
    }

    #endregion

    [SerializeField, BoxGroup("Managers"), Required] ScoreManager _scoreManager;
    [SerializeField, BoxGroup("Managers"), Required] DeadZone _deadZone;

    [SerializeField, BoxGroup("Stat Conf")] ComboLevel[] _comboLevels;
    [SerializeField, BoxGroup("Stat Conf")] ComboReceiver[] _comboReceiver;

    [SerializeField, BoxGroup("UI"), Required] Text _comboText;
    [SerializeField, BoxGroup("UI"), Required] Slider _comboSlider;

    int _currentIndex;
    int _currentValue;

    public ComboLevel CurrentCoef => _comboLevels[_currentIndex];

    private void Awake()
    {
        _scoreManager.OnScoreUp += ApplyNote;
        _deadZone.OnMissedNote += MissedNote;

        Initialization();
    }

    public void Initialization()
    {
        _currentIndex = 0;
        _currentValue = 0;
        UpdateUI();
    }

    void UpdateUI()
    {
        _comboSlider.minValue = 0;
        _comboSlider.maxValue = CurrentCoef.MaxScore;
        _comboSlider.value = _currentValue;

        _comboText.text = $"x{CurrentCoef.Coef}";
    }

    void MissedNote(InGameNote ign)
    {
        ApplyNote(ign, true);
    }

    void ApplyNote(InGameNote ign) => ApplyNote(ign, false);
    void ApplyNote(InGameNote ign, bool error = false)
    {
        _currentValue += !error ? 
            _comboReceiver.FirstOrDefault(i => i.Rank == ign.CurrentRank)?.Score ?? 0 : 
            _comboReceiver.First(i=>i.Rank== ValidationRank.Error).Score;

        if(_currentValue > CurrentCoef.MaxScore)
        {
            _currentValue -= CurrentCoef.MaxScore;
            _currentIndex = Mathf.Min(_currentIndex+1, _comboLevels.Length-1);

        }
        else if(_currentValue < 0)
        {
            if (_currentIndex == 0) { _currentValue = 0; }
            else
            {
                _currentIndex--;
                _currentValue = CurrentCoef.MaxScore + _currentValue;
            }
        }

        UpdateUI();
    }



}
