namespace TinyMonsterArena.Domain.Entities {
    public class PlayerEntity {
        public int Health { get; private set; }
        
        public bool IsDead => Health <= 0;
        
        public void TakeDamage(int damage) {
            if (IsDead) return;
            Health -= damage;
            if (Health <= 0) Health = 0;
        }
    }
}