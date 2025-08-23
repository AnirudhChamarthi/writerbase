# Security Documentation

## Overview
This document outlines the security measures implemented in the Terminal Writing Application to protect against common vulnerabilities and ensure safe operation.

## Security Measures Implemented

### 1. Path Traversal Protection
**Vulnerability**: Path traversal attacks could allow access to files outside the intended directory.
**Solution**: 
- Input validation and sanitization of project names
- Path normalization and validation
- Security checks to ensure file operations stay within the projects directory

```csharp
// Sanitize project name to prevent path traversal
var sanitizedName = SanitizeFileName(projectName);

// Additional security check: ensure the path is within our projects directory
var fullPath = Path.GetFullPath(filePath);
var projectsDirFullPath = Path.GetFullPath(_projectsDirectory);

if (!fullPath.StartsWith(projectsDirFullPath, StringComparison.OrdinalIgnoreCase))
{
    throw new SecurityException("Invalid project path detected");
}
```

### 2. Input Validation
**Vulnerability**: Malicious input could cause application crashes or security issues.
**Solution**:
- Length limits on all user inputs
- Null/empty validation
- Character sanitization for filenames

```csharp
// Validate input
if (string.IsNullOrWhiteSpace(title))
    throw new ArgumentException("Project title cannot be empty", nameof(title));
    
if (title.Length > 100)
    throw new ArgumentException("Project title cannot exceed 100 characters", nameof(title));
```

### 3. File Name Sanitization
**Vulnerability**: Invalid characters in filenames could cause file system errors.
**Solution**:
- Replace invalid filename characters with underscores
- Remove path traversal attempts (.., \, /)
- Limit filename length

```csharp
private static string SanitizeFileName(string fileName)
{
    // Remove or replace invalid filename characters
    var invalidChars = Path.GetInvalidFileNameChars();
    var sanitized = fileName;
    
    foreach (var invalidChar in invalidChars)
    {
        sanitized = sanitized.Replace(invalidChar, '_');
    }
    
    // Remove path traversal attempts
    sanitized = sanitized.Replace("..", "_");
    sanitized = sanitized.Replace("\\", "_");
    sanitized = sanitized.Replace("/", "_");
    
    return sanitized;
}
```

### 4. Data Storage Security
**Vulnerability**: Sensitive data could be exposed through file operations.
**Solution**:
- All user data is stored in the user's profile directory
- No sensitive information (passwords, keys, tokens) is stored
- JSON files contain only writing content and metadata

### 5. Error Handling
**Vulnerability**: Error messages could leak sensitive information.
**Solution**:
- Generic error messages for security-related failures
- Detailed logging for debugging (not exposed to users)
- Graceful degradation when possible

## Security Best Practices

### For Users
1. **Keep the application updated** to receive security patches
2. **Don't share project files** if they contain sensitive content
3. **Use strong file system permissions** on your projects directory
4. **Backup your work regularly** using external storage

### For Developers
1. **Input validation**: Always validate and sanitize user input
2. **Path security**: Use `Path.Combine()` and validate paths
3. **Error handling**: Don't expose sensitive information in error messages
4. **Dependencies**: Keep NuGet packages updated
5. **Code review**: Review security-related code changes carefully

## Known Limitations

### Current Limitations
- No encryption of project files (stored as plain JSON)
- No user authentication or access control
- No network security (local application only)

### Future Security Enhancements
- Optional project file encryption
- User authentication system
- Secure backup and sync features
- Audit logging for sensitive operations

## Reporting Security Issues

If you discover a security vulnerability in this application:

1. **Do not** create a public GitHub issue
2. **Do not** disclose the vulnerability publicly
3. **Email** the maintainer with details of the vulnerability
4. **Include** steps to reproduce the issue
5. **Wait** for acknowledgment and resolution

## Security Checklist

- [x] Input validation implemented
- [x] Path traversal protection
- [x] File name sanitization
- [x] Error handling without information disclosure
- [x] No sensitive data storage
- [x] User data isolation
- [x] Comprehensive .gitignore
- [x] Security documentation

## Dependencies Security

The application uses the following dependencies:
- **Terminal.Gui**: Terminal UI framework (MIT License)
- **Newtonsoft.Json**: JSON serialization (MIT License)
- **Microsoft.Data.Sqlite**: Database access (Apache 2.0 License)
- **Microsoft.Extensions.DependencyInjection**: DI container (Apache 2.0 License)

All dependencies are regularly updated and use permissive open-source licenses.

## Compliance

This application is designed for personal use and does not handle:
- Personal Identifiable Information (PII)
- Financial data
- Health information
- Government classified information

For enterprise use, additional security measures may be required.
