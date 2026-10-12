
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
        public static List<Weapon> ReadWeaponsFromFile(string filePath, IGameLogger logger)
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
                        logger.Log($"Line {lineNumber}: Empty or whitespace line skipped.");
                        continue;
                    }
                    try
                    {
                        var weapon = ParseLine(line, lineNumber, logger);
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
        public static Weapon ParseLine(string line, int lineNumber, IGameLogger logger)
        {
            var parts = line.Split(';');
            try
            {
                if (parts.Length < 7)
                {
                    throw new InvalidRecordException($"Line {lineNumber}: Not enough parameters to create a weapon.");
                }
                return WeaponFactory.CreateWeapon(parts, lineNumber, logger);
            }

            catch (Exception ex)
            {
                throw new InvalidRecordException($"Line {lineNumber}: Non-numeric value or incorrect data type. Details: {ex.Message}");
            }
        }
    }
}
