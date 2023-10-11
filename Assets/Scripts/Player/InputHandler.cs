using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;
using UnityEngine;
using SimplePlatformer.Player;

namespace SimplePlatformer.Assets.Scripts.Player
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputHandler : MonoBehaviour
    {
        private PlayerInput _playerInput;
        private PlayerController _playerController;

        private string actionMapPlayerControls = "PlayerControlls";
        private string actionMapMenuControls = "UI";

        public InputHandler()
        {
            _playerInput = GetComponent<PlayerInput>();
        }

        //INPUT SYSTEM ACTION METHODS --------------

        //This is called from PlayerInput; when a joystick or arrow keys has been pushed.
        //It stores the input Vector as a Vector3 to then be used by the smoothing function.


        public void OnMovement(InputAction.CallbackContext value)
        {
            Vector2 inputMovement = value.ReadValue<Vector2>();
            rawInputMovement = new Vector2(inputMovement.x, rb2d.velocity.y);
        }

        //This is called from PlayerInput, when a button has been pushed, that corresponds with the 'Attack' action
        public void OnAttack(InputAction.CallbackContext value)
        {
            if (value.started)
            {
                //playerCombatBehaviour.Attack();
            }
        }

        //This has a hold Interaction. When the hold ends, starts falling
        public void OnJump(InputAction.CallbackContext value)
        {
            if (value.started)
            {
                //playerMovementBehaviour.StartJumping();
            }
            if (value.performed || value.canceled)
            {
                //playerMovementBehaviour.CancelJumping();
            }

        }

        public void OnInteract(InputAction.CallbackContext value)
        {
            if (value.started)
            {
                playerInteractableBehaviour.Interact();
            }
        }

        public void OnBowAttack(InputAction.CallbackContext value)
        {
            if (value.started)
            {
                //playerCombatBehaviour.BowAttack();
            }
        }


        //This is called from Player Input, when a button has been pushed, that correspons with the 'TogglePause' action
        public void OnTogglePause(InputAction.CallbackContext value)
        {
            if (value.started)
            {
                GameManager.GetInstance().TogglePauseState();
            }
        }

        //INPUT SYSTEM AUTOMATIC CALLBACKS --------------

        //This is automatically called from PlayerInput, when the input device has changed
        //(IE: Keyboard -> Xbox Controller)
        public void OnControlsChanged()
        {

            if (_playerInput.currentControlScheme != currentControlScheme)
            {

                currentControlScheme = _playerInput.currentControlScheme;

                //playerVisualsBehaviour.UpdatePlayerVisuals();
                RemoveAllBindingOverrides();
            }
        }

        //This is automatically called from PlayerInput, when the input device has been disconnected and can not be identified
        //IE: Device unplugged or has run out of batteries

        public void OnDeviceLost()
        {
            currentControlScheme = _playerInput.currentControlScheme;
            //playerVisualsBehaviour.SetDisconnectedDeviceVisuals();
        }


        public void OnDeviceRegained()
        {
            StartCoroutine(WaitForDeviceToBeRegained());
        }

        IEnumerator WaitForDeviceToBeRegained()
        {
            yield return new WaitForSeconds(0.1f);
            //playerVisualsBehaviour.UpdatePlayerVisuals();
        }

        void RemoveAllBindingOverrides()
        {
            InputActionRebindingExtensions.RemoveAllBindingOverrides(_playerInput.currentActionMap);
        }

        //Switching Action Maps ----

        public void EnableGameplayControls()
        {
            //_playerInput.SwitchCurrentActionMap(actionMapPlayerControls);
        }

        public void EnablePauseMenuControls()
        {
            _playerInput.SwitchCurrentActionMap(actionMapMenuControls);
        }
    }
}
