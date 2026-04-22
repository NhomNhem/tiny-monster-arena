using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TinyMonsterArena.Infrastructure.Input {
    public class InputReader : MonoBehaviour {
        private PlayerControls_InputSystem_Actions _input;
        
        public Vector2 Move { get; private set; }
        public bool Attack { get; private set; }

        private void Awake() {
            _input = new PlayerControls_InputSystem_Actions();
        }

        private void OnEnable() {
            _input.Enable();
            
            _input.Player.Move.performed += ctx => Move = ctx.ReadValue<Vector2>();
            _input.Player.Move.canceled += ctx => Move = Vector2.zero;

            _input.Player.Attack.performed += ctx => Attack = true;
            _input.Player.Attack.canceled += ctx => Attack = false;
        }


        private void OnDisable() => _input.Disable();
    }
}