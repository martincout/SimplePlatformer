using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AndroidButtons : MonoBehaviour
{
    [SerializeField] private PlayerActions inputActions;
    [SerializeField] private GameObject containerButtons;
    private bool active;
    // Start is called before the first frame update
    void Start()
    {
        active = containerButtons.activeSelf;
        inputActions.Enable();
        inputActions.PlayerControlls.Pause.started += _ctx => ToggleAndroidButtons(_ctx);
    }

    private void ToggleAndroidButtons(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            active = !active;
            containerButtons.SetActive(active);
        }
    }

}
