using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using WeaponGameRestApi.Core;
using WeaponGameRestApi.Models;
using WeaponGameRestApi.Services;




namespace WeaponGameRestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]


    public class GameController : ControllerBase
    {
        
        private readonly GameEngineService _gameEngineService;
        private readonly IWeaponFactory _weaponFactory;
        private readonly IEnemyFactory _enemyFactory;
       public GameController(GameEngineService gameEngineService, IWeaponFactory weaponFactory, IEnemyFactory enemyFactory)
        {
            _gameEngineService = gameEngineService;
            _weaponFactory = weaponFactory;
            _enemyFactory = enemyFactory;
        }
        [HttpPost("add-enemy")]
        public IActionResult Add([FromBody] EnemyDto dto)
        {
            try
            {
                var enemy = _enemyFactory.CreateEnemy(dto);

                _gameEngineService.AddEnemy(enemy); 
                return Ok(new { message = $"Enemy {enemy.Name} added" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpPost("add-weapon")]
        public IActionResult AddWeapon([FromBody] WeaponDto dto)
        {
            try
            {
               
                string specificParam = "0";
                if (dto.Type.Equals("Rifle", StringComparison.OrdinalIgnoreCase))
                {
                    specificParam = dto.IsAutomatic.ToString();
                }
                else
                {
                    if (dto.Type.Equals("Shotgun", StringComparison.OrdinalIgnoreCase))
                    {
                        specificParam = dto.PelletCount.ToString();
                    }
                }


                string[] parts = new string[]
                {
            dto.Type,
            dto.Name,
            dto.Damage.ToString(),
            dto.BulletSpeed.ToString(System.Globalization.CultureInfo.InvariantCulture),
            dto.MaxAmmo.ToString(),
            dto.CurrentAmmo.ToString(),
            dto.FireRate.ToString(System.Globalization.CultureInfo.InvariantCulture),
            specificParam
                };

                
                var weapon = _weaponFactory.CreateWeapon(dto);
                _gameEngineService.AddWeapon(weapon);

                _gameEngineService.SetWeapon(weapon);

                return Ok(new { message = $"Weapon {weapon.Name} has been created " });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new   
            {

                PlayerHealth = _gameEngineService.CurrentPlayer.Health,
                EnemiesCount = _gameEngineService.Enemies.Count
            });
        }
        [HttpGet("set-weapon")]
        public IActionResult SetWeapon([FromQuery] int weaponId)
        {
            var weapon = _gameEngineService.GetWeapon(weaponId - 1);
            if (weapon == null)
            {
                return NotFound(new { message = $"Weapon {weaponId} not found." });
            }
            _gameEngineService.SetWeapon(weapon);
            return Ok(new { message = $"Weapon {weapon.Name} has been set as the current weapon." });
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
            foreach (var enemy in _gameEngineService.Enemies)
            {
                if (!enemy.IsDead)
                {
                    _gameEngineService.EnemyAttackAction(enemy);
                    return Ok(new { message = $"Enemy {enemy.Name} attacked the player." });
                }
            }
            return Ok(new { message = "No active enemies to attack the player." });
        }
    }
}

