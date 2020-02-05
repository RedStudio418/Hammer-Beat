using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    [SerializeField] Button[] _buttons;
    [SerializeField, BoxGroup("Animator Conf")] string _selectedBoolParam = "Selected";

    public event Action<Partition> OnLaunchPartition;

    int _focusindex;
    bool changed;

    private void OnEnable()
    {
        Focus(0);
    }

    void Focus(int newIdx)
    {
        _focusindex = newIdx;
        foreach(var el in _buttons.Where(i=> i != _buttons[_focusindex])) el.GetComponent<Animator>().SetBool(_selectedBoolParam, false);
        _buttons[_focusindex].GetComponent<Animator>().SetBool(_selectedBoolParam, true);
    }

    private void Update()
    {
        if(!changed)
        {
            if(Input.GetAxisRaw("Horizontal") >= 1  || Input.GetAxisRaw("Horizontal") <= -1)
            {
                _focusindex = Input.GetAxisRaw("Horizontal") > 0 ? _focusindex+1 : _focusindex-1;
                changed = true;
            }

            if (_focusindex >= _buttons.Length) _focusindex = _buttons.Length - 1;
            if (_focusindex < 0) _focusindex = 0;
            Focus(_focusindex);
        }
        else
        {
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) < 0.2f) changed = false;
        }

        if(Input.GetButtonDown("Fire_A"))
        {
            _buttons[_focusindex].onClick.Invoke();
        }

    }

    public void LaunchMusic(Partition p)
    {
        OnLaunchPartition?.Invoke(p);
    }



}
