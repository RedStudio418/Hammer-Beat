using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ValidationNote;

public class InGameNote : MonoBehaviour
{
    [SerializeField, BoxGroup("UI")] Sprite _brokenState;
    [SerializeField, BoxGroup("UI")] Sprite _fixedState;
    [SerializeField, BoxGroup("UI"), Required] Image _spriteComp;

    [SerializeField, BoxGroup("Physics"), Required] BoxCollider _boxCollider;

    public ValidationRank CurrentRank { get; private set; }
    [ShowNativeProperty] string Editor_CurrentRank => Enum.GetName(typeof(ValidationRank), CurrentRank);
    public bool Activated { get; private set; }

    MusicBoard _masterMusicBoard;
    Vector2 _gameDirection = Vector2.left;

    SpecialCountDown _timer;
    Vector3 _startPosition;
    Vector3 _destinationPosition;
    Vector3 _averageDirection;

    public event Action<InGameNote> OnDeadNote;
    public event Action<InGameNote> OnValidate;
    public event Action<InGameNote> OnDestroyedNote;

    internal InGameNote Initialize(MusicBoard musicBoard)
    {
        _masterMusicBoard = musicBoard;
        Activated = true;
        CurrentRank = ValidationRank.Null;
        _spriteComp.sprite = _brokenState;

        _startPosition = transform.position;
        _timer = new SpecialCountDown(_masterMusicBoard.MovementDuration);
        _destinationPosition = new Vector3(_masterMusicBoard.ValidationXPosition, _startPosition.y, _startPosition.z);
        return this;
    }

    internal void IsInDeadZone()
    {
        Activated = false;
        OnDeadNote?.Invoke(this);
        ClearNote();
    }

    internal void UpdateValidationState(ValidationNote.ValidationRank rank) => CurrentRank = rank;

    internal void UpdateByMusicBoard()
    {
        if (!_timer.isDone)
        {
            var currentPos = transform.position;
            transform.position = Vector3.Lerp(_startPosition, _destinationPosition, _timer.Progress);
            var nextPos = transform.position;
            if((nextPos-currentPos).magnitude > _averageDirection.magnitude) _averageDirection = nextPos - currentPos;
        }
        else
        {
            transform.Translate(_averageDirection*Time.deltaTime*50);
        }
    }

    internal void ValidateNote()
    {
        // The note is not eligible yet
        if (CurrentRank == ValidationRank.Null) return;
        Activated = false;
        _spriteComp.sprite = _fixedState;
        OnValidate?.Invoke(this);
        _boxCollider.enabled = false;
        StartCoroutine(CoroutineExtension.WaitSecondsAnd(1f, () =>ClearNote()));
    }

    void ClearNote()
    {
        OnDestroyedNote?.Invoke(this);
        Destroy(gameObject);
    }

}
