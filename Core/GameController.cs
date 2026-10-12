using System;
using System.Collections.Generic;
using System.Linq;




namespace WeaponGameRestApi.Core
{
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
    public class Player : IDamageable//, IMovement, IControllable
    {
        public int Health { get; private set; }
        public bool IsDead => Health <= 0;

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
    internal class GameController
    {
        private Player _player;
        private List<Weapon> _weapons;
        private Weapon _playerWeapon;
        private List<Enemy> _enemies;
        private IGameLogger _logger;

        public GameController(Player player, List<Weapon> weapons, List<Enemy> enemies, IGameLogger logger)
        {
            _player = player;
            _weapons = weapons;
            _enemies = enemies;
            _logger = logger;
            foreach (var weapon in _weapons)
            {
                weapon.OnMissed += (w) => _logger.Log($"Weapon {w.Name} missed the target.");
                weapon.OnFired += HandleWeaponFired;
                //weapon.OnReloaded += HandleWeaponReloaded;
                weapon.OnOutOfAmmo += HandleWeaponReloaded;
            }
            _playerWeapon = _weapons.FirstOrDefault();
            foreach (var enemy in _enemies)
            {
                enemy.OnAttacked += HandleEnemyAttacked;
                enemy.OnStunnedPlayer += HandleEnemyStunnedPlayer;
                enemy.OnMultiShotAttack += HandleEnemyMultiShotAttack;
            }
        }
        public void StartGame()
        {
            Console.WriteLine("Game started!");
            while (!_player.IsDead && _enemies.Any(T => !T.IsDead))
            {
                _playerWeapon.Fire();
                foreach (var enemy in _enemies)
                {
                    if (!enemy.IsDead)
                    {
                        enemy.Attack();
                    }
                }
                Console.WriteLine("\n/////////////////////////////////////////////////////");
                Console.WriteLine("Press Enter to continue to the next round...");
                Console.ReadLine();
                Console.WriteLine($"Player Health: {_player.Health}");
                foreach (var enemy in _enemies)
                {
                    Console.WriteLine($"Enemy {enemy.Name} Health: {enemy.Health}");
                }

                Console.WriteLine("\n/////////////////////////////////////////////////////");
            }
            Console.WriteLine(_player.IsDead ? "Player is dead!" : "All enemies are dead!");
        }
        private void HandleWeaponFired(Weapon weapon, int damage)
        {
            var target = _enemies.FirstOrDefault(e => !e.IsDead);
            if (target != null)
            {
                Console.WriteLine($"Fired with {weapon.Name} for {damage} damage at {target.Name}!\n");
                target.TakeDamage(damage);
            }
        }
    private void HandleWeaponReloaded(Weapon weapon)
        {
            Console.WriteLine($"{weapon.Name} reloaded.\n");
        }
        private void HandleWeaponSwitched(Weapon weapon)
        {
            Console.WriteLine($"{weapon.Name} is out of ammo. Switching to next weapon.");
            SwitchToNextWeapon();
        }
        private void HandleEnemyAttacked(Enemy enemy, int damage)
        {
            
            _player.TakeDamage(damage);
            Console.WriteLine($"{enemy.Name} attacked the player for {damage} damage!\n");
        }
        private void HandleEnemyStunnedPlayer(Enemy enemy, int duration)
        {
            Console.WriteLine($"Player stunned for {duration} seconds by {enemy.Name}!\n");
        }
        private void HandleEnemyMultiShotAttack(Enemy enemy, int damagePerShot, int numberOfShots)
        {
            for (int i = 0; i < numberOfShots; i++)
            {
                _player.TakeDamage(damagePerShot);
                Console.WriteLine($"{enemy.Name} attacked the player for {damagePerShot} damage!\n");
            }
        }
        private void SwitchToNextWeapon()
        {
            int currentIndex = _weapons.IndexOf(_playerWeapon);
            int nextIndex = (currentIndex + 1) % _weapons.Count;
            _playerWeapon = _weapons[nextIndex];
            Console.WriteLine($"Switched to {_playerWeapon.Name}.");
        }
    }
}

