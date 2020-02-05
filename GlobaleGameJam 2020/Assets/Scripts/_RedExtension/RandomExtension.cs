using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomExtension : MonoBehaviour {

    public static bool Check(float winProba) => UnityEngine.Random.Range(0f, 100f) < winProba;


}
