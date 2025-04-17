// Importing necessary namespaces
using System.Text.Json; // Used for serializing and deserializing JSON data
using System.Text.Json.Serialization; // Provides attributes to control JSON serialization
#nullable enable // Enables nullable reference types for better null safety
using System; // Base namespace for system types
using System.Collections.Generic; // Provides List and other collections
using System.Linq; // Enables LINQ operations on collections
using System.Threading;
using System.Numerics; // Enables multithreading and thread-based delays

namespace main // Define the namespace for the program
{
    class Program // Main class for the task manager program
    {
        public static bool Save = true; // Determines if autosave is enabled
        public static int AutoSaveTime = 1; // Autosave interval in minutes

        public static string currentSearch = "";
        public static bool searchRunning = false;

        // Predefined tags that can be used to categorize tasks
        public static readonly string[] TAGS = 
        {
            "urgent",          // Needs immediate attention
            "high_priority",   // Important but not urgent
            "medium_priority", // Moderate importance
            "low_priority",    // Least important
            "personal",        // Personal tasks
            "work",            // Work-related tasks
            "home",            // Chores or household tasks
            "health",          // Fitness, mental health, etc.
            "learning",        // Study or skill-building tasks
            "waiting",         // Tasks blocked or waiting on others
            "optional"         // Not necessary but nice to do
        };

        public class Task // Defines a Task object
        {
            public bool done { get; set; } // Indicates whether the task is completed
            public string name { get; set; } // Task name
            public string description { get; set; } // Task description
            public string tag { get; set; } // List of tags associated with the task

            public Task() 
            {
                name = string.Empty; // Default empty name
                description = string.Empty; // Default empty description
                tag =string.Empty; // Initialize tags list
            }

            public Task(bool done, string name, string description, string tags)
            {
                this.done = done; // Set task completion status
                this.name = name; // Set task name
                this.description = description; // Set task description
                this.tag = tag ?? string.Empty; // Set task tags
            }
        }

        public static List<Task> tasks = new List<Task>(); // List to store all tasks

        public static void CreateTask(Task t) // Adds a new task to the list
        {
            if (tasks.Any(existing => existing.name == t.name)) // Check for duplicate task names
            {
                Console.WriteLine($"Task with name \"{t.name}\" already exists.");
                return;
            }

            
            if (!TAGS.Contains(t.tag) && t.tag != string.Empty) // If tag is not allowed
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Invalid tag: {t}");
                return;
            }
            

            tasks.Add(t); // Add task to the list
        }

        public static bool CompleteTask(string name) // Marks a task as completed
        {
            foreach (Task task in tasks)
            {
                if (task.name == name)
                {
                    task.done = true; // Mark task as done
                    return true;
                }
            }
            return false; // Task not found
        }

        public static bool UpdateTask(string name, Task update) // Updates an existing task
        {
            for (int i = 0; i < tasks.Count; i++)
            {
                if (tasks[i].name == name)
                {
                    tasks[i] = update; // Replace task with updated version
                    return true;
                }
            }
            return false; // Task not found
        }

        public static List<Task> GetTasks(string path = "./Tasks.json") // Loads tasks from file
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Getting Tasks From Tasks.json");

            if (!File.Exists(path)) // If the file doesn't exist
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Could not find Tasks file. Writing a new instance.");
                var emptyTasks = new List<Task>(); // Create empty task list
                File.WriteAllText(path, JsonSerializer.Serialize(emptyTasks, new JsonSerializerOptions { WriteIndented = true }));
                Console.ForegroundColor = ConsoleColor.Cyan;
                return emptyTasks;
            }

            string json = File.ReadAllText(path); // Read file content

            if (string.IsNullOrWhiteSpace(json)) // If file is empty
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Tasks file is empty. Initializing with empty task list.");
                var emptyTasks = new List<Task>();
                File.WriteAllText(path, JsonSerializer.Serialize(emptyTasks, new JsonSerializerOptions { WriteIndented = true }));
                Console.ForegroundColor = ConsoleColor.Cyan;
                return emptyTasks;
            }

            try
            {
                return JsonSerializer.Deserialize<List<Task>>(json) ?? new List<Task>(); // Try to deserialize tasks
            }
            catch (JsonException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error reading Tasks.json: {ex.Message}");
                Console.WriteLine("Resetting file to a new empty task list.");

                var emptyTasks = new List<Task>();
                File.WriteAllText(path, JsonSerializer.Serialize(emptyTasks, new JsonSerializerOptions { WriteIndented = true }));
                Console.ForegroundColor = ConsoleColor.Cyan;
                return emptyTasks;
                
            }
        }

        public static void WriteTasks(string path = "./Tasks.json") // Saves tasks to file
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Saving Tasks To Tasks.json");
            string json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true }); // Convert list to JSON
            File.WriteAllText(path, json); // Write to file
            Console.ForegroundColor = ConsoleColor.Cyan;
        }

        public static void ListTasks() // Lists all tasks to the console
        {
            Dictionary<string, List<Task>> categorizedTasks = new Dictionary<string, List<Task>>();

            // Initialize the dictionary with all tags
            foreach (string tag in TAGS)
            {
                categorizedTasks[tag] = new List<Task>();
            }

            // Categorize each task
            foreach (Task task in tasks)
            {
                if (categorizedTasks.ContainsKey(task.tag))
                {
                    categorizedTasks[task.tag].Add(task);
                }
                else
                {
                    // Fallback: unknown tag
                    if (!categorizedTasks.ContainsKey("uncategorized"))
                        categorizedTasks["uncategorized"] = new List<Task>();
                    categorizedTasks["uncategorized"].Add(task);
                }
            }

            // Display tasks per category
            foreach (var tagGroup in categorizedTasks)
            {
                if (tagGroup.Value.Count == 0) continue;

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\n{new string('=', 25)}\n{tagGroup.Key.ToUpper()}\n{new string('=', 25)}\n");
                Console.ForegroundColor = ConsoleColor.Cyan;

                foreach (Task task in tagGroup.Value)
                {
                    if (task.done)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"[x] {task.name}: {task.description}");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"[ ] {task.name}: {task.description}");
                    }
                    Thread.Sleep(100);
                }
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
        }   

        public static void AutoSave() // Continuously autosaves in the background
        {
            while (true)
            {
                if (Save)
                {
                    WriteTasks(); // Save tasks
                    Thread.Sleep(TimeSpan.FromMinutes(AutoSaveTime)); // Wait before next save
                }
            }
        }
        public static void StartLiveSearch()
        {
            System.Threading.Tasks.Task.Run(() =>
            {
                while (searchRunning)
                {
                    Console.Clear();
                    Console.WriteLine("[Esc] to do back]");
                    Console.WriteLine($"Search: {currentSearch}");
                    Console.WriteLine(new string('-', 40));
                    ShowMatchingTasks(currentSearch);
                    Thread.Sleep(100); // Avoid flickering
                }
            });

            while (searchRunning)
            {
                var key = Console.ReadKey(searchRunning);

                if (key.Key == ConsoleKey.Escape)
                {
                    searchRunning = false;
                    break;
                }

                if (key.Key == ConsoleKey.Backspace && currentSearch.Length > 0)
                {
                    currentSearch = currentSearch.Substring(0, currentSearch.Length - 1);
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    currentSearch += key.KeyChar;
                }

            
            }

            Console.Clear();
            Console.WriteLine("Exited live search.");
        }
        public static void ShowMatchingTasks(string query)
        {
            var filtered = tasks.Where(t =>
                t.name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                t.description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                t.tag.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (var task in filtered)
            {
                Console.ForegroundColor = task.done ? ConsoleColor.Green : ConsoleColor.Yellow;
                Console.WriteLine($"{(task.done ? "[x]" : "[ ]")} {task.name} ({task.tag}) - {task.description}");
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
        }

        public static void ShowTags(string id) {
            
            

            Console.ForegroundColor = ConsoleColor.Cyan;
        }
        
        public static void ExtOnEsc() {
            while (true) {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Escape) {
                    break;
                }
            }
        }
        public static void Main(string[] args) // Main program loop
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            tasks = GetTasks(); // Load tasks from file
            Thread.Sleep(300); // Initial delay
            System.Threading.Tasks.Task.Run(() => AutoSave()); // Start autosave in background
            while (true)
            {
                Console.Clear(); // Clear console

                Console.WriteLine("\nEnter Operation:\n [c] Create Task   [u] Update Task\n [l] List Tasks    [d] Complete Task\n [q] Quit          [s] Save\n [i] Settings      [o] Search Tasks");
                var key = Console.ReadKey(true);
                string? input = ""; // Read user input
                if (key.Key == ConsoleKey.Backspace && currentSearch.Length > 0)
                {
                    input = currentSearch.Substring(0, currentSearch.Length - 1);
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    input += key.KeyChar;
                }

                switch (input)
                {
                    case "c": // Create task
                        Console.Clear();
                        Console.Write("Enter Name: ");
                        string? name = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(name)) break;

                        Console.Write("Enter Description: ");
                        string? description = Console.ReadLine();
                        if (description == null) break;

                        Console.Write("Enter Tags (comma-separated): ");
                        string? tag = Console.ReadLine();
                        if (description == null) break;


                        Task t = new Task(false, name, description, tag ?? string.Empty); // Create task object

                        CreateTask(t); // Add task
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine($"\n\nCreated Task {t.name}");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Thread.Sleep(500); // Pause
                        break;

                    case "u": // Update task
                        Console.Clear();
                        Console.Write("Enter Name to Update: ");
                        string? originalName = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(originalName)) break;

                        Console.Write("Enter New Name: ");
                        string? updatedName = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(updatedName)) break;

                        Console.Write("Enter Description: ");
                        string? newDescription = Console.ReadLine();
                        if (newDescription == null) break;

                        Console.Write("Enter Tags (comma-separated): ");
                        string? newTag = Console.ReadLine();


                        t = new Task(false, updatedName, newDescription, newTag ?? string.Empty); // Create updated task

                        UpdateTask(originalName, t); // Update task
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine($"\n\nUpdated Task {originalName} to {updatedName}");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Thread.Sleep(3000);
                        break;

                    case "d": // Complete task
                        Console.Clear();
                        System.Threading.Tasks.Task.Run(() => {ExtOnEsc();});
                        Console.Write("Enter Name of Task to Complete: ");
                        string? taskToComplete = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(taskToComplete))
                        {
                            CompleteTask(taskToComplete); // Mark as completed
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.WriteLine($"\n\nCompleted Task {taskToComplete}");
                            Console.ForegroundColor = ConsoleColor.Cyan;
                        }
                        Thread.Sleep(500);
                        break;

                    case "l": // List tasks
                        bool leave = false;
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("Listing all tasks.\n[Esc] to exit");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        ListTasks();
                        ExtOnEsc();
                        break;

                    case "i": // Settings menu
                        leave = false;
                        while (!leave) {
                            Console.Clear();
                            Console.WriteLine("[s] Toggle Autosave [t] Set Autosave Time [b] Go back");
                            key = Console.ReadKey(true);
                            input = ""; // Read user input
                            if (key.Key == ConsoleKey.Backspace && currentSearch.Length > 0)
                            {
                                input = currentSearch.Substring(0, currentSearch.Length - 1);
                            }
                            else if (!char.IsControl(key.KeyChar))
                            {
                                input += key.KeyChar;
                            }

                            switch (input?.ToLower()) {
                                case "s":
                                    Save = !Save; // Toggle autosave
                                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                                    Console.WriteLine("Autosave " + Save);
                                    Console.ForegroundColor = ConsoleColor.Cyan;
                                    break;
                                case "t":
                                    Console.Clear();
                                    Console.WriteLine("Enter save delay in minutes: ");

                                    if (int.TryParse(Console.ReadLine(), out int newTime))
                                    {
                                        AutoSaveTime = newTime; // Set autosave time
                                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                                        if (newTime == 1) Console.WriteLine($"Autosave time set to {newTime} minute.");
                                        else Console.WriteLine($"Autosave time set to {newTime} minutes.");
                                        Console.ForegroundColor = ConsoleColor.Cyan;
                                    }
                                    else
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine("Invalid input. Please enter a valid number.");
                                        Console.ForegroundColor = ConsoleColor.Cyan;

                                    }

                                    break;
                                case "b":
                                    leave = true; // Exit settings
                                    break;
                                default:
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Invalid input");
                                    Thread.Sleep(500);
                                    Console.ForegroundColor = ConsoleColor.Cyan;
                                    break;   
                            }
                        }
                        Thread.Sleep(500);
                        
                        break;

                    case "s": // Manual save
                        Console.Clear();
                        WriteTasks();
                        Thread.Sleep(500);
                        break;
                    
                    case "o":
                        searchRunning = true;
                        Console.Clear();                       
                        StartLiveSearch();
                        ExtOnEsc();
                        break; 

                    case "q": // Quit program
                        Console.Clear();
                        Console.WriteLine("leaving the program");
                        WriteTasks();
                        Thread.Sleep(500);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        return;

                    default: // Invalid command
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid input");
                        Thread.Sleep(500);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        break;
                }
            }
        }
    }
}
