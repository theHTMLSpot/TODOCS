# ✅ Terminal Task Manager

A lightweight, fully-featured command-line task manager written in **C#**.  
Supports live search, tagging, autosave, and a clean, minimal user experience — all from your terminal.

---

## ✨ Features

- 📋 Create, read, update, and complete tasks
- 🏷️ Tag tasks with categories (e.g., `school`, `work`, `personal`)
- 🔎 **Live search** with real-time filtering by name or tag
- 💾 **Autosave** to local `.taskdata` file for persistence
- ✅ Visual completion indicators (✔️ / ❌)
- 🌈 Color-coded task output
- 🔁 Auto-clear finished tasks from the main view (optional)
- 🧠 Simple and fast — no external libraries or frameworks

---

## 📦 Getting Started

### Prerequisites

- [.NET 6 SDK or later](https://dotnet.microsoft.com/en-us/download)

### Running the App

```bash
dotnet run
```

Tasks will autosave to a `.taskdata` file in the project root.

---

## 🛠️ Usage

After starting the app, you'll be presented with a menu:

```
1. Create task
2. Show all tasks
3. Show completed tasks
4. Show unfinished tasks
5. Start live search
6. Clear finished tasks
7. Save and Exit
```

### Task Format

Each task includes:
- Name
- Description
- Tag (e.g., `school`, `work`)
- Completion Status

---

## 🔍 Live Search

Start typing to search task **name** or **tag**.  
Use arrow keys and backspace to edit your query in real time.

---

## 📁 Data Persistence

Tasks are automatically saved to a `.taskdata` file using JSON serialization.  
On next launch, tasks are reloaded so you never lose progress.

---

## 🚀 Planned Features

- Due dates with sorting
- CLI flags for quick interactions
- Task deletion
- Tag autocomplete & suggestion
- UI animations with ASCII art flair 😎

---

## 🤝 Contributing

PRs are welcome! If you'd like to add a feature or improve performance, go for it and submit a pull request.

---

## 📄 License

MIT License
