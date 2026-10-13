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
    public class WeaponController : ControllerBase
    {
        private readonly IRepository<Weapon> _weaponRepository;
        private readonly IWeaponFactory _weaponFactory;

        public WeaponController(IRepository<Weapon> weaponRepository, IWeaponFactory weaponFactory)
        {
            _weaponRepository = weaponRepository;
            _weaponFactory = weaponFactory;
        }

        [HttpPost]
        public async Task<IActionResult> CreateWeapon([FromBody] WeaponDto dto)
        {
            try
            {
                var weapon = _weaponFactory.CreateWeapon(dto);

                await _weaponRepository.AddAsync(weapon);

                return CreatedAtAction(nameof(GetWeaponById), new { id = weapon.Id }, weapon);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWeapons()
        {
            var weapons = await _weaponRepository.ListAsync();
            return Ok(weapons);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWeaponById(string id)
        {
            var weapon = await _weaponRepository.GetAsync(id);
            if (weapon == null)
            {
                return NotFound(new { message = $"Weapon with ID {id} not found." });
            }
            return Ok(weapon);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWeapon(string id, [FromBody] WeaponDto dto)
        {
            var existingWeapon = await _weaponRepository.GetAsync(id);
            if (existingWeapon == null)
            {
                return NotFound(new { message = $"Weapon with ID {id} not found." });
            }

            try
            {
                var updatedWeapon = _weaponFactory.CreateWeapon(dto);

                updatedWeapon.Id = id;

                await _weaponRepository.UpdateAsync(updatedWeapon);
                return Ok(updatedWeapon);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWeapon(string id)
        {
            var existingWeapon = await _weaponRepository.GetAsync(id);
            if (existingWeapon == null)
            {
                return NotFound(new { message = $"Weapon with ID {id} not found." });
            }

            await _weaponRepository.DeleteAsync(id);
            return NoContent(); 
        }
    }
}