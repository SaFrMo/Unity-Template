using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    // singleton
    public static GameManager current;
    private void Awake()
    {
        current = this;
    }
}
