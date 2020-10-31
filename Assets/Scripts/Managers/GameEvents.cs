using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    // singleton
    public static GameEvents current;
    private void Awake()
    {
        current = this;
    }
}
