# Terminal Writing Application - System Architecture Diagram

## High-Level System Overview

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                        TERMINAL WRITING APPLICATION                         │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐        │
│  │   PROGRAM.CS    │    │   MAINWINDOW    │    │  PROJECTMANAGER │        │
│  │   (Entry Point) │───▶│   (Main UI)     │───▶│   (Service)     │        │
│  └─────────────────┘    └─────────────────┘    └─────────────────┘        │
│           │                       │                       │                │
│           │                       │                       │                │
│           ▼                       ▼                       ▼                │
│  ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐        │
│  │  TERMINAL.GUI   │    │ CHAPTERMANAGER  │    │   JSON FILES    │        │
│  │  (UI Framework) │    │   WINDOW        │    │   (Storage)     │        │
│  └─────────────────┘    └─────────────────┘    └─────────────────┘        │
│           │                       │                       │                │
│           │                       │                       │                │
│           ▼                       ▼                       ▼                │
│  ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐        │
│  │  COLOR SCHEME   │    │ CHAPTEREDITOR   │    │   DATA MODELS   │        │
│  │  (Theme)        │    │   WINDOW        │    │   (Project,     │        │
│  └─────────────────┘    └─────────────────┘    │    Chapter)     │        │
│                                                 └─────────────────┘        │
└─────────────────────────────────────────────────────────────────────────────┘
```

## Detailed Component Architecture

### 1. Application Entry Point (Program.cs)
```
┌─────────────────────────────────────────────────────────────────────────────┐
│                              PROGRAM.CS                                     │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐        │
│  │   INITIALIZE    │    │   SETUP THEME   │    │   CREATE        │        │
│  │  TERMINAL.GUI   │───▶│   (White/Black) │───▶│   SERVICES      │        │
│  └─────────────────┘    └─────────────────┘    └─────────────────┘        │
│           │                       │                       │                │
│           │                       │                       │                │
│           ▼                       ▼                       ▼                │
│  ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐        │
│  │   CREATE MAIN   │    │   RUN MAIN      │    │   CLEANUP       │        │
│  │   WINDOW        │───▶│   APPLICATION   │───▶│   ON EXIT       │        │
│  └─────────────────┘    └─────────────────┘    └─────────────────┘        │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

### 2. User Interface Layer
```
┌─────────────────────────────────────────────────────────────────────────────┐
│                              UI LAYER                                      │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌─────────────────────────────────────────────────────────────────────────┐ │
│  │                           MAIN WINDOW                                  │ │
│  │  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌─────────────┐      │ │
│  │  │   HEADER    │ │ PROJECT     │ │   ACTIONS   │ │   STATUS    │      │ │
│  │  │             │ │   LIST      │ │   BUTTONS   │ │    BAR      │      │ │
│  │  └─────────────┘ └─────────────┘ └─────────────┘ └─────────────┘      │ │
│  └─────────────────────────────────────────────────────────────────────────┘ │
│                                    │                                        │
│                                    ▼                                        │
│  ┌─────────────────────────────────────────────────────────────────────────┐ │
│  │                        CHAPTER MANAGER                                 │ │
│  │  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌─────────────┐      │ │
│  │  │   CHAPTER   │ │   ADD       │ │   EDIT      │ │   DELETE    │      │ │
│  │  │    LIST     │ │  BUTTON     │ │  BUTTON     │ │  BUTTON     │      │ │
│  │  └─────────────┘ └─────────────┘ └─────────────┘ └─────────────┘      │ │
│  └─────────────────────────────────────────────────────────────────────────┘ │
│                                    │                                        │
│                                    ▼                                        │
│  ┌─────────────────────────────────────────────────────────────────────────┐ │
│  │                        CHAPTER EDITOR                                  │ │
│  │  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌─────────────┐      │ │
│  │  │   TITLE     │ │   CONTENT   │ │   WORD      │ │   SAVE      │      │ │
│  │  │   FIELD     │ │   EDITOR    │ │   COUNT     │ │  BUTTON     │      │ │
│  │  └─────────────┘ └─────────────┘ └─────────────┘ └─────────────┘      │ │
│  └─────────────────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────────────────┘
```

### 3. Service Layer
```
┌─────────────────────────────────────────────────────────────────────────────┐
│                            SERVICE LAYER                                   │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌─────────────────────────────────────────────────────────────────────────┐ │
│  │                        PROJECT MANAGER                                 │ │
│  │                                                                         │ │
│  │  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌─────────────┐      │ │
│  │  │   CREATE    │ │    LOAD     │ │    SAVE     │ │   DELETE    │      │ │
│  │  │  PROJECT    │ │  PROJECT    │ │  PROJECT    │ │  PROJECT    │      │ │
│  │  └─────────────┘ └─────────────┘ └─────────────┘ └─────────────┘      │ │
│  │                                                                         │ │
│  │  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌─────────────┐      │ │
│  │  │    ADD      │ │   UPDATE    │ │   DELETE    │ │  REORDER    │      │ │
│  │  │  CHAPTER    │ │  CHAPTER    │ │  CHAPTER    │ │  CHAPTERS   │      │ │
│  │  └─────────────┘ └─────────────┘ └─────────────┘ └─────────────┘      │ │
│  └─────────────────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────────────────┘
```

### 4. Data Layer
```
┌─────────────────────────────────────────────────────────────────────────────┐
│                              DATA LAYER                                    │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌─────────────────────────────────────────────────────────────────────────┐ │
│  │                           DATA MODELS                                  │ │
│  │                                                                         │ │
│  │  ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐    │ │
│  │  │     PROJECT     │    │     CHAPTER     │    │ PROJECTSETTINGS │    │ │
│  │  │                 │    │                 │    │                 │    │ │
│  │  │ • Id            │    │ • Id            │    │ • AutoSave      │    │ │
│  │  │ • Title         │    │ • Title         │    │ • Theme         │    │ │
│  │  │ • Description   │    │ • Order         │    │ • FontSize      │    │ │
│  │  │ • CreatedDate   │    │ • Content       │    │ • TabSize       │    │ │
│  │  │ • LastModified  │    │ • WordCount     │    │ • WordGoal      │    │ │
│  │  │ • Chapters      │    │ • CharacterCount│    │ • DistractionFree│    │ │
│  │  │ • Settings      │    │ • Notes         │    └─────────────────┘    │ │
│  │  └─────────────────┘    └─────────────────┘                            │ │
│  └─────────────────────────────────────────────────────────────────────────┘ │
│                                    │                                        │
│                                    ▼                                        │
│  ┌─────────────────────────────────────────────────────────────────────────┐ │
│  │                           STORAGE                                       │ │
│  │                                                                         │ │
│  │  ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐    │ │
│  │  │   JSON FILES    │    │   NEWTONSOFT    │    │   FILE SYSTEM   │    │ │
│  │  │                 │    │     JSON        │    │                 │    │ │
│  │  │ • Project.json  │    │ • Serialization │    │ • User Profile  │    │ │
│  │  │ • Chapter.json  │    │ • Deserialization│   │ • WritingApp    │    │ │
│  │  │ • Settings.json │    │ • Error Handling│   │ • Projects/      │    │ │
│  │  └─────────────────┘    └─────────────────┘    └─────────────────┘    │ │
│  └─────────────────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────────────────┘
```

### 5. Event Flow and Keyboard Shortcuts
```
┌─────────────────────────────────────────────────────────────────────────────┐
│                        EVENT FLOW & INTERACTION                            │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐        │
│  │   USER INPUT    │───▶│  KEYBOARD       │───▶│   ACTION        │        │
│  │                 │    │  SHORTCUTS      │    │   EXECUTION     │        │
 │  │ • Mouse Click   │    │                 │    │                 │        │
 │  │ • Key Press     │    │ • N/n = New     │    │ • Create Project│        │
 │  │ • Navigation    │    │ • O/o = Open    │    │ • Load Project  │        │
 │  └─────────────────┘    │ • C/c = Chapter │    │ • Edit Chapter  │        │
 │                         │ • A/a = Add     │    │ • Delete Chapter│        │
 │                         │ • E/e = Edit    │    │ • Save Changes  │        │
 │                         │ • D/d = Delete  │    │ • Exit App      │        │
 │                         │ • S/s = Settings│    └─────────────────┘        │
 │                         │ • H/h = Help    │                               │
 │                         │ • Q/q = Quit    │                               │
│                         └─────────────────┘                               │
│                                    │                                       │
│                                    ▼                                       │
│  ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐        │
│  │   UI UPDATE     │───▶│   DATA PERSIST  │───▶│   STATUS        │        │
│  │                 │    │                 │    │   FEEDBACK      │        │
│  │ • Refresh Lists │    │ • Save to JSON  │    │ • Success Msg   │        │
│  │ • Update Counts │    │ • Update Files  │    │ • Error Msg     │        │
│  │ • Show Dialogs  │    │ • Backup Data   │    │ • Progress Info │        │
│  └─────────────────┘    └─────────────────┘    └─────────────────┘        │
└─────────────────────────────────────────────────────────────────────────────┘
```

### 6. Technology Stack
```
┌─────────────────────────────────────────────────────────────────────────────┐
│                           TECHNOLOGY STACK                                 │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐        │
│  │   LANGUAGE      │    │   FRAMEWORK     │    │   UI LIBRARY    │        │
│  │                 │    │                 │    │                 │        │
│  │ • C# 12.0       │    │ • .NET 8.0      │    │ • Terminal.Gui  │        │
│  │ • Strongly      │    │ • Cross-Platform│    │ • TUI Framework │        │
│  │   Typed         │    │ • Modern        │    │ • Event-Driven  │        │
│  │ • OOP Support   │    │ • High Perf     │    │ • Rich Controls │        │
│  └─────────────────┘    └─────────────────┘    └─────────────────┘        │
│                                    │                                       │
│                                    ▼                                       │
│  ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐        │
│  │   DATA STORAGE  │    │   SERIALIZATION │    │   DEPENDENCY    │        │
│  │                 │    │                 │    │   INJECTION     │        │
│  │ • JSON Files    │    │ • Newtonsoft    │    │ • Microsoft     │        │
│  │ • File System   │    │   Json          │    │   Extensions    │        │
│  │ • User Profile  │    │ • LINQ          │    │ • DI Container  │        │
│  │ • Cross-Platform│    │ • Error Handling│    │ • Service Locator│       │
│  └─────────────────┘    └─────────────────┘    └─────────────────┘        │
└─────────────────────────────────────────────────────────────────────────────┘
```

## Key Design Principles

### 1. **Separation of Concerns**
- **UI Layer**: Handles user interaction and display
- **Service Layer**: Contains business logic
- **Data Layer**: Manages data persistence and models

### 2. **Event-Driven Architecture**
- Components communicate through events
- Loose coupling between modules
- Easy to extend and modify

### 3. **Modular Design**
- Each window is a separate class
- Services are independent and reusable
- Models are clean and focused

### 4. **Dual Interaction Model**
- **Mouse Clicks**: Traditional UI interaction
- **Keyboard Shortcuts**: Vim-style efficiency (case-insensitive)
- **Both work simultaneously**

### 5. **Data Persistence**
- JSON-based storage
- Automatic saving
- Cross-platform compatibility

## File Structure
```
WritingApp/
├── WritingApp.sln                 # Solution file
├── WritingApp/
│   ├── WritingApp.csproj          # Project file
│   ├── Program.cs                 # Entry point
│   ├── SimpleTest.cs              # Test utility
│   ├── Models/
│   │   ├── Project.cs             # Project data model
│   │   ├── Chapter.cs             # Chapter data model
│   │   ├── ProjectSettings.cs     # Settings model
│   │   └── ChapterNotes.cs        # Notes model
│   ├── Services/
│   │   └── ProjectManager.cs      # Business logic service
│   └── UI/
│       ├── MainWindow.cs          # Main application window
│       ├── ChapterManagerWindow.cs # Chapter management UI
│       └── ChapterEditorWindow.cs # Text editor UI
├── README.md                      # User documentation
├── CODE_EXPLANATION.md            # Technical documentation
└── run_writing_app.bat            # Windows launcher
```

## Color Scheme Configuration

The application is configured to use a **white-on-black** terminal theme:

```csharp
var normalColor = new Terminal.Gui.ColorScheme
{
    Normal = Terminal.Gui.Attribute.Make(Terminal.Gui.Color.White, Terminal.Gui.Color.Black),
    Focus = Terminal.Gui.Attribute.Make(Terminal.Gui.Color.Black, Terminal.Gui.Color.White),
    HotNormal = Terminal.Gui.Attribute.Make(Terminal.Gui.Color.BrightYellow, Terminal.Gui.Color.Black),
    HotFocus = Terminal.Gui.Attribute.Make(Terminal.Gui.Color.BrightYellow, Terminal.Gui.Color.White)
};
```

**Expected Colors:**
- **Normal Text**: White text on black background
- **Focused Elements**: Black text on white background
- **Hot Keys**: Bright yellow text on black background
- **Focused Hot Keys**: Bright yellow text on white background

### Color Scheme Implementation

The color scheme is applied at multiple levels to ensure consistent theming:

1. **Global Level** (`Program.cs`): Sets the default color scheme for the entire application
2. **Window Level**: Each window explicitly sets its color scheme
3. **Component Level**: Child components inherit the color scheme from their parent windows

**Why the UI was blue and white before:**
- Terminal.Gui has default color schemes that override global settings
- Individual components need explicit color scheme assignment
- The blue/white theme was Terminal.Gui's default color scheme
- Our white-on-black theme now overrides these defaults at all levels
