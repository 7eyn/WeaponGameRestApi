using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WeaponGameRestApi.Core;


namespace WeaponGameRestApi.Core
{
    public class Weapon
    {
        public virtual string SerializeToCsv()
        {
            return $"{GetType().Name}; {Name}; {CurrentWeaponBullet.Damage}; {CurrentWeaponBullet.Speed}; {MaxAmmo}; {CurrentAmmo}; {FireRate}";
        }
        public event Action<Weapon, int> OnFired;   
        public event Action<Weapon> OnMissed;     
        public event Action<Weapon> OnOutOfAmmo;
        public event Action<Weapon> OnReloaded;
        protected void InvokeOnFired(int damage) => OnFired?.Invoke(this, damage);
        protected void InvokeOnMissed() => OnMissed?.Invoke(this);
        protected void InvokeOnOutOfAmmo() => OnOutOfAmmo?.Invoke(this);
        protected void InvokeOnReloaded() => OnReloaded?.Invoke(this);

        private static readonly Random _rnd = new Random();

        private string _name;
        private int _maxAmmo;
        private int _ammo;
        private double _fireRate;
        public int ProjectilesPerShot { get; set; } = 1;

        public class Bullet 
        {
            private int _damage;
            private double _speed;

            public int Damage
            {
                get { return _damage; }
                set
                {
                    if (value < 0)
                    {
                        throw new ArgumentException("Damage cannot be negative.");
                    }
                    _damage = value;
                }
            }

            public double Speed
            {
                get { return _speed; }
                set
                {
                    if (value < 0)
                    {
                        throw new ArgumentException("Speed cannot be negative.");
                    }
                    _speed = value;
                }
            }
            public Bullet(int damage, double speed)
            {
                Damage = damage;
                Speed = speed;

            }
        }


        ///////////////////////////////////////////////////////////////////////////////
        //                              PROPERTIES                                   //
        ///////////////////////////////////////////////////////////////////////////////
        public Bullet CurrentWeaponBullet { get; private set; }
        public string Name
        {
            get { return _name; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Weapon name cannot be empty.");
                }
                _name = value;
            }
        }



        public int MaxAmmo
        {
            get { return _maxAmmo; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Max ammo cannot be negative.");
                }
                _maxAmmo = value;
            }
        }

        public int CurrentAmmo
        {
            get { return _ammo; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Current ammo cannot be negative.");
                }
                _ammo = value;
            }
        }

        public double FireRate
        {
            get { return _fireRate; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Fire rate cannot be negative.");
                }
                _fireRate = value;
            }
        }



        ///////////////////////////////////////////////////////////////////////////////
        //                              CONSTRUCTORS                                 //
        ///////////////////////////////////////////////////////////////////////////////
        public Weapon(IGameLogger logger)
        {
            Name = "Rifle";
            CurrentWeaponBullet = new Bullet(10, 100.0);
            MaxAmmo = 30;
            CurrentAmmo = 30;
            FireRate = 0.5;
            string logMessage = $"Weapon created: {_name}, Damage: {CurrentWeaponBullet.Damage}, Max Ammo: {_maxAmmo}, Fire Rate: {_fireRate}";
            logger.Log(logMessage);


        }
        public Weapon(string name, int damage, double bulletSpeed, int maxAmmo, int currentAmmo, double fireRate, IGameLogger logger)
        {
            Name = name;
            CurrentWeaponBullet = new Bullet(damage, bulletSpeed);
            MaxAmmo = maxAmmo;
            CurrentAmmo = currentAmmo;
            FireRate = fireRate;
            string logMessage = $"Weapon created: {_name}, Damage: {CurrentWeaponBullet.Damage}, Max Ammo: {_maxAmmo}, Fire Rate: {_fireRate}";
            logger.Log(logMessage);

        }
        ///////////////////////////////////////////////////////////////////////////////
        //                              METHODS                                      //
        ///////////////////////////////////////////////////////////////////////////////
        public virtual void Fire()
        {

            if (CurrentAmmo <= 0)
            {
                OnOutOfAmmo?.Invoke(this);
                Reload();
                return;
            }
            CurrentAmmo--;
            int number = _rnd.Next(1, 101);
            if (number <= 9)
            {
                OnMissed?.Invoke(this);
                
                return;
            }

            OnFired?.Invoke(this, CurrentWeaponBullet.Damage*ProjectilesPerShot);

        }
        public virtual void Reload()
        {
            OnReloaded?.Invoke(this);
            CurrentAmmo = MaxAmmo;
        }
        public virtual void DisplayInfo()
        {
            Console.WriteLine($"Weapon: {_name}");
            Console.WriteLine($"Damage: {CurrentWeaponBullet.Damage}");
            Console.WriteLine($"Ammo: {CurrentAmmo}");
            Console.WriteLine($"Fire Rate: {_fireRate}");

        }
    }
}
///////////////////////////////////////////////////////////////////////////////
//                              CLASSES                                      //
///////////////////////////////////////////////////////////////////////////////

class Revolver : Weapon
{
    public Revolver(string name, int damage, double bulletSpeed, int ammo, double fireRate, IGameLogger logger)
        : base(name, damage, bulletSpeed, 6, ammo, fireRate, logger)
    {
    }
    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine("-------------------------");
        Console.WriteLine("-------------------------");
    }
}

class Rifle : Weapon
{
    public bool IsAutomatic { get; set; }
    public Rifle(string name, int damage, double bulletSpeed, int maxAmmo, int ammo, double fireRate, bool isAutomatic, IGameLogger logger)
        : base(name, damage, bulletSpeed, maxAmmo, ammo, fireRate, logger)
    {
        IsAutomatic = isAutomatic;
    }
    public override string SerializeToCsv()
    {
        return $"{base.SerializeToCsv()}; {IsAutomatic}";
    }
    public override void Fire()
    {
        for (int i = 0; i < (IsAutomatic ? 3 : 1); i++)
        {
            if (CurrentAmmo <= 0)
            {
                base.Fire();
                break;
            }

            base.Fire();
        }
    }
    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine($"Is Automatic: {IsAutomatic}");
        Console.WriteLine("-------------------------");
        Console.WriteLine("-------------------------");

    }
    public void ToggleAutomatic()
    {
        IsAutomatic = !IsAutomatic;
        Console.WriteLine($"{Name} automatic mode is now {(IsAutomatic ? "ON" : "OFF")}");
    }
}

class Shotgun : Weapon
{
    
    public Shotgun(string name, int damage, double bulletSpeed, int maxAmmo, int ammo, double fireRate, int pelletCount, IGameLogger logger)
        : base(name, damage, bulletSpeed, maxAmmo, ammo, fireRate, logger)
    {
        ProjectilesPerShot = pelletCount;
        MaxAmmo = 2;
    }
    public override string SerializeToCsv()
    {
        return $"{base.SerializeToCsv()}; {ProjectilesPerShot}";
    }
    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine($"Pellet Count: {ProjectilesPerShot}");
        Console.WriteLine("-------------------------");
        Console.WriteLine("-------------------------");
    }
}
///////////////////////////////////////////////////////////////////////////////
//                              FACTORY                                      //
///////////////////////////////////////////////////////////////////////////////

public class WeaponFactory
{
    private static readonly Dictionary<string, Func<string[], IGameLogger, Weapon>> _weaponRegistry
        = new Dictionary<string, Func<string[], IGameLogger, Weapon>>(StringComparer.OrdinalIgnoreCase);


    static WeaponFactory()
    {
        RegisterNewWeapon("Revolver", (parts, logger) => new Revolver(
                parts[1].Trim(),
                int.Parse(parts[2].Trim()),
                double.Parse(parts[3].Trim(), System.Globalization.CultureInfo.InvariantCulture),
                int.Parse(parts[5].Trim()),
                double.Parse(parts[6].Trim(), System.Globalization.CultureInfo.InvariantCulture),
                logger));

        RegisterNewWeapon("Rifle", (parts, logger) => new Rifle(
            parts[1].Trim(),
            int.Parse(parts[2].Trim()),
            double.Parse(parts[3].Trim(), System.Globalization.CultureInfo.InvariantCulture),
            int.Parse(parts[4].Trim()),
            int.Parse(parts[5].Trim()),
            double.Parse(parts[6].Trim(), System.Globalization.CultureInfo.InvariantCulture),
            bool.Parse(parts[7].Trim()),
            logger));

        RegisterNewWeapon("Shotgun", (parts, logger) => new Shotgun(
            parts[1].Trim(),
            int.Parse(parts[2].Trim()),
            double.Parse(parts[3].Trim(), System.Globalization.CultureInfo.InvariantCulture),
            int.Parse(parts[4].Trim()),
            int.Parse(parts[5].Trim()),
            double.Parse(parts[6].Trim(), System.Globalization.CultureInfo.InvariantCulture),
            int.Parse(parts[7].Trim()),
            logger));
    }


    public static void RegisterNewWeapon(string type, Func<string[], IGameLogger, Weapon> creatorMethod)
    {
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Weapon type cannot be empty.");

        _weaponRegistry[type] = creatorMethod;
    }


    public static Weapon CreateWeapon(string[] parts,int lineNumber, IGameLogger logger)
    {

        string typeOfObject = parts[0].Trim();

        if (_weaponRegistry.TryGetValue(typeOfObject, out var creatorMethod))
        {
            try
            {
                return creatorMethod(parts, logger);
            }
            catch (Exception ex)
            {
                throw new InvalidRecordException($"Line {lineNumber}: Non-numeric value or incorrect data type. Details: {ex.Message}");
            }
        }

        throw new InvalidRecordException($"Line {lineNumber}: Unknown weapon type '{typeOfObject}'.");
    }
}
