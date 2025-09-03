using Terminal.Gui;
using writerbase.Services;
using writerbase.UI;

namespace writerbase;

/// <summary>
/// Main entry point for the Terminal Writing Application.
/// This class is responsible for:
/// 1. Initializing the Terminal.Gui framework
/// 2. Setting up the application theme and color scheme
/// 3. Creating the main services (ProjectManager)
/// 4. Starting the main user interface
/// 5. Handling application lifecycle and cleanup
/// </summary>
class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// This method is called by the .NET runtime when the application starts.
    /// </summary>
    /// <param name="args">Command line arguments (not used in this application)</param>
    static void Main(string[] args)
    {

        
        try
        {
            // =============================================================================
            // APPLICATION INITIALIZATION
            // =============================================================================
            // Terminal.Gui is the UI framework that provides terminal-based user interface
            // This initializes the terminal driver and sets up the screen
            Application.Init();
            
            // =============================================================================
            // THEME CONFIGURATION
            // =============================================================================
            // Configure the color scheme for a traditional terminal theme
            // Normal text: White text on black background
            // Focus text: Black text on white background (for selected items)
            
            // Set up the color scheme for white-on-black terminal theme
            var normalColor = new Terminal.Gui.ColorScheme
            {
                Normal = Terminal.Gui.Attribute.Make(Terminal.Gui.Color.White, Terminal.Gui.Color.Black),
                Focus = Terminal.Gui.Attribute.Make(Terminal.Gui.Color.Black, Terminal.Gui.Color.White),
                HotNormal = Terminal.Gui.Attribute.Make(Terminal.Gui.Color.BrightYellow, Terminal.Gui.Color.Black),
                HotFocus = Terminal.Gui.Attribute.Make(Terminal.Gui.Color.BrightYellow, Terminal.Gui.Color.White)
            };
            
            // Apply the color scheme globally to override Terminal.Gui defaults
            Application.Top.ColorScheme = normalColor;
            
            // =============================================================================
            // SERVICE INITIALIZATION
            // =============================================================================
            // Create the ProjectManager service that handles all project-related operations
            // This follows the Dependency Injection pattern - services are created first,
            // then passed to UI components that need them
            var projectManager = new ProjectManager();
            
            // =============================================================================
            // USER INTERFACE SETUP
            // =============================================================================
            // Create the main window and add it to the application
            // The MainWindow will handle project management and navigation to other screens
            var mainWindow = new MainWindow(projectManager);
            Application.Top.Add(mainWindow);
            
            // =============================================================================
            // APPLICATION EXECUTION
            // =============================================================================
            // Start the main application loop
            // This blocks until the user exits the application
            // Terminal.Gui handles all user input and screen updates
            Application.Run();
            
            // =============================================================================
            // CLEANUP
            // =============================================================================
            // Properly shut down Terminal.Gui to restore the terminal to its original state
            Application.Shutdown();
        }
        catch (Exception ex)
        {
            // =============================================================================
            // ERROR HANDLING
            // =============================================================================
            // If anything goes wrong during startup, display the error and exit
            // This ensures the user gets feedback if there's a problem
            Console.WriteLine($"Fatal error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            
            // Try to restore the terminal
            try
            {
                Application.Shutdown();
            }
            catch
            {
                // Ignore shutdown errors
            }
            
            Environment.Exit(1);
        }
    }
}
