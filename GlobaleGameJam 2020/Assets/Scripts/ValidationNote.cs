using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValidationNote : MonoBehaviour
{
    public enum ValidationRank { Null = -1, Perfect = 0, VeryGood = 1, Good = 2, Boarf = 3, Error= 4}

    [SerializeField] ValidationRank _rank;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out InGameNote note))
        {
            note.UpdateValidationState(_rank);
        }

    }


}
