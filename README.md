# Todo List (WPF)

A lightweight, native Windows To-Do list application built with C# and WPF (.NET).

## Features

- **Task Management**: Easily add and delete tasks.
- **Prioritization**: Assign High, Medium, or Low priorities to tasks. Tasks are automatically sorted so pending high-priority tasks stay at the top.
- **Customizable Colors**: Customize the background colors for each priority level via the built-in Settings menu.
- **Task Duration & Countdown Timer**: Set an estimated duration for a task and start a countdown timer. The timer can be paused, reset, or minimized to a small docked window in the corner of your screen.
- **Task Filtering**: Quickly filter the list to only show today's tasks.
- **Data Persistence**: Tasks and settings are automatically saved locally (`todos.json`, `settings.json`) so nothing is lost when the app is closed.

## Getting Started

### Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/download) (Version 10.0 or compatible)

### Running the App
1. Open your terminal or command prompt.
2. Navigate to the project directory:
   ```bash
   cd D:\Projects\todo-list
   ```
3. Run the application:
   ```bash
   dotnet run
   ```

## Usage

- **Add a Task**: Select a priority from the dropdown, optionally specify a duration using the hours/minutes stepper, type your task in the text box, and click **Add** (or press Enter).
- **Start a Timer**: Click the **▶** (Play) button on a task to open its countdown timer.
- **Dock Timer**: In the timer window, click **Minimize to Dock** to place a small, unobtrusive timer in the bottom-right corner of your screen. The size of this dock can be configured in the Settings.
- **Mark as Done**: Click the green **✓** button. Completed tasks will turn grey, have a strikethrough, and automatically move to the bottom of the list.
- **Delete Task**: Once marked as done, click the red **X** button to permanently remove the task.
- **Settings**: Click the **⚙** (Gear) icon in the top right to adjust dock sizes and priority colors.
