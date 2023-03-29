# DotNetCodeBaseAnalyzer

## Introduction
This repository contains the source code for the ".NET Code Base Analyzer" -- a tool for analysing a C#.NET code base. It works as a library or as a console application.

The tool can search a root directory for C#.NET solution, project and source code files, and create a graph representation of all the dependencies between those files. The tool also provides a set of analyzer rules, based on said graph, to detect issues, such as broken references etc.

## Features
The tool...
- ...can analyse a "code base root directory", and represent the code base as a graph of solutions, projects, and source code files. (TODO)
- ...allows users to look up the dependencies of, i.e. what is included in, a specific solution or project. (TODO)
- ...allows users to look up where a specific source code file or project is used/referenced. (TODO)
- ...provides analyzer rules to detect issues with the code base. (TODO)
- ...reports issues (errors and warnings) as console output. (TODO)
- ...reports issues (errors and warnings) as a JSON file dump. (TODO)

## Analyser Rules (TODO)
- Solutions must not reference non-existent projects. (Done)
- Projects must not reference non-existent projects. (Done)
- Projects within a solution can only reference projects which are also included in the solution. (TODO)
- Projects should be included in at least one solution. (TODO)
- Source code files should be included in at least one project. (TODO)
- Projects must have circular references. (TODO)