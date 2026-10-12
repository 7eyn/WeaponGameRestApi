using Microsoft.AspNetCore.SignalR;
using WeaponGameRestApi.Controllers;
using WeaponGameRestApi.Core;
using WeaponGameRestApi.Hubs;
namespace WeaponGameRestApi.Services
{
    public class GameEngineService
    {
        private readonly IHubContext<GameHub> _hubContext;
        private readonly IGameLogger _gameLogger;
        public Player CurrentPlayer { get; private set; }
        public List<Enemy> Enemies { get; private set; } = new List<Enemy>();
        public Weapon ?CurrentWeapon { get; private set; }
        public GameEngineService(IHubContext<GameHub> hubContext, IGameLogger gameLogger)
        {
            _hubContext = hubContext;
            _gameLogger = gameLogger;

            CurrentPlayer = new Player(1500);
        }
        public void SetWeapon(Weapon weapon)
        {
            CurrentWeapon = weapon;

            
            CurrentWeapon.OnFired += async (w, damage) =>
            { 
                var target = Enemies.FirstOrDefault(e => !e.IsDead);

                await _hubContext.Clients.All.SendAsync("WeaponEvent", $"{w.Name} fired in {target?.Name ?? "unknown target"}, damage: {damage}");
                if (target != null)
                {
                    target.TakeDamage(damage);
                }
            };

            CurrentWeapon.OnMissed += async (w) =>
            {
                await _hubContext.Clients.All.SendAsync("WeaponEvent", $"{w.Name} missed!");
            };

            CurrentWeapon.OnOutOfAmmo += async (w) =>
            {
                await _hubContext.Clients.All.SendAsync("WeaponEvent", $"{w.Name} — is out of ammo. Reloading...");
                w.Reload();
            };
        }
        public void AddEnemy(Enemy enemy)
        {

            enemy.OnDamaged += async (e, damage) =>
            {
                await _hubContext.Clients.All.SendAsync("EnemyStatus", new { name = e.Name, eventType = "Damaged", takenDamage = damage, hpLeft = e.Health });
            };

            enemy.OnDied += async (e) =>
            {
                await _hubContext.Clients.All.SendAsync("EnemyStatus", new { name = e.Name, eventType = "Died" });
                Enemies.Remove(e); 
            };

            enemy.OnAttacked += async (e, damage) =>
            {
                CurrentPlayer.TakeDamage(damage);
                await _hubContext.Clients.All.SendAsync("PlayerStatus", new { eventType = "AttackedBy", enemy = e.Name, hpLeft = CurrentPlayer.Health });
            };
            Enemies.Add(enemy);
        }

        public void PlayerShootAction()
        {
            if (CurrentWeapon == null || !Enemies.Any()) return;

            CurrentWeapon.Fire();
        }

        public void EnemyAttackAction(Enemy enemy)
        {
            if (!enemy.IsDead)
            {
                enemy.Attack();
            }
        }

    }
}
