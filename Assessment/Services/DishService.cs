using Assessment.Models;
using System.Text.Json;

namespace Assessment.Services
{
    public class DishService
    {
        private readonly string _dataFilePath;

        private static readonly HashSet<string> SpecificImages = new(StringComparer.OrdinalIgnoreCase)
        {
            "gongbaojiding.jpg", "mapodoufu.jpg", "qingzhenluyu.jpg", "baiqieji.jpg", "kaoya.jpg"
        };

        public DishService()
        {
            _dataFilePath = GetDataFilePath();
            InitializeDataFile();
        }

        private string GetDataFilePath()
        {
            var appDataDir = FileSystem.AppDataDirectory;
            return Path.Combine(appDataDir, "dishes.json");
        }

        private async void InitializeDataFile()
        {
            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("dishes.json");
                using var reader = new StreamReader(stream);
                var json = await reader.ReadToEndAsync();
                File.WriteAllText(_dataFilePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DishService] Failed to seed from bundled asset: {ex.Message}");
                if (!File.Exists(_dataFilePath))
                {
                    var dishes = GenerateDefaultDishes();
                    SaveDishesToFile(dishes);
                }
            }
        }

        private List<Dish> LoadDishesFromFile()
        {
            try
            {
                var jsonContent = File.ReadAllText(_dataFilePath);
                return JsonSerializer.Deserialize<List<Dish>>(jsonContent) ?? new List<Dish>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DishService] Failed to load dishes: {ex.Message}");
                return GenerateDefaultDishes();
            }
        }

        private void SaveDishesToFile(List<Dish> dishes)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(dishes, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_dataFilePath, jsonContent);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DishService] Failed to save dishes: {ex.Message}");
            }
        }

        public List<Dish> GetAllDishes() => SortDishes(LoadDishesFromFile());

        public Dish? GetDishById(int id) => LoadDishesFromFile().FirstOrDefault(d => d.Id == id);

        public List<Dish> GetDishesByCategory(string category) =>
            SortDishes(LoadDishesFromFile().Where(d => d.Category == category).ToList());

        public List<Dish> GetDishesPaged(int page, int pageSize, string? category = null)
        {
            var dishes = LoadDishesFromFile();
            var source = category == null || category == "All"
                ? dishes
                : dishes.Where(d => d.Category == category);
            var sorted = SortDishes(source.ToList());
            return sorted.Skip(page * pageSize).Take(pageSize).ToList();
        }

        public List<string> GetCategories() =>
            LoadDishesFromFile().Select(d => d.Category).Distinct().ToList();

        private static List<Dish> SortDishes(List<Dish> dishes)
        {
            return dishes
                .OrderByDescending(d => SpecificImages.Contains(d.ImageName ?? string.Empty))
                .ThenBy(d => d.Id)
                .ToList();
        }

        public void AddDish(Dish dish)
        {
            var dishes = LoadDishesFromFile();
            dish.Id = dishes.Any() ? dishes.Max(d => d.Id) + 1 : 1;
            dishes.Add(dish);
            SaveDishesToFile(dishes);
        }

        public void UpdateDish(Dish dish)
        {
            var dishes = LoadDishesFromFile();
            var index = dishes.FindIndex(d => d.Id == dish.Id);
            if (index != -1)
            {
                dishes[index] = dish;
                SaveDishesToFile(dishes);
            }
        }

        public void DeleteDish(int dishId)
        {
            var dishes = LoadDishesFromFile();
            dishes.RemoveAll(d => d.Id == dishId);
            SaveDishesToFile(dishes);
        }

        private List<Dish> GenerateDefaultDishes()
        {
            return new List<Dish>
            {
                new Dish
                {
                    Id = 1, Name = "Kung Pao Chicken", Category = "Sichuan",
                    Description = "A classic Sichuan dish featuring diced chicken stir-fried with peanuts, dried chili peppers, and Sichuan peppercorns. The chicken is tender and the peanuts add a satisfying crunch, making it a perfect companion to steamed rice.",
                    Price = 28.00m, ImageName = "gongbaojiding.jpg", Rating = 4.8, IsSpicy = true, PrepTimeMinutes = 20
                },
                new Dish
                {
                    Id = 2, Name = "Mapo Tofu", Category = "Sichuan",
                    Description = "A signature Sichuan dish with silky tofu cubes simmered in a spicy, aromatic sauce with minced beef and doubanjiang (fermented broad bean paste). The dish is vibrant red, intensely flavorful, and wonderfully fragrant.",
                    Price = 22.00m, ImageName = "mapodoufu.jpg", Rating = 4.7, IsSpicy = true, PrepTimeMinutes = 15
                },
                new Dish
                {
                    Id = 3, Name = "Steamed Sea Bass", Category = "Cantonese",
                    Description = "A Cantonese classic made with fresh sea bass, steamed to perfection and finished with sizzling hot oil and soy sauce. The fish is delicate, succulent, and retains its natural sweetness and nutritional value.",
                    Price = 58.00m, ImageName = "qingzhenluyu.jpg", Rating = 4.9, IsSpicy = false, PrepTimeMinutes = 25
                },
                new Dish
                {
                    Id = 4, Name = "Poached Chicken", Category = "Cantonese",
                    Description = "A traditional Cantonese dish featuring chicken poached to achieve smooth, glossy skin and tender, subtly sweet meat. Served with a classic ginger-scallion dipping sauce, it is an indispensable staple of Cantonese cuisine.",
                    Price = 38.00m, ImageName = "baiqieji.jpg", Rating = 4.6, IsSpicy = false, PrepTimeMinutes = 30
                },
                new Dish
                {
                    Id = 5, Name = "Peking Duck", Category = "Beijing",
                    Description = "One of China's most internationally renowned dishes. The duck is roasted until the skin is perfectly crisp and the meat is juicy and tender. Served with thin pancakes, sweet bean sauce, and shredded scallions for a uniquely satisfying experience.",
                    Price = 88.00m, ImageName = "kaoya.jpg", Rating = 4.9, IsSpicy = false, PrepTimeMinutes = 45
                },
                new Dish
                {
                    Id = 6, Name = "Dongpo Braised Pork", Category = "Zhejiang",
                    Description = "A famous Hangzhou dish of pork belly slowly braised until caramelized and deeply flavorful. The result is melt-in-your-mouth tender meat with a glossy, reddish-brown color. Rich without being greasy, it is a true comfort dish.",
                    Price = 45.00m, ImageName = "general.jpg", Rating = 4.7, IsSpicy = false, PrepTimeMinutes = 60
                },
                new Dish
                {
                    Id = 7, Name = "Sichuan Boiled Fish", Category = "Sichuan",
                    Description = "A Chongqing-style dish featuring tender fish fillets poached in a fiery, aromatic chili oil broth with bean sprouts and dried chilies. Spicy, numbing, and intensely flavorful without being greasy.",
                    Price = 48.00m, ImageName = "general.jpg", Rating = 4.8, IsSpicy = true, PrepTimeMinutes = 25
                },
                new Dish
                {
                    Id = 8, Name = "Sweet & Sour Pork", Category = "Shandong",
                    Description = "A classic Shandong dish of crispy battered pork tenderloin coated in a glossy sweet and sour sauce. Golden in color, with a perfect balance of tangy and sweet flavors, it is beloved by both children and adults alike.",
                    Price = 32.00m, ImageName = "general.jpg", Rating = 4.5, IsSpicy = false, PrepTimeMinutes = 20
                },
                new Dish
                {
                    Id = 9, Name = "Shrimp Dumplings", Category = "Cantonese",
                    Description = "The quintessential Cantonese dim sum item. Translucent, delicate wrappers encase plump, fresh shrimp. The texture is pleasantly bouncy (QQ) and each bite releases a burst of juicy, savory flavor.",
                    Price = 25.00m, ImageName = "general.jpg", Rating = 4.8, IsSpicy = false, PrepTimeMinutes = 15
                },
                new Dish
                {
                    Id = 10, Name = "Twice-Cooked Pork", Category = "Sichuan",
                    Description = "The king of Sichuan home cooking. Slices of pork belly are first boiled, then wok-fried with leeks, garlic sprouts, and doubanjiang. Richly aromatic with a lingering savory finish, it is the ultimate comfort food.",
                    Price = 30.00m, ImageName = "general.jpg", Rating = 4.6, IsSpicy = true, PrepTimeMinutes = 20
                },
                new Dish
                {
                    Id = 11, Name = "Stir-Fried Rice Noodles", Category = "Cantonese",
                    Description = "A classic Cantonese street food dish. Wide rice noodles are wok-fried over intense heat with beef and bean sprouts, developing that signature wok hei (breath of wok) for a smoky, deeply satisfying flavor.",
                    Price = 35.00m, ImageName = "general.jpg", Rating = 4.6, IsSpicy = false, PrepTimeMinutes = 15
                },
                new Dish
                {
                    Id = 12, Name = "Pickled Cabbage Fish", Category = "Sichuan",
                    Description = "A nationwide sensation in recent years. Delicate fish fillets swim in a tangy, spicy broth with pickled mustard greens that add a refreshing sour crunch. Bright, appetizing, and irresistibly warming.",
                    Price = 52.00m, ImageName = "general.jpg", Rating = 4.8, IsSpicy = true, PrepTimeMinutes = 25
                },
                new Dish
                {
                    Id = 13, Name = "Lion's Head Meatballs", Category = "Huaiyang",
                    Description = "A celebrated Huaiyang dish of oversized, tender pork meatballs braised with leafy greens in a rich, savory broth. The name symbolizes reunion and happiness, and the dish delivers deep, satisfying flavor with every bite.",
                    Price = 42.00m, ImageName = "general.jpg", Rating = 4.5, IsSpicy = false, PrepTimeMinutes = 40
                },
                new Dish
                {
                    Id = 14, Name = "Soup Dumplings", Category = "Shanghai",
                    Description = "Shanghai's iconic specialty. Each dumpling has a paper-thin wrapper generously filled with seasoned pork and a burst of rich, savory broth. A gentle bite releases the soup, creating a memorable and delightful experience.",
                    Price = 20.00m, ImageName = "general.jpg", Rating = 4.8, IsSpicy = false, PrepTimeMinutes = 20
                },
                new Dish
                {
                    Id = 15, Name = "Lanzhou Hand-Pulled Noodles", Category = "Noodles",
                    Description = "One of China's top ten noodle dishes. Hand-pulled noodles are springy and smooth, served in a crystal-clear, flavorful beef broth with thin slices of radish and fresh cilantro — simple, pure, and utterly satisfying.",
                    Price = 18.00m, ImageName = "general.jpg", Rating = 4.5, IsSpicy = false, PrepTimeMinutes = 10
                },
                new Dish
                {
                    Id = 16, Name = "Char Siu (BBQ Pork)", Category = "Cantonese",
                    Description = "The crown jewel of Cantonese roasted meats. Pork is marinated in a sweet-savory glaze and roasted until the exterior is beautifully glazed and the interior remains tender and juicy. Sweet, smoky, and deeply satisfying.",
                    Price = 35.00m, ImageName = "general.jpg", Rating = 4.7, IsSpicy = false, PrepTimeMinutes = 50
                },
                new Dish
                {
                    Id = 17, Name = "Di San Xian", Category = "Dongbei",
                    Description = "A beloved Dongbei home-style dish. Eggplant, potato, and green pepper are stir-fried together in a savory sauce. The three vegetables complement each other perfectly, creating a nutritious and satisfying vegetarian-friendly option.",
                    Price = 22.00m, ImageName = "general.jpg", Rating = 4.4, IsSpicy = false, PrepTimeMinutes = 15, IsVegetarian = true
                },
                new Dish
                {
                    Id = 18, Name = "Spicy Chicken", Category = "Sichuan",
                    Description = "A Chongqing specialty featuring bite-sized chicken pieces fried until crispy on the outside and tender inside, then stir-fried with an abundance of dried chilies and Sichuan peppercorns for a fiery, numbing, and utterly addictive flavor.",
                    Price = 36.00m, ImageName = "general.jpg", Rating = 4.7, IsSpicy = true, PrepTimeMinutes = 25
                },
                new Dish
                {
                    Id = 19, Name = "Guo Bao Rou", Category = "Dongbei",
                    Description = "A signature Dongbei dish of pork loin slices coated in potato starch, double-fried to a golden crisp, and then coated in a tangy, sweet-and-sour sauce. Crispy on the outside, tender inside, with a perfectly balanced flavor.",
                    Price = 30.00m, ImageName = "general.jpg", Rating = 4.6, IsSpicy = false, PrepTimeMinutes = 20
                },
                new Dish
                {
                    Id = 20, Name = "Clay Pot Rice", Category = "Cantonese",
                    Description = "A traditional Cantonese comfort dish where rice is cooked in a clay pot with cured meats, chicken, and vegetables. The bottom develops a golden, crispy crust while the top remains fluffy and aromatic — a true sensory delight.",
                    Price = 32.00m, ImageName = "general.jpg", Rating = 4.7, IsSpicy = false, PrepTimeMinutes = 25
                }
            };
        }
    }
}
