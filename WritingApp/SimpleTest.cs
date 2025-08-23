using WritingApp.Models;
using WritingApp.Services;

namespace WritingApp;

/// <summary>
/// Simple test program to demonstrate the basic functionality
/// </summary>
public class SimpleTest
{
    public static void RunTest()
    {
        Console.WriteLine("=== Terminal Writing Application Test ===\n");
        
        try
        {
            // Create project manager
            var projectManager = new ProjectManager();
            Console.WriteLine("✓ ProjectManager created successfully");
            
            // Create a new project
            var project = projectManager.CreateProject("Test Novel", "A test novel for demonstration");
            Console.WriteLine($"✓ Created project: {project.Title}");
            Console.WriteLine($"  Description: {project.Description}");
            Console.WriteLine($"  ID: {project.Id}");
            Console.WriteLine($"  Created: {project.CreatedDate}");
            
            // Add some chapters
            var chapter1 = projectManager.AddChapter("1: Test1");
            Console.WriteLine($"✓ Added chapter: {chapter1.Title}");
            
            var chapter2 = projectManager.AddChapter("Ch2");
            Console.WriteLine($"✓ Added chapter: {chapter2.Title}");
            
            var chapter3 = projectManager.AddChapter("Ch3 test");
            Console.WriteLine($"✓ Added chapter: {chapter3.Title}");
            
            // Update chapter content
            projectManager.UpdateChapter(chapter1.Id, chapter1.Title, 
                "Test test test test test");
            Console.WriteLine("✓ Updated Chapter 1 with content");
            
            projectManager.UpdateChapter(chapter2.Id, chapter2.Title,
                "TEEEST");
            Console.WriteLine("✓ Updated Chapter 2 with content");

            projectManager.UpdateChapter(chapter3.Id, chapter3.Title,
                "Test3");
            Console.WriteLine("✓ Updated Chapter 3 with content");
            
            // Display project statistics
            var currentProject = projectManager.CurrentProject;
            Console.WriteLine($"\n=== Project Statistics ===");
            Console.WriteLine($"Title: {currentProject?.Title}");
            Console.WriteLine($"Total Chapters: {currentProject?.Chapters.Count}");
            Console.WriteLine($"Total Words: {currentProject?.TotalWordCount}");
            Console.WriteLine($"Total Characters: {currentProject?.TotalCharacterCount}");
            
            // List all chapters
            Console.WriteLine($"\n=== Chapters ===");
            if (currentProject?.Chapters != null)
            {
                foreach (var chapter in currentProject.Chapters.OrderBy(c => c.Order))
                {
                    Console.WriteLine($"  {chapter.Order}. {chapter.Title} ({chapter.WordCount} words)");
                }
            }
            
            // Test project persistence
            Console.WriteLine($"\n=== Testing Project Load===");
            var projectName = currentProject?.Title ?? "Test Novel";
            var loadedProject = projectManager.LoadProject(projectName);
            
            if (loadedProject != null)
            {
                Console.WriteLine($"✓ Successfully loaded project: {loadedProject.Title}");
                Console.WriteLine($"  Chapters: {loadedProject.Chapters.Count}");
                Console.WriteLine($"  Words: {loadedProject.TotalWordCount}");
            }
            else
            {
                Console.WriteLine("✗ Failed to load project");
            }
            
            Console.WriteLine($"\n=== Test Complete ===");
            Console.WriteLine("All basic functionality is working correctly!");
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Test failed with error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }
}
