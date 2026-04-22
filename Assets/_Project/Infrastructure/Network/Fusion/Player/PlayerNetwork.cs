using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Addons.FSM;
using Fusion.Addons.KCC;
using TinyMonsterArena.Infrastructure.Network.Fusion.Input;
using TinyMonsterArena.Infrastructure.Network.Fusion.Player.States;
using TinyMonsterArena.Presentation.Player;
using UnityEngine;

namespace TinyMonsterArena.Infrastructure.Network.Fusion.Player {
    public class PlayerNetwork : NetworkBehaviour, IStateMachine {
        public KCC KCC;
        public PlayerView PlayerView;

        // Reference tới các State cụ thể để các State có thể chuyển đổi lẫn nhau
        public IdleState IdleState;
        public MoveState MoveState;
        public AttackState AttackState;

        public NetworkInputData Input { get; private set; }

        public string Name { get; }
        public IState ActiveState { get; }
        public IState PreviousState { get; }
        public IState[] States { get; }

        public void Initialize(StateMachineController controller, NetworkRunner runner) { }

        public override void FixedUpdateNetwork() {
            if (GetInput(out NetworkInputData input)) {
                Input = input;
            }
        }

        public void Deinitialize(bool hasState) { }
        public void SetDefaultState(int stateId) { }
        public void Reset() { }

        public bool TryActivateState(int stateId, bool allowReset = false) {
            return true;
        }

        public bool ForceActivateState(int stateId, bool allowReset = false) {
            return true;
        }

        public bool TryDeactivateState(int stateId) {
            return true;
        }

        public bool ForceDeactivateState(int stateId) {
            return true;
        }

        public bool TryToggleState(int stateId, bool value) {
            return true;
        }

        public void ForceToggleState(int stateId, bool value) { }
        public bool? EnableLogging { get; set; }
        public int WordCount { get; }
        public unsafe void Read(int* ptr) { }
        public unsafe void Write(int* ptr) { }
        public void Interpolate(InterpolationData interpolationData) { }

        public void CollectStateMachines(List<IStateMachine> stateMachines) {
            // Khởi tạo máy trạng thái với tên "Locomotion"
            var locomotionMachine = new StateMachine<StateBehaviour>("Locomotion", 
                IdleState, 
                MoveState, 
                AttackState
            );

            // Thiết lập trạng thái mặc định là Idle
            locomotionMachine.SetDefaultState(IdleState.StateId);
            
            stateMachines.Add(locomotionMachine);
        }
    }
}