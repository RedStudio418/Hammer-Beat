using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static Partition;

public class MusicInputManager : MonoBehaviour
{
    #region InternalTypes
    [Serializable] public class MusicLine
    {
        [SerializeField] Transform _line;
        [SerializeField] MusicNote _musicNote;
        [SerializeField] VirtualNote _note;

        public Transform Line => _line;
        public MusicNote MusicNote => _musicNote;
        public VirtualNote Note => _note;
    }
    #endregion

    [SerializeField, BoxGroup("Input Conf")] bool _useAxis;

    [SerializeField, BoxGroup("MusicLines conf")] MusicLine[] _musicLines;

    [SerializeField, BoxGroup("Validation")] Transform _repairLine;
    [SerializeField, BoxGroup("Validation")] float _validationThreshold;
    [SerializeField, BoxGroup("Validation")] float _disableThreshold;

    bool[] _validated;

    public MusicLine[] MusicLines => _musicLines;

    public event Action OnUp;
    public event Action OnDown;
    public event Action OnRight;
    public event Action OnLeft;

    public UnityEvent OnHammerHit;

    public UnityEvent OnUpUnity;
    public UnityEvent OnDownUnity;
    public UnityEvent OnRightUnity;
    public UnityEvent OnLeftUnity;

    private void Awake()
    {
        _validated = new bool[] { false, false, false, false };
        OnUp += () => _musicLines.First(i => i.Note == VirtualNote.Up).Line.GetComponentsInChildren<InGameNote>().FirstOrDefault(i=>i.Activated)?.ValidateNote();
        OnDown += () => _musicLines.First(i => i.Note == VirtualNote.Down).Line.GetComponentsInChildren<InGameNote>().FirstOrDefault(i => i.Activated)?.ValidateNote();
        OnRight += () => _musicLines.First(i => i.Note == VirtualNote.Right).Line.GetComponentsInChildren<InGameNote>().FirstOrDefault(i => i.Activated)?.ValidateNote();
        OnLeft += () => _musicLines.First(i => i.Note == VirtualNote.Left).Line.GetComponentsInChildren<InGameNote>().FirstOrDefault(i => i.Activated)?.ValidateNote();
    }

    float _lastX;
    float _lastY;

    public void InputPass()
    {

        // Get Input
        float y=0;
        float x=0;

        if(_useAxis)
        {
            y = Input.GetAxisRaw("Horizontal");
            x = Input.GetAxisRaw("Vertical");
        }
        else
        {
            if(Input.GetButtonDown("Fire_A")) { x--; }
            if(Input.GetButtonDown("Fire_B")) { y++; }
            if(Input.GetButtonDown("Fire_X")) { y--; }
            if(Input.GetButtonDown("Fire_Y")) { x++; }
        }

        if(_lastX != x || _lastY != y)
        {
            // Debug.Log($"x : {x} - y : {y}");
            _lastY = y;
            _lastX = x;
        }

        // Check Up
        if (!_validated[(int)VirtualNote.Up] && x > _validationThreshold)
        {
            _validated[(int)VirtualNote.Up] = true;
            OnUp?.Invoke();
            OnHammerHit?.Invoke();
            OnUpUnity?.Invoke();
            // Debug.Log("Up");
        }
        if (!_validated[(int)VirtualNote.Right] && y > _validationThreshold)
        {
            _validated[(int)VirtualNote.Right] = true;
            OnRight?.Invoke();
            OnHammerHit?.Invoke();
            OnRightUnity?.Invoke();
            // Debug.Log("Right");
        }
        if (!_validated[(int)VirtualNote.Down] && x < _validationThreshold*-1)
        {
            _validated[(int)VirtualNote.Down] = true;
            OnDown?.Invoke();
            OnHammerHit?.Invoke();
            OnDownUnity?.Invoke();
            // Debug.Log("Down");
        }
        if (!_validated[(int)VirtualNote.Left] && y < _validationThreshold*-1)
        {
            _validated[(int)VirtualNote.Left] = true;
            OnLeft?.Invoke();
            OnHammerHit?.Invoke();
            OnLeftUnity?.Invoke();
            // Debug.Log("Left");
        }

        // Check disabled
        if ((_validated[(int)VirtualNote.Up] || _validated[(int)VirtualNote.Down]) && Mathf.Abs(x) <= _disableThreshold)
        {
            _validated[(int)VirtualNote.Up] = false;
            _validated[(int)VirtualNote.Down] = false;
        }
        if ((_validated[(int)VirtualNote.Right] || _validated[(int)VirtualNote.Left]) && Mathf.Abs(y) <= _disableThreshold)
        {
            _validated[(int)VirtualNote.Right] = false;
            _validated[(int)VirtualNote.Left] = false;
        }

    }


}
