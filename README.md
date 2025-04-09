# TooDeePlatformer

## What is this

This project is a C# Windows Forms Mario Maker like editor that I made and redid during my first and second years at the ETML (École Technique des Métiers de Lausanne) in 2021.

Unfortunaly, the driver to play the created levels was never created.

My original intentions making this app was to create a 2D platformer using Windows Forms. To make this work, I would have needed to create maps and such.

So, I decided to create an editor to create those maps and levels. First called MarioMakerExp and now TooDeePlatformerEditor.
It first started as a simple application for myself to something I wanted to be used by others and became a much more complex project than originaly intended.
Though keep in mind that it was only my first and second year as an apprentice Software Developper.

## What can it do

The editor can create maps (**Worlds**) with different levels (**Dimensions**).
Those dimensions can be filled with **Obstacles** with custom texture stored in the World's folder.
Aside obstacles, some **Special Objects** can be added which can have special properties, such as killing the player, teleporting them to another place or Dimension, making the player win or triggering the movement set of a **Folder**.
Folders contain groups of Obstacles and Special Objects and can have **Moves** assigned to them. Moves dictates how and where a Folder must move after being triggered.
New **Obstacle Types** and **Special Types** can be added directly to the world.

## How to test it

To open the editor, go to /TooDeePlatformer/TooDeePlatformerApp/Editor/TooDeePlatformerEditor.exe.
Two test worlds are there. I advise opening Test10.

## How to access the code

The code was made and compiled using Visual Studio 2019.
The solution can be found here : /TooDeePlatformer/MarioMakerExp/MarioMakerExp.sln.
To simply view the code, it is mostly contained within /TooDeePlatformer/MarioMakerExp/MarioMakerExp/TooDeePlatformer/Editor/.

## How to use the editor

When you open the Editor, you may either create a new world or open an already existing one. 
Worlds are stored in /TooDeePlatformer/TooDeePlatformerApp/Worlds/.

To add new **Obstacles** drag and release your pointer to select a zone.
Then right click the selected zone and click Add From Selection>Obstacle.
This will add the selected **Obstacle Type**. 
To change your selected Obstacle Type, go to the Tree View to the left and within the **Obstacle Types** node within the World node, select the type you want to add.

The same process is used for Special Types.
