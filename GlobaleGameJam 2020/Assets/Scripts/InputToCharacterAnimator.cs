using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputToCharacterAnimator : MonoBehaviour
{
    [SerializeField, BoxGroup("External Ref"), Required] MusicInputManager _inputToCharacter;
    [SerializeField, BoxGroup("Internal Ref"), Required] Animator _animator;
    [SerializeField, BoxGroup("Animator Conf")] string _hitTrigger = "Hit";

    private void Awake()
    {
        _inputToCharacter.OnUp += FireHit;
        _inputToCharacter.OnDown += FireHit;
        _inputToCharacter.OnRight += FireHit;
        _inputToCharacter.OnLeft += FireHit;
    }

    void FireHit() => _animator.SetTrigger(_hitTrigger);





}
