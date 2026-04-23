using UnityEngine;
using UnityEngine.InputSystem;

namespace TinyMonsterArena.Infrastructure.Input {
    public class InputReader : MonoBehaviour {
        private PlayerControls_InputSystem_Actions _input;
        private bool _attackQueued;

        public Vector2 Move { get; private set; }

        private void Awake() {
            _input = new PlayerControls_InputSystem_Actions();
        }

        private void OnEnable() {
            _input.Enable();

            _input.Player.Move.performed += OnMovePerformed;
            _input.Player.Move.canceled += OnMoveCanceled;
            _input.Player.Attack.performed += OnAttackPerformed;
        }

        private void OnDisable() {
            _input.Player.Move.performed -= OnMovePerformed;
            _input.Player.Move.canceled -= OnMoveCanceled;
            _input.Player.Attack.performed -= OnAttackPerformed;
            _input.Disable();

            Move = Vector2.zero;
            _attackQueued = false;
        }

        public bool ConsumeAttack() {
            bool attackQueued = _attackQueued;
            _attackQueued = false;
            return attackQueued;
        }

        private void OnMovePerformed(InputAction.CallbackContext context) {
            Move = context.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext context) {
            Move = Vector2.zero;
        }

        private void OnAttackPerformed(InputAction.CallbackContext context) {
            _attackQueued = true;
        }
    }
}
