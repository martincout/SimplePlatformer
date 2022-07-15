using SimplePlatformer.Assets.Scripts.Player.States;
using SimplePlatformer.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputProvider : MonoBehaviour
{
    [SerializeField] private PlayerActions inputActions;
    [SerializeField] private PlayerStateMachine playerController;
    // Start is called before the first frame update
    void Awake()
    {
        inputActions = new PlayerActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.PlayerControlls.Movement.started += _ctx => playerController.OnMovement(_ctx);
        inputActions.PlayerControlls.Movement.performed += _ctx => playerController.OnMovement(_ctx);
        inputActions.PlayerControlls.Movement.canceled += _ctx => playerController.OnMovement(_ctx);
        inputActions.PlayerControlls.Jump.started += _ctx => playerController.OnJump(_ctx);
        inputActions.PlayerControlls.Jump.performed += _ctx => playerController.OnJump(_ctx);
        inputActions.PlayerControlls.Jump.canceled += _ctx => playerController.OnJump(_ctx);
        inputActions.PlayerControlls.Attack.started += _ctx => playerController.OnAttack(_ctx);
        inputActions.PlayerControlls.Bow.started += _ctx => playerController.OnBowAttack(_ctx);
        inputActions.PlayerControlls.Interact.started += _ctx => playerController.OnInteract(_ctx);
        inputActions.PlayerControlls.Pause.started += _ctx => playerController.OnTogglePause(_ctx);
        inputActions.PlayerControlls.Pause.performed += _ctx => playerController.OnTogglePause(_ctx);
        inputActions.PlayerControlls.Pause.canceled += _ctx => playerController.OnTogglePause(_ctx);

    }

    private void OnDisable()
    {
        inputActions.Disable();
        inputActions.PlayerControlls.Movement.started -= _ctx => playerController.OnMovement(_ctx);
        inputActions.PlayerControlls.Movement.performed -= _ctx => playerController.OnMovement(_ctx);
        inputActions.PlayerControlls.Movement.canceled -= _ctx => playerController.OnMovement(_ctx);
        inputActions.PlayerControlls.Jump.started -= _ctx => playerController.OnJump(_ctx);
        inputActions.PlayerControlls.Jump.performed -= _ctx => playerController.OnJump(_ctx);
        inputActions.PlayerControlls.Jump.canceled -= _ctx => playerController.OnJump(_ctx);
        inputActions.PlayerControlls.Attack.started -= _ctx => playerController.OnAttack(_ctx);
        inputActions.PlayerControlls.Bow.started -= _ctx => playerController.OnBowAttack(_ctx);
        inputActions.PlayerControlls.Interact.started -= _ctx => playerController.OnInteract(_ctx);
        inputActions.PlayerControlls.Pause.started -= _ctx => playerController.OnTogglePause(_ctx);
        inputActions.PlayerControlls.Pause.performed -= _ctx => playerController.OnTogglePause(_ctx);
        inputActions.PlayerControlls.Pause.canceled -= _ctx => playerController.OnTogglePause(_ctx);
    }
}
