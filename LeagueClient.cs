namespace Data.Models

{
    public class Champion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        [Required]
        [MaxLength(20)]
        public string Role { get; set; }

        [Range(1, 10)]
        public int Difficulty { get; set; }
    }
}




    public class Summoner
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Username { get; set; }

        [Required]
        public int Level { get; set; }
    }





    public class AppDbContext : DbContext
    {
        public DbSet<Champion> Champions { get; set; }
        public DbSet<Summoner> Summoners { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=lol_client.db");
        }
    }


    public class ChampionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
    }

    public class SummonerDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public int Level { get; set; }
    }




    public interface IChampionService
    {
        void Create(string name, string role, int difficulty);
        IEnumerable<ChampionDto> GetAll();
        void Update(int id, string role);
        void Delete(int id);
    }




    public class ChampionService : IChampionService
    {
        private readonly AppDbContext context = new();

        public void Create(string name, string role, int difficulty)
        {
            context.Champions.Add(new Champion
            {
                Name = name,
                Role = role,
                Difficulty = difficulty
            });
            context.SaveChanges();
        }

        public IEnumerable<ChampionDto> GetAll()
        {
            return context.Champions
                .Select(c => new ChampionDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Role = c.Role
                })
                .ToList();
        }

        public void Update(int id, string role)
        {
            var champion = context.Champions.Find(id);
            if (champion == null) return;

            champion.Role = role;
            context.SaveChanges();
        }

        public void Delete(int id)
        {
            var champion = context.Champions.Find(id);
            if (champion == null) return;

            context.Champions.Remove(champion);
            context.SaveChanges();
        }
    }




    public class ConsoleUI
    {
        private readonly IChampionService championService;

        public ConsoleUI(IChampionService championService)
        {
            this.championService = championService;
        }

        public void ShowMenu()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=== LEAGUE OF LEGENDS CLIENT ===");
            Console.ResetColor();

            Console.WriteLine("1. Add Champion");
            Console.WriteLine("2. List Champions");
            Console.WriteLine("3. Update Champion Role");
            Console.WriteLine("4. Remove Champion");
            Console.WriteLine("0. Exit");
        }

        public void Handle(string input)
        {
            switch (input)
            {
                case "1": Create(); break;
                case "2": List(); break;
                case "3": Update(); break;
                case "4": Delete(); break;
            }
        }

        private void Create()
        {
            Console.Write("Name: ");
            string name = Console.ReadLine();

            Console.Write("Role: ");
            string role = Console.ReadLine();

            Console.Write("Difficulty (1-10): ");
            int diff = int.Parse(Console.ReadLine());

            championService.Create(name, role, diff);
        }

        private void List()
        {
            foreach (var c in championService.GetAll())
            {
                Console.WriteLine($"{c.Id}. {c.Name} - {c.Role}");
            }
            Console.ReadKey();
        }

        private void Update()
        {
            Console.Write("ID: ");
            int id = int.Parse(Console.ReadLine());

            Console.Write("New Role: ");
            string role = Console.ReadLine();

            championService.Update(id, role);
        }

        private void Delete()
        {
            Console.Write("ID: ");
            int id = int.Parse(Console.ReadLine());

            championService.Delete(id);
        }
    }



    public class Engine
    {
        public void Run()
        {
            var championService = new ChampionService();
            var ui = new ConsoleUI(championService);

            while (true)
            {
                ui.ShowMenu();
                var input = Console.ReadLine();

                if (input == "0") break;

                ui.Handle(input);
            }
        }
    }



class Program
{
    static void Main()
    {
        new Engine().Run();
    }
}
