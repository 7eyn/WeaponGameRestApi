using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Linq;
using WeaponGameRestApi.Core;



namespace WeaponGameRestApi.Core
{
    class Program
    {

        static void Main(string[] args)
        {
            using (FileLogger logger = new FileLogger("Log.txt"))
            {
                logger.Log("Application started.");
                try
                {
                    
                    List<Weapon> weapons = new List<Weapon>
                {
                    new Shotgun("Shotgun", 2, 200.0, 2, 2, 1.5, 25, logger),
                    new Revolver("Revolver", 20, 300.0, 1, 1.0, logger),
                    new Rifle("Rifle", 10, 500.0, 30, 30, 0.5, true, logger)
                    
                };
                    Player player = new Player(1000);
                    List<Enemy> enemies = new List<Enemy>
                    {
                        EnemyFactory.CreateEnemy("Soldier", 50, 25),
                        EnemyFactory.CreateEnemy("Tank", 200, 10),
                        EnemyFactory.CreateEnemy("Boss", 500, 50)
                    };
                    GameController gameController = new GameController(player, weapons, enemies, logger);
                    gameController.StartGame();


                }
                catch (Exception ex)
                {
                    logger.Log($"Error: {ex.Message}");
                    Console.WriteLine($"Error: {ex.Message}");
                }
                finally
                {
                    logger.Log("Application ended.");
                }
            }
        }
    }
}


