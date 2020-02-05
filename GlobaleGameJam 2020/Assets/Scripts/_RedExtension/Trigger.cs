using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger {

    bool _value;
    public Trigger()
    {
        _value = false;
    }
    public void Activate() => _value = true;

    public bool IsActivated()
    {
        if (!_value) return false;

        _value = false;
        return true;
    }

}
