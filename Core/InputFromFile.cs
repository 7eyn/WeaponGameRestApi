
using WeaponGameRestApi.Models;

public class InvalidRecordException : Exception
{
    public InvalidRecordException(string message) : base(message)
    {

    }
}

namespace WeaponGameRestApi.Core

{
    internal class InputFromFile
    {
        public static List<Weapon> ReadWeaponsFromFile(string filePath, IGameLogger logger, IWeaponFactory _weaponFactory)
        {
            var weapons = new List<Weapon>();
            int lineNumber = 0;
            try
            {
                foreach (var line in File.ReadLines(filePath))
                {
                    lineNumber++;
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        
                        continue;
                    }
                    try
                    {
                        var weapon = ParseLine(line, lineNumber,_weaponFactory);
                        weapons.Add(weapon);
                    }
                    catch (InvalidRecordException ex)
                    {
                        logger.Log(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log($"Error reading file: {ex.Message}");
            }
            return weapons;
        }
        public static Weapon ParseLine(string line, int lineNumber, IWeaponFactory _weaponFactory)
        {
            var parts = line.Split(';');
            try
            {
                if (parts.Length < 7)
                {
                    throw new InvalidRecordException($"Line {lineNumber}: Not enough parameters to create a weapon.");
                }
                string type = parts[0].Trim();
                var dto = new WeaponDto
                {
                    Type = type,
                    Name = parts[1].Trim(),
                    Damage = int.Parse(parts[2].Trim()),
                    BulletSpeed = double.Parse(parts[3].Trim(), System.Globalization.CultureInfo.InvariantCulture),
                    MaxAmmo = int.Parse(parts[4].Trim()),
                    CurrentAmmo = int.Parse(parts[5].Trim()),
                    FireRate = double.Parse(parts[6].Trim(), System.Globalization.CultureInfo.InvariantCulture)
                };
                if (parts.Length > 7)
                {
                    if (type.Equals("Rifle", StringComparison.OrdinalIgnoreCase))
                        dto.IsAutomatic = bool.Parse(parts[7].Trim());
                    else if (type.Equals("Shotgun", StringComparison.OrdinalIgnoreCase))
                        dto.PelletCount = int.Parse(parts[7].Trim());
                }

                return _weaponFactory.CreateWeapon(dto);
            }

            catch (Exception ex)
            {
                throw new InvalidRecordException($"Line {lineNumber}: Non-numeric value or incorrect data type. Details: {ex.Message}");
            }
        }
    }
}
