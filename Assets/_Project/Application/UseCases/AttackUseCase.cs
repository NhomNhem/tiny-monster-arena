using TinyMonsterArena.Domain.Entities;

namespace TinyMonsterArena.Application.UseCases {
    public class AttackUseCase {
        public void Execute(PlayerEntity attacker, PlayerEntity target, int damage) {
            target.TakeDamage(damage);
        }
    }
}