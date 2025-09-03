# writerbase - Terminal Writing Application

A distraction-free terminal-based writing application designed for novel and short story creation. Built with C# and .NET, featuring a modular architecture with strong OOP principles.

## Features

### Core Writing Features
- **Full-screen text editor** with word and character counting
- **Chapter management** with easy creation, editing, and organization
- **Project management** for organizing multiple writing projects
- **Auto-save functionality** to prevent data loss
- **Keyboard shortcuts** for efficient writing workflow

### Writing Tools
- **Real-time statistics** (word count, character count)
- **Chapter organization** with ordering and titles
- **Project templates** for different story types
- **Distraction-free mode** for focused writing
- **Export capabilities** (planned for future versions)

### User Interface
- **Terminal-based UI** using Terminal.Gui
- **Dark theme** for reduced eye strain
- **Modal dialogs** for project and chapter management
- **Status bars** with project information
- **Intuitive navigation** with mouse and keyboard support

## Prerequisites

### Required Software
- **.NET 8.0 SDK** or later
- **Windows, macOS, or Linux** (cross-platform)

### Installing .NET 8.0

#### Windows
1. Download the .NET 8.0 SDK from [Microsoft's website](https://dotnet.microsoft.com/download/dotnet/8.0)
2. Run the installer and follow the setup wizard
3. Verify installation by opening Command Prompt and running:
   ```
   dotnet --version
   ```

#### macOS
1. **Option 1**: Download from [Microsoft's website](https://dotnet.microsoft.com/download/dotnet/8.0)
2. **Option 2**: Use Homebrew:
   ```bash
   brew install dotnet
   ```
3. Verify installation:
   ```bash
   dotnet --version
   ```

#### Linux (Ubuntu/Debian)
1. Add Microsoft package repository:
   ```bash
   wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
   sudo dpkg -i packages-microsoft-prod.deb
   rm packages-microsoft-prod.deb
   ```
2. Install .NET SDK:
   ```bash
   sudo apt-get update
   sudo apt-get install -y dotnet-sdk-8.0
   ```
3. Verify installation:
   ```bash
   dotnet --version
   ```

## Installation and Setup

### 1. Clone or Download the Project
```bash
git clone <repository-url>
cd Terminal-App
```

### 2. Restore Dependencies
```bash
dotnet restore
```

### 3. Build the Application
```bash
dotnet build
```

### 4. Run the Application
```bash
dotnet run --project writerbase
```

## Usage Guide

### Starting the Application
1. Open your terminal/command prompt
2. Navigate to the project directory
3. Run: `dotnet run --project writerbase`

### Creating a New Project
1. Click "New Project" or press the corresponding button
2. Enter a project title (required)
3. Optionally add a description
4. Click "Create"

### Managing Chapters
1. Select a project from the list
2. Click "F2 - Open Project" or press F2
3. Use the interface to:
   - **Add Chapter**: Create new chapters (F1)
   - **Edit Chapter**: Open the full-screen editor (F2)
   - **Delete Chapter**: Remove chapters (F3)
   - **Close**: Return to main window (Esc)

### Writing in the Editor
1. Select a project and click "F2 - Open Project" or press F2
2. Select a chapter and click "F2 - Edit Chapter" or press F2
3. The full-screen editor will open with:
   - **Title field** at the top
   - **Content area** for writing
   - **Statistics bar** showing word/character counts
   - **Status bar** with save information

### Keyboard Shortcuts

#### Main Window
- **F1**: New Project
- **F2**: Open Project (Chapter Manager)
- **F3**: Help
- **Esc**: Quit
- **Enter**: Open selected project
- **↑/↓**: Navigate through projects

#### Chapter Manager (Open Project)
- **F1**: Add Chapter
- **F2**: Edit Chapter
- **F3**: Delete Chapter
- **Esc**: Close

#### Chapter Editor
- **F1**: Save
- **Esc**: Close (with save prompt)
- **Tab**: Indent text
- **Enter**: New paragraph

### Project Management
- **Projects are automatically saved** to your user directory
- **Location**: `~/writerbase/Projects/` (or `%USERPROFILE%\writerbase\Projects\` on Windows)
- **Format**: JSON files for easy backup and sharing
- **Auto-save**: Enabled by default (every 5 minutes)

## Project Structure

```
Terminal-App/
├── writerbase.sln              # Solution file
├── writerbase/
│   ├── writerbase.csproj       # Project file
│   ├── Program.cs              # Application entry point
│   ├── Models/                 # Data models
│   │   ├── Project.cs
│   │   └── Chapter.cs
│   ├── Services/               # Business logic
│   │   └── ProjectManager.cs
│   └── UI/                     # User interface
│       ├── MainWindow.cs
│       ├── OpenProjectWindow.cs
│       └── ChapterEditorWindow.cs
├── README.md                   # User documentation
├── SECURITY.md                 # Security documentation
├── system_architecture_diagram.md # System architecture
├── LICENSE                     # MIT License
└── .gitignore                  # Git ignore rules
```

## Architecture

### Design Principles
- **Modular Architecture**: Each component has a single responsibility
- **Object-Oriented Design**: Strong encapsulation, inheritance, and polymorphism
- **Event-Driven**: Components communicate through well-defined interfaces
- **Cross-Platform**: Runs on Windows, macOS, and Linux

### Key Components
- **ProjectManager**: Handles project lifecycle and persistence
- **MainWindow**: Primary application interface with project navigation
- **OpenProjectWindow**: Chapter organization and management interface
- **ChapterEditorWindow**: Full-screen writing editor

## Development

### Building from Source
```bash
# Clone the repository
git clone <repository-url>
cd Terminal-App

# Restore dependencies
dotnet restore

# Build in Debug mode
dotnet build

# Build in Release mode
dotnet build --configuration Release


```

### Adding New Features
1. Follow the existing architecture patterns
2. Add models in the `Models/` directory
3. Add services in the `Services/` directory
4. Add UI components in the `UI/` directory
5. Update the main window to integrate new features

## Troubleshooting

### Common Issues

#### "dotnet command not found"
- Ensure .NET 8.0 SDK is installed
- Restart your terminal after installation
- Verify installation with `dotnet --version`

#### "Terminal.Gui not found"
- Run `dotnet restore` to restore NuGet packages
- Check your internet connection
- Verify the project file includes the Terminal.Gui package

#### Application crashes on startup
- Check terminal size (minimum 80x24 characters recommended)
- Ensure your terminal supports the required features
- Try running in a different terminal emulator

#### Projects not saving
- Check write permissions in your user directory
- Ensure sufficient disk space
- Check for antivirus software blocking file operations

### Getting Help
- Check the console output for error messages
- Verify your .NET version: `dotnet --version`
- Ensure your terminal supports the required features
- Try running in a different terminal emulator

## Future Features

### Planned Enhancements
- **Character Management**: Character profiles and relationships
- **Plot Organization**: Scene breakdown and timeline tracking
- **Notes System**: Research notes and annotations
- **Export Options**: Markdown, PDF, DOCX export
- **Themes**: Multiple color schemes
- **Plugins**: Extensible architecture for custom features

### Contributing
This project follows good OOP principles and modular design. When contributing:
1. Follow existing code patterns
2. Add appropriate error handling
3. Include unit tests for new features
4. Update documentation
5. Ensure cross-platform compatibility

## License

This project is open source and available under the MIT License.

## Support

For issues, questions, or contributions:
1. Check the troubleshooting section
2. Review the architecture documentation
3. Create an issue with detailed information
4. Include your operating system and .NET version

---

**Happy Writing!** 📝

This terminal writing application provides a distraction-free environment for your creative writing projects, with all the tools you need to organize and write your stories effectively.
