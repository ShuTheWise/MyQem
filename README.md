# MyQem
Mesh simplification using Quadric Error Metrics written in C#/.NET.

## Purpose

This program can be used to simplify a triangular mesh to target number of triangles or using quality multiplier, the relationship is as follows:

`Quality * Input model tris count = Target tris count`

Input mesh has to be in STL file format (.stl). Output mesh will also be in that format.

## Installation
This is a dotnet global tool, for installation simply type in terminal:

`dotnet tool install --global qem`

You need to have .NET Core installed on your computer for this to work: https://dotnet.microsoft.com/download

More information about installing and uninstalling global tools:
https://docs.microsoft.com/pl-pl/dotnet/core/tools/global-tools

This program is also included in nuget repository:
https://www.nuget.org/packages/qem/

For offline installation the nuget package is also included as a release.

#### Additional information
Compiled and tested using:
- Visual Studio 2019 v16
- .NET Framework v4.6.1
- Windows 10

## Usage

In the terminal type:

`qem <input file path> <output file path> -q <quality>`

or

`qem <input file path> <output file path> -t <target triangle count>`

- input and output files should have .stl extension
- quality has to be between 0.0 and 1.0
- target tris count cannot exceed input model's tris count

If you do not specify neither quality nor target triangle count, the original model will be simplified by a quality factor of 0.5:

`qem <input file path> <output file path>`

For more information type:

`qem help`
