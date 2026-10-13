using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponGameRestApi.Controllers;
using WeaponGameRestApi.Models;

namespace WeaponGameRestApi.Core
{

    public abstract class Enemy : IDamageable, IEnemy, IEntity//, IMovement
    {

        public event Action<Enemy, int> ?OnDamaged; 
        public event Action<Enemy> ?OnDied;        


        public event Action<Enemy, int> ?OnAttacked;             
        public event Action<Enemy, int> ?OnStunnedPlayer;        
        public event Action<Enemy, int, int> ?OnMultiShotAttack;

        protected void InvokeOnDamaged(int damage) =>OnDamaged?.Invoke(this, damage);
        protected void InvokeOnDied()=>OnDied?.Invoke(this);
        

        protected void InvokeOnAttacked(int damage)=> OnAttacked?.Invoke(this, damage);
      
        protected void InvokeOnStunnedPlayer(int duration)=>OnStunnedPlayer?.Invoke(this, duration);
        protected void InvokeOnMultiShotAttack(int damagePerShot, int numberOfShots)=> OnMultiShotAttack?.Invoke(this, damagePerShot, numberOfShots);
     
        public string ?Name { get; protected set; }
        public int Health { get; protected set; }
        public bool IsDead => Health <= 0;
        public int Damage { get; protected set; }
        public string Id { get; set; }=  Guid.NewGuid().ToString();

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


    public interface IEnemyFactory
    {
        Enemy CreateEnemy(EnemyDto dto);
    }
    public class EnemyFactory : IEnemyFactory
    {
        private  readonly Dictionary<string, Func<EnemyDto, Enemy>> _enemyRegistry
            = new Dictionary<string, Func<EnemyDto, Enemy>>(StringComparer.OrdinalIgnoreCase);

        public EnemyFactory()
        {
            _enemyRegistry = new Dictionary<string, Func<EnemyDto, Enemy>>(StringComparer.OrdinalIgnoreCase);
            RegisterDefaultEnemies();
        }
        private void RegisterDefaultEnemies()
        {
            RegisterNewEnemy("tank", (dto) => new TankEnemy(dto.Health, dto.Damage));
            RegisterNewEnemy("boss", (dto) => new BossEnemy(dto.Health, dto.Damage));
            RegisterNewEnemy("soldier", (dto) => new SoldierEnemy(dto.Health, dto.Damage));
        }

        
        public void RegisterNewEnemy(string type, Func<EnemyDto, Enemy> creatorMethod)
        {
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("Enemy type cannot be empty.");

            _enemyRegistry[type] = creatorMethod;
        }

        
        public  Enemy CreateEnemy(EnemyDto dto)
        {
           

            if (_enemyRegistry.TryGetValue(dto.Type ?? string.Empty, out var creatorMethod))
            {
                return creatorMethod(dto); 
            }

            throw new ArgumentException($"Invalid enemy type: '{dto.Type}'.");
        }
    }
}



