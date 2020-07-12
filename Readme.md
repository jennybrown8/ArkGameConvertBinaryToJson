# Ark Game Binary to Json Convertor

This C# app wraps the ArkSaveGameToolkit in a self-contained
executable that loads from the binary save file and writes
out to json.  All other processing happens in an associated
python script (see https://github.com/jennybrown8/ArkGameConvertToJsonAnalyzer
for the python code; these two repos work together).

I'm not super familiar with C# and kinda hacked this 
together out of code examples and some thoughtful testing.
It may not be flexible or reusable once anything
changes around it.

To produce the SavegameToolkit\*.dll files, I 
downloaded the ArkSaveGameToolkit source from their
github, loaded it up in Visual Studio 2019, and 
built the project (without any changes).  

Then I copied those DLLs into "supporting" in this project
to have a clean source for git; however, that isn't enough
to make it compile in Visual Studio.  For that, I placed
a copy of those DLLs into the bin/debug/\* folder and
manually added them to the project's build path.

Convincing the project to publish a statically linked exe
as a single file took a bit of digging around in menus.
I found it eventually.  It looks weird at first because it
spits out a long list of dll files along the way, but then
it finally publishes the exe with everything included at
the end.

I have no idea if I got the .gitignore include/exclude quite 
right for a C# project going into git, so here's hoping.



