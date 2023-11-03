using SimplePlatformer.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class to handle Android buttons because the On-Screen Buttons don't handle Unity Events from the Editor
/// </summary>
public class AndroidController : MonoBehaviour
{
    [SerializeField] private PlayerActions inputActions;
    [SerializeField] private PlayerController playerController;
    // Start is called before the first frame update
    void Awake()
    {
        inputActions = new PlayerActions();
    }

    private void OnEnable()
    {
        

    }

    private void OnDisable()
    {
        
    }
}
