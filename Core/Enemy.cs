using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponGameRestApi.Core;

namespace WeaponGameRestApi.Core
{

    public abstract class Enemy : IDamageable, IEnemy//, IMovement
    {

        public event Action<Enemy, int> OnDamaged; 
        public event Action<Enemy> OnDied;        


        public event Action<Enemy, int> OnAttacked;             
        public event Action<Enemy, int> OnStunnedPlayer;        
        public event Action<Enemy, int, int> OnMultiShotAttack;

        protected void InvokeOnDamaged(int damage) =>OnDamaged?.Invoke(this, damage);
        protected void InvokeOnDied()=>OnDied?.Invoke(this);
        

        protected void InvokeOnAttacked(int damage)=> OnAttacked?.Invoke(this, damage);
      
        protected void InvokeOnStunnedPlayer(int duration)=>OnStunnedPlayer?.Invoke(this, duration);
        protected void InvokeOnMultiShotAttack(int damagePerShot, int numberOfShots)=> OnMultiShotAttack?.Invoke(this, damagePerShot, numberOfShots);
     
        public string Name { get; protected set; }
        public int Health { get; protected set; }
        public bool IsDead => Health <= 0;
        public int Damage { get; protected set; }
        public Enemy(int health, int damage)
        {
            Health = health;
            Damage = damage;
        }
        public void TakeDamage(int damage)
        {
            if (damage < 0)
            {
                throw new ArgumentException("Damage cannot be negative.");
            }
            Health -= damage;
            if (Health < 0)
            {
                Health = 0;
                OnDied?.Invoke(this);
            }
            else
            {
                OnDamaged?.Invoke(this, damage);
            }
            }
        public void DisplayStatus()
        {
            Console.WriteLine($"Enemy: {Name} | Health: {Health}");
        }
        public abstract void Attack();
    }

    public class TankEnemy : Enemy
    {
        private static readonly Random _rnd = new Random();
        public TankEnemy(int health, int damage) : base(health, damage)
        {
            Name = "Armored Soldier";
        }
        public override void Attack()
        {
            int number = _rnd.Next(1, 4);
            switch (number)
            {
                case 1:
                    InvokeOnAttacked(Damage);
                    break;
                case 2:
                    InvokeOnAttacked(Damage * 2);
                    break;
                case 3:
                    InvokeOnStunnedPlayer(5);
                    break;
            }

        }

    }

    public class SoldierEnemy : Enemy
    {
        public SoldierEnemy(int health, int damage) : base(health, damage)
        {
            Name = "Soldier";
        }
        public override void Attack()
        {
            InvokeOnAttacked(Damage);
        }
    }

    public class BossEnemy : Enemy
    {
        private static readonly Random _rnd = new Random();
        public BossEnemy(int health, int damage) : base(health, damage)
        {
            Name = "Boss";
        }
        public override void Attack()
        {
            int number = _rnd.Next(1, 4);
            switch (number)
            {
                case 1:
                    InvokeOnMultiShotAttack(Damage / 2, 3);
                    break;
                case 2:
                    InvokeOnAttacked(Damage * 2);
                    break;
                case 3:
                    InvokeOnStunnedPlayer(10);
                    break;
            }
        }
    }
    public class EnemyFactory
    {
        private static readonly Dictionary<string, Func<int, int, Enemy>> _enemyRegistry
            = new Dictionary<string, Func<int, int, Enemy>>(StringComparer.OrdinalIgnoreCase);

        
        static EnemyFactory()
        {
            RegisterNewEnemy("tank", (health, damage) => new TankEnemy(health, damage));
            RegisterNewEnemy("boss", (health, damage) => new BossEnemy(health, damage));
            RegisterNewEnemy("soldier", (health, damage) => new SoldierEnemy(health, damage));
        }

        
        public static void RegisterNewEnemy(string type, Func<int, int, Enemy> creatorMethod)
        {
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("Enemy type cannot be empty.");

            _enemyRegistry[type] = creatorMethod;
        }

        
        public static Enemy CreateEnemy(string type, int health, int damage)
        {
          
            if (_enemyRegistry.TryGetValue(type, out var creatorMethod))
            {
                return creatorMethod(health, damage); 
            }

            throw new ArgumentException($"Invalid enemy type: '{type}'.");
        }
    }
}



