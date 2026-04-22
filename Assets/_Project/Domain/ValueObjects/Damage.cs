namespace TinyMonsterArena.Domain.ValueObjects {
    public readonly struct Damage {
        public readonly int Value;
        public Damage(int value) {
            Value = value;
        }
    }
}