using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using WeaponGameRestApi.Core;
using WeaponGameRestApi.Models;
using WeaponGameRestApi.Repositories;
using WeaponGameRestApi.Services;




namespace WeaponGameRestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]


    public class GameController : ControllerBase
    {
        
        private readonly GameEngineService _gameEngineService;
        private readonly IRepository<Weapon> _weaponRepository;
        private readonly IRepository<Enemy> _enemyRepository;

        public GameController(
             GameEngineService gameEngineService,
             IRepository<Weapon> weaponRepository,
             IRepository<Enemy> enemyRepository)
        {
            _gameEngineService = gameEngineService;
            _weaponRepository = weaponRepository;
            _enemyRepository = enemyRepository;
        }
        [HttpPost("add-enemy/{id}")]
        public async Task<IActionResult> SpawnEnemy(string id)
        {
            var templateEnemy = await _enemyRepository.GetAsync(id);
            if (templateEnemy == null)
            {
                return NotFound(new { message = $"Enemy template with ID {id} not found." });
            }

            var gameEnemy = templateEnemy.Clone();
            _gameEngineService.AddEnemy(gameEnemy);
            return Ok(new { message = $"Enemy {gameEnemy.Name} spawned." });
        }
        [HttpPost("add-weapon/{id}")]
        public async Task<IActionResult> AddWeapon(string id)
        {
            var templateWeapon = await _weaponRepository.GetAsync(id);
            if (templateWeapon == null)
            {
                return NotFound(new { message = $"Weapon template with ID {id} not found." });
            }
            var gameWeapon = templateWeapon.Clone();
            _gameEngineService.AddWeapon(gameWeapon);
            if (_gameEngineService.CurrentWeapon == null)
            {
                _gameEngineService.SetWeapon(gameWeapon);
            }
            return Ok(new { message = $"Weapon {gameWeapon.Name} has been added to the game." });
            
        }
        [HttpPost("set-current-weapon/{id}")]
        public IActionResult SetCurrentWeapon(string id)
        {
            var weapon = _gameEngineService.GetWeapon(id);
            if (weapon == null)
            {
                return NotFound(new { message = $"Weapon with ID {id} is not in your arsenal." });
            }

            _gameEngineService.SetWeapon(weapon);
            return Ok(new { message = $"Weapon {weapon.Name} has been set as the current weapon." });
        }
        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new   
            {
                PlayerHealth = _gameEngineService.CurrentPlayer.Health,
                EnemiesCount = _gameEngineService.Enemies.Count,
                CurrentWeapon = _gameEngineService.CurrentWeapon?.Name ?? "None"
            });
        }
        

        [HttpPost("shoot")]
        public IActionResult Shoot()
        {
            var currentWeapon = _gameEngineService.CurrentWeapon;
            var targetEnemy = _gameEngineService.Enemies.FirstOrDefault(e => !e.IsDead);
            _gameEngineService.PlayerShootAction();
            return Ok(new { message = $"Player fired at {targetEnemy?.Name ?? "unknown target"} with {currentWeapon?.Name ?? "unknown weapon"}." });
        }
        [HttpPost("enemy-attack")]
        public IActionResult EnemyAttack()
        {
            var activeEnemies = _gameEngineService.Enemies.Where(e => !e.IsDead).ToList();
            if(!activeEnemies.Any())
            {
                return Ok(new { message = "No active enemies to attack the player." });
            }
            
            foreach (var enemy in activeEnemies)
            {
                
                    _gameEngineService.EnemyAttackAction(enemy);
                    return Ok(new { message = $"Enemy {enemy.Name} attacked the player." });
                
            }
            return Ok(new { message = "No active enemies to attack the player." });
        }
    }
}

