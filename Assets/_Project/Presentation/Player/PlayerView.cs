using TinyMonsterArena.Domain.Entities;
using UnityEngine;

namespace TinyMonsterArena.Presentation.Player {
    public class PlayerView : MonoBehaviour {
        private Animator _animator;
        
        private static readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");
        private static readonly int AttackHash = Animator.StringToHash("Attack");
        private static readonly int DieHash = Animator.StringToHash("Die");
        
        private void Awake() {
            _animator = GetComponentInChildren<Animator>(true);
        }
        
        public void SetLocomotion(bool isMoving) {
            if (_animator != null) {
                _animator.SetFloat(MoveSpeedHash, isMoving ? 1f : 0f);
            }
        }

        public void PlayIdle() {
            SetLocomotion(false);
        }

        public void PlayRun() {
            SetLocomotion(true);
        }
        
        public void PlayerDeath() {
            if (_animator != null) {
                _animator.SetTrigger(DieHash);
            }
        }
        
        public void TriggerAttack() {
            if (_animator != null) {
                _animator.SetTrigger(AttackHash);
            }
        }

        public void PlayAttack() {
            TriggerAttack();
        }
    }
}
