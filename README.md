# DTDL V2 Interface Generator

## Introduction
This repo contains a simple tool that creates a set of DTDL V2 interfaces for creating interfaces in large numbers to support testing of tools and services that may be affected by large numbers of models.

## Pre-requisites 

Install Dotnet Core 3.1 or later https://dotnet.microsoft.com/download

## Usage

1. Clone this repo
1. Change to the interface-gen folder
1. Build the app with `dotnet build -o ./bin`
1. Run the application to get usage `.\bin\interface_gen.exe -h`

```
interface_gen:
  Program to generate a bunch of interfaces into a local file store

Usage:
  interface_gen [options]

Options:
  --repo-root <repo-root>          sets the root of the repository to generate into [default: c:\temp\registry]
  --num-create <num-create>        sets the total number of interfaces to create [default: 5]
  --max-failures <max-failures>    sets max invalid models that will be created [default: 25]
  --version                        Show version information
  -?, -h, --help                   Show help and usage information
```
### Examples

Generate 5000 interfaces into directory `c:\source\registry`

```
.\bin\interface_gen.exe --repo-root c:\source\registry --num-create 5000
```
