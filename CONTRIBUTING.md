# Contributing Guide for Project `Console`

Thank you for considering contributing to the `Console` project! We appreciate your help in making this project better. This guide will walk you through the process of contributing to the project, including setting up your development environment, coding standards, and submitting your changes.

## Table of Contents

1. [Getting Started](#getting-started)
    - [Fork the Repository](#fork-the-repository)
    - [Clone the Repository](#clone-the-repository)

2. [Coding Standards](#coding-standards)
    - [C# Style Guide](#c-style-guide)
    - [New Command Implementations](#new-command-implementations)

3. [Submitting a Pull Request](#submitting-a-pull-request)
    - [Creating a Branch](#creating-a-branch)
    - [Committing Changes](#committing-changes)
    - [Pushing Changes](#pushing-changes)
    - [Opening a Pull Request](#opening-a-pull-request)

## Getting Started

### Fork the Repository

To contribute to the `Console` project, you'll need to fork the repository to your GitHub account. This will create a copy of the repository that you can freely modify without affecting the original project.

1. Navigate to the [Console repository](https://github.com/your-username/Console).
2. Click the "Fork" button in the upper right corner of the page.
3. After forking, you'll be taken to your own forked repository.

### Clone the Repository

Once you've forked the repository, you'll need to clone it to your local machine to make changes.

```bash
git clone https://github.com/your-username/Console.git
cd Console
```

Now you're ready to start coding!

## Coding Standards

### C# Style Guide

We follow the default C# coding standards for this project. Please make sure your code adheres to these guidelines to maintain a consistent codebase. You can refer to the [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions) for more details.

### New Command Implementations

When adding new command implementations to the `Console` project, please follow these steps:

1. Create a new file for your command under the appropriate subfolder within the `Commands/Builtins` directory. Choose a meaningful subfolder name that reflects the purpose of the command.
2. Inherit your new command class from `BaseBuiltinCommand`.
3. Use the `base.WriteLine()` method provided by `BaseBuiltinCommand` to output information within your command implementation.

Example file structure:
```
Commands/
└── Builtins/
    └── DesiredSubFolder/
        └── NewCommand.cs
```

Example code for a new command implementation:
```csharp
using System;

namespace Console.Commands.Builtins.DesiredSubFolder
{
    public class NewCommand : BaseBuiltinCommand
    {
        public override string Name => "NewCommand";
        public override string Description => "The description for my command.";

        public override string DocString => "My docstring";

        public override void Run(List<string> args, IConsole parent)
        {
            // Your command logic here
            base.WriteLine("Output information using base.WriteLine()");
        }
    }
}
```

## Submitting a Pull Request

### Creating a Branch

Before making changes, create a new branch for your work. This helps keep your changes isolated from the main codebase.

```bash
git checkout -b feature/your-feature-name
```

### Committing Changes

Commit your changes with clear and concise messages that describe the purpose of each commit.

```bash
git add .
git commit -m "Add new command implementation for DesiredSubFolder"
```

### Pushing Changes

Push your branch to your forked repository.

```bash
git push origin feature/your-feature-name
```

### Opening a Pull Request

1. Visit your forked repository on GitHub.
2. Click on the "Pull Requests" tab.
3. Click the "New Pull Request" button.
4. Select the base branch (usually `main` of the original repository) and your branch (`feature/your-feature-name`) for the compare.
5. Provide a meaningful title and description for your pull request.
6. Click "Create Pull Request."

A maintainer of the `Console` project will review your pull request, provide feedback, and work with you to merge your changes if they meet the project's standards.

Thank you for your contribution! Your effort helps make the `Console` project better for everyone. If you have any questions or need assistance, please feel free to reach out to us through the pull request or issue system. Happy coding!
