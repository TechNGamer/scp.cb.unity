# Modding
If you wish to make a mod for this game, what you will need is an
IDE (I recommend Rider, but Visual Studio or any other C# IDE will work | Optional),
Unity 2020.3, `git`, [Blender](https://www.blender.org/) (optional) and some
knowledge on how the basics of modding. Let's start off with something easy,
making a new room for this game.

#Making a Room
## Preparing
Before we start doing anything, if you are making any new models/rooms, make sure
you have Blender installed along with Unity 2020.3. Make sure you also have Git installed
as you will need that (of course the other option it to just download the repo zip).

Once Git is installed, you need to then open the terminal, go to the location you want
to store these files. I do not recommend the game's folder, as that can lead to a mess.
Once there, type `git clone https://github.com/TechNGamer/scp.cb.unity` to proceed
downloading of the game's source code.

### Installing Blender
To start, install [Blender](https://www.blender.org/). If you are on Windows,
macOS, or know how to install and run programs on Linux, 
simply go to https://www.blender.org/download/. Another option is to install it
from [Steam](https://store.steampowered.com/app/365670/Blender/). Lastly, for Linux
you can do one of the following:

* `sudo apt install blender` (Debian)
* `sudo pacman -S blender` (Arch)
* `sudo zypper install blender` (OpenSUSE)
* `sudo yum install blender` (Fedora/RHEL-based distro's)

Side note: Another, but not recommended option, is to use ProBuilder within Unity.
It is not recommended as Unity is ***NOT*** meant as a 3D creation tool, Blender is.

## Importing from Blender
Once you've made the room in Blender, you will go to File -> Export -> FBX. Once you've
Exported it to an FBX file, drop it into Unity. Preferably into `/AssetBundle/Model`
for better organization. Drag it into the scene view and fix any issues that occured
during importing.

## Setting Up
Once you've fixed any import issues, you can start to hook up anything into the game.
As a bit of a reference, you can look at the other room prefabs of the game located at
`/Resource/Prefab/Rooms`.
