using TinyMonsterArena.Domain.Entities;
using UnityEngine;

namespace TinyMonsterArena.Presentation.Player {
    public class PlayerView : MonoBehaviour {
        private Animator _animator;
        
        private static readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");
        private static readonly int AttackHash = Animator.StringToHash("Attack");
        private static readonly int DieHash = Animator.StringToHash("Die");
        
        private void Awake() {
            _animator = GetComponent<Animator>();
        }
        
        public void PlayIdle() => _animator.SetFloat(MoveSpeedHash, 0f);
        public void PlayRun() => _animator.SetFloat(MoveSpeedHash, 1f);
        
        public void PlayerDeath() => _animator.SetTrigger(DieHash);
        
        public void PlayAttack() => _animator.SetTrigger(AttackHash);
    }
}