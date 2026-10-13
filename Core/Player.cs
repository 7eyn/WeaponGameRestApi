using WeaponGameRestApi.Controllers;

namespace WeaponGameRestApi.Core
{
    public interface IEntity
    {
        string Id { get; set; }
    }
    public interface IDamageable
    {
        void TakeDamage(int damage);
        bool IsDead { get; }

    }
    public interface IEnemy
    {
        void Attack();
    }
    public interface IMovement
    {
        void MoveLeft();
        void MoveRight();
        void MoveFront();
        void MoveBack();
    }
    public interface IControllable
    {
        void Jump();
        void Crouch();
        void Dash();
    }
    public class Player : IDamageable, IEntity//, IMovement, IControllable
    {
        public int Health { get; private set; }
        public bool IsDead => Health <= 0;
        public string ?Id { get; set; }

        public Player(int health)
        {
            Health = health;
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            Console.WriteLine($"Player took {damage} damage. Remaining HP: {Math.Max(0, Health)}");
        }
    }
}
