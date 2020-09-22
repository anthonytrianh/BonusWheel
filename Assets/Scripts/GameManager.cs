using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Properties
    [HideInInspector] public Wheel wheel;
    [HideInInspector] public Results results;
    #endregion

    #region Mono
    // Start is called before the first frame update
    void Start()
    {
        wheel = GetComponentInChildren<Wheel>();
        results = GetComponentInChildren<Results>();
    }
    #endregion
}
