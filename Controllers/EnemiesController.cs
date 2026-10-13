using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WeaponGameRestApi.Core;
using WeaponGameRestApi.Models;
using WeaponGameRestApi.Repositories;

namespace WeaponGameRestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnemiesController : ControllerBase
    {
        private readonly IRepository<Enemy> _enemyRepository;
        private readonly IEnemyFactory _enemyFactory;

        public EnemiesController(IRepository<Enemy> enemyRepository, IEnemyFactory enemyFactory)
        {
            _enemyRepository = enemyRepository;
            _enemyFactory = enemyFactory;
        }

        [HttpPost]
        public async Task<IActionResult> CreateEnemy([FromBody] EnemyDto dto)
        {
            try
            {
                var enemy = _enemyFactory.CreateEnemy(dto);

                await _enemyRepository.AddAsync(enemy);

                return CreatedAtAction(nameof(GetEnemyById), new { id = enemy.Id }, enemy);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEnemies()
        {
            var enemies = await _enemyRepository.ListAsync();
            return Ok(enemies);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEnemyById(string id)
        {
            var enemy = await _enemyRepository.GetAsync(id);
            if (enemy == null)
            {
                return NotFound(new { message = $"Enemy with ID {id} not found." });
            }
            return Ok(enemy);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEnemy(string id, [FromBody] EnemyDto dto)
        {
            var existingEnemy = await _enemyRepository.GetAsync(id);
            if (existingEnemy == null)
            {
                return NotFound(new { message = $"Enemy with ID {id} not found." });
            }

            try
            {
                var updatedEnemy = _enemyFactory.CreateEnemy(dto);

                updatedEnemy.Id = id;

                await _enemyRepository.UpdateAsync(updatedEnemy);
                return Ok(updatedEnemy);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnemy(string id)
        {
            var existingEnemy = await _enemyRepository.GetAsync(id);
            if (existingEnemy == null)
            {
                return NotFound(new { message = $"Enemy with ID {id} not found." });
            }

            await _enemyRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}