# SCP: Containment Breach - Unity Edition
SCP:Containment Breach (SCP:CB) - Unity Edition is a rewrite of the original game, SCP: Containment Breach, made by Regallis11.
My goal with this project is to rewrite the game in the Unity game engine for a couple of purposes:
  1) Better compatibility
  2) Easier coding
  3) Better support

The reasoning for each is as follows:

## Better compatibility
The Unity game engine is built on top of Mono, and Mono is well supported.
This means that if this game fails to build properly for Linux, the Windows build should run just fine through Wine/Proton.
Since I am using Unity 2020.3, it is a more modern version of the game engine that is in Long-term Support.
So only bug fixes for the engine are expected along with optimizations and nothing else.

## Easier coding
This ties into the first point, but is still a point onto itself.
C# is, in my honest opinion, easier to read than C++.
Another great point about using this language is ability to integrate outside Dynamic Link Libraries (DLLs) better than say Java,
or without the need for header files like C++.
The language can also drop down to a more "unsafe" mode as C# puts it, allowing for things like pointers.

## Better support
I am a heavy Linux user, mainly using Arch Linux for my desktop/laptop and Rocky Linux (based on Red Hat Enterprise Linux) for my server.
I treat Windows as a second-class OS on my computers, and as such I love it when games I love support Linux and use native binaries.
The problem I have with the original game and a popular remaking of it, is that they only support Windows (with the popular remake also supporting macOS.)
While it should, in theory, run nicely in Linux, I would have to either build it from source and hope that it will build for Linux,
or run it through Wine/Proton. Which has it's own problems as well.

# Requirements
  * IDE (of course)
    * JetBrains Rider (Recommended)
    * Visual Studio
      * Resharper would be a good recommended extension.
    * Visual Studio Code
      * Unity Extension should be used.
    * Any other IDE that supports C#/Unity.
  * Unity 2020.3.16
  
# How to build
Check [here](https://docs.unity3d.com/Manual/BuildSettings.html) on how to build Unity projects.

# Contributing
If you want to contribute, you can. The only requirements is that you follow the style of code that I write.
There is also to be no harrassing, doxxing, racism, or extremism (eg. Nazism) within the issues, code, or anywhere someone can type.
Doing so will result in a ban on contributing code to the project. See the `Code of Conduct` for more rules.
