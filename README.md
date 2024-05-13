# What is this?
This is a game engine, what did you think it is?


# How does it work?

Well, the way this game engine is going to work is that it is going
to be split in two, the host application and the game engine DLL.
The host application is minimal, and just loads and runs the engine
DLL. Now, the way the engine will work is that you will be able to
reference the engine DLL in your mod projects, and create your own DLL which
will then go into the `CoreMods` folder, and be loaded by the game
engine. Alone, the game engine will do nothing, however with a mod
DLL, you can add anything to the game engine, and use the game 
engine's various low-level features to create meshes, gameplay, and
putting it all together you can make a game!


# Why should I care?

You shouldn't.


# How does this benefit me?

It gives you a platform to make a game.
