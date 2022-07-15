using SimplePlatformer.Assets.Scripts.Player.States;
using SimplePlatformer.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputProvider : MonoBehaviour
{
    [SerializeField] private PlayerActions inputActions;
    [SerializeField] public PlayerStateMachine PlayerStateMachine;
    // Start is called before the first frame update
    void Awake()
    {
        PlayerStateMachine = GetComponent<PlayerStateMachine>();
        inputActions = new PlayerActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.PlayerControlls.Movement.started += _ctx => PlayerStateMachine.OnMovement(_ctx);
        inputActions.PlayerControlls.Movement.performed += _ctx => PlayerStateMachine.OnMovement(_ctx);
        inputActions.PlayerControlls.Movement.canceled += _ctx => PlayerStateMachine.OnMovement(_ctx);
        inputActions.PlayerControlls.Jump.started += _ctx => PlayerStateMachine.OnJump(_ctx);
        inputActions.PlayerControlls.Jump.performed += _ctx => PlayerStateMachine.OnJump(_ctx);
        inputActions.PlayerControlls.Jump.canceled += _ctx => PlayerStateMachine.OnJump(_ctx);
        inputActions.PlayerControlls.Attack.started += _ctx => PlayerStateMachine.OnAttack(_ctx);
        inputActions.PlayerControlls.Bow.started += _ctx => PlayerStateMachine.OnBowAttack(_ctx);
        inputActions.PlayerControlls.Interact.started += _ctx => PlayerStateMachine.OnInteract(_ctx);
        inputActions.PlayerControlls.Pause.started += _ctx => PlayerStateMachine.OnTogglePause(_ctx);
        inputActions.PlayerControlls.Pause.performed += _ctx => PlayerStateMachine.OnTogglePause(_ctx);
        inputActions.PlayerControlls.Pause.canceled += _ctx => PlayerStateMachine.OnTogglePause(_ctx);

    }

    private void OnDisable()
    {
        inputActions.Disable();
        inputActions.PlayerControlls.Movement.started -= _ctx => PlayerStateMachine.OnMovement(_ctx);
        inputActions.PlayerControlls.Movement.performed -= _ctx => PlayerStateMachine.OnMovement(_ctx);
        inputActions.PlayerControlls.Movement.canceled -= _ctx => PlayerStateMachine.OnMovement(_ctx);
        inputActions.PlayerControlls.Jump.started -= _ctx => PlayerStateMachine.OnJump(_ctx);
        inputActions.PlayerControlls.Jump.performed -= _ctx => PlayerStateMachine.OnJump(_ctx);
        inputActions.PlayerControlls.Jump.canceled -= _ctx => PlayerStateMachine.OnJump(_ctx);
        inputActions.PlayerControlls.Attack.started -= _ctx => PlayerStateMachine.OnAttack(_ctx);
        inputActions.PlayerControlls.Bow.started -= _ctx => PlayerStateMachine.OnBowAttack(_ctx);
        inputActions.PlayerControlls.Interact.started -= _ctx => PlayerStateMachine.OnInteract(_ctx);
        inputActions.PlayerControlls.Pause.started -= _ctx => PlayerStateMachine.OnTogglePause(_ctx);
        inputActions.PlayerControlls.Pause.performed -= _ctx => PlayerStateMachine.OnTogglePause(_ctx);
        inputActions.PlayerControlls.Pause.canceled -= _ctx => PlayerStateMachine.OnTogglePause(_ctx);
    }
}
