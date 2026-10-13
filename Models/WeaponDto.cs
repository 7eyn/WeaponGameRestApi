namespace WeaponGameRestApi.Models
{
    public class WeaponDto
    {
        public string? Id { get; set; }
        public string ?Type { get; set; } 
        public string ?Name { get; set; }
        public int Damage { get; set; }
        public double BulletSpeed { get; set; }
        public int MaxAmmo { get; set; }
        public int CurrentAmmo { get; set; }
        public double FireRate { get; set; }

        public bool IsAutomatic { get; set; } = false;
        public int PelletCount { get; set; } = 1;     
    }
}
