# DotNetCodeBaseAnalyzer

## Introduction
This repository contains the source code for the "**.NET Code Base Analyzer**" -- a tool for analysing a C#.NET code base. It works as a library or as a console application.

The tool can search a root directory for C#.NET solution, project and source code files, and create a graph representation of all the dependencies between those files. The tool also provides a set of analyzer rules, based on said graph, to detect issues, such as broken references etc.

## Features
The tool...
- ...can analyse a "code base root directory", and represent the code base as a graph of solutions, projects, and source code files. (Done)
- ...allows users to look up the dependencies of, i.e. what is included in, a specific solution or project. (Done)
- ...allows users to look up where a specific source code file or project is used/referenced. (Done)
- ...provides analyzer rules to detect issues with the code base. (TODO)
- ...reports issues (errors and warnings) as console output. (Done)
- ...reports issues (errors and warnings) as a JSON file dump. (TODO)

## Analyser Rules (TODO)
- Solutions must not reference non-existent projects. (Done)
- Projects must not reference non-existent projects. (Done)
- Projects within a solution can only reference projects which are also included in the solution. (TODO)
- Projects should be included in at least one solution. (TODO)
- Source code files should be included in at least one project. (TODO)
- Projects must have circular references. (TODO)

## User Instruction
To use the "**.NET Code Base Analyzer**" console application, run CMD and call the EXE file followed by a command and necessary parameters.

```
C:\Tools\CodeBaseAnalyzer>CodeBaseAnalyzer.Cmd.exe analyze "C:\Data\Dev\MyCodeDirectory"
```

### Commands
Use the ***help*** command to get an overview of all available commands.

```
C:\Tools\CodeBaseAnalyzer>CodeBaseAnalyzer.Cmd.exe help
Available commands:
> help
  Displays helpful information on available commands.
> analyze
  Analyzes a code base, and displays all detected issues.
> check
  Analyses a code base or specific solution, and fails (returns a non-zero value) if there are any errors. This may be useful automated builds / CI pipelines.
> solution
  Displays information on a specific solution.
> project
  Displays information on a specific project.
> usages
  Finds and lists all usages of a source code file, i.e. projects and solutions which include the file.

Done.
```

### Parameters
Each command is generally followed by, ...
* first, **required** parameters in the right order, and...
* secondly, **optional (named)** parameters as key-value pairs.

```
C:\Tools\CodeBaseAnalyzer>CodeBaseAnalyzer.Cmd.exe check "C:\Data\Dev\MyCodeDirectory" --solution "src\MySolution.sln"
```

In the example above, the ***check*** command takes one required parameter, the ***root*** directory, and the optional (named) ***--solution*** parameter.

### Help Command
Use the ***help*** command with the *command* parameter to get info on a specific command and its parameters.

```
C:\Tools\CodeBaseAnalyzer>CodeBaseAnalyzer.Cmd.exe help --command analyze
> analyze
  Analyzes a code base root directory.
  Parameters:
  > root (required)
    The code base root directory.

Done.
```

### Help Option
Alternatively, use the ***--help*** or ***-h*** option with any command to get the same info on that command.

```
C:\Tools\CodeBaseAnalyzer>CodeBaseAnalyzer.Cmd.exe solution --help
> solution
  Displays information on a specific solution.
  Parameters:
  > root (required)
    The code base root directory.
  > file (required)
    The solution file full name, or partial name.
```


