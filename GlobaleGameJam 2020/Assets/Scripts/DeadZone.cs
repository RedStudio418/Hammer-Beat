using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    public event Action<InGameNote> OnMissedNote;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent( out InGameNote note))
        {
            note.IsInDeadZone();
            OnMissedNote?.Invoke(note);
        }
    }


}
