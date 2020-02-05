using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ValidationNote;

public class NoteValidationToAnimator : MonoBehaviour
{
    #region InternalTypes
    [Serializable] class InGameAnimatorConf
    {
        public ValidationRank Rank;
        public string TriggerName = "";
    }
    #endregion

    [SerializeField, BoxGroup("Internal Ref"), Required] Animator _animator;
    [SerializeField, BoxGroup("Internal Ref"), Required] InGameNote _inGameNote;

    [SerializeField, BoxGroup("Animator Conf")] InGameAnimatorConf[] _triggers;

    private void Awake()
    {
        _inGameNote.OnValidate += UpdateAnimator;

    }

    void UpdateAnimator(InGameNote ign)
    {
        var correctTrigger = _triggers.FirstOrDefault(i => i.Rank == ign.CurrentRank);
        _animator.SetTrigger(correctTrigger.TriggerName);
    }


}
