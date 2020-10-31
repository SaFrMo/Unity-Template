using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    // singleton
    public static UIManager current;
    private void Awake()
    {
        current = this;
    }
}
