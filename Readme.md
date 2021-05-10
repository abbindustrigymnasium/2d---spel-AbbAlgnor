# School Shooter
## Quick info
**_School shooter_** is a game where you eh... Shoot each other in school... 
But don't worry after some consulting with Joakim we both felt that it would be a good idea to use paintball guns instead of real guns.... Cause ya know the whole school setting
## Big Plans
###### _Just a heads up, this is written just after doing some quick research. No actuall testing has been done yet._
### Plans in list (in priority)
1. 1v1 Multiplayer (because I think its cool)
2. 3D (I already have permission from Jeton to do 3d so might aswell do it)
3. Map based on "spetslokalen"
5. Clean low poly look
6. Cool weapons
### Plans in text
I'm planning on making a simple first person shooter located in "spetslokalen" since it has decent flow, good layout and has a chokepoint roughly in the middle of the map, also since I'm terrible at making maps from scratch. 
Its going to be 1v1 over LAN multiplayer, however if you have the port open it will most likely work on separate networks.
The visual style is most likely going to be a clean low poly look with baked lighting. 
The gameplay is not really something I'm going to focus on, if it has a weapon you can shoot and decent movement, then I'm glad with that.
## Big Problems
### Problems in chronological order
1. Everything went well just until quite literally the day I was going to upload everything to GitHub. So I lost about 2 weeks worth of progress.
2. Weird performance issues where even a project without any scripts somehow uses 20ms of frametime on scripts.
3. I have now managed to roughly get back all the progress i lost, however now I somehow have managed to make everything jittery.
4. Multiplayer is confusing I think I have managed to get something working, however I'm unsure as to how well it will work with anything more complex than two cylinders that can move.
5. Maybe not a problem but I got sidetracked and made a 2D terrain generator. Took roughly 2 days to make. But it can generate roughly 50km of terrain with 10 vertices per meter within 20 seconds.
### Problems summarized
- Multiplayer is tricky.
- Jittery movement.
- Weird performance issues.
- Data loss.
- Easy to get sidetracked
## Big Solutions
All the problems listed have now been fixed. 
- Jittery movement was caused by having the camera update before moving the player causing it to be one frame behind.
- Sidetracked solved by realizing the deadline was closing in
- Data loss fixed by simply remaking everything that was lost. However I would say that it looks way better now at least.
- Multiplayer fixed by watching many hours of youtube while getting sidetracked and by pure chance finding a really good video about it. (sorry Joakim the video you sent me used unity's old networking thing. This game uses a brand new API that's not even fully released yet. However it works really good and is quite easy to use)
## Images
![Screenshot from 2021-05-10 22-03-13](https://user-images.githubusercontent.com/71272158/117717849-a0489e80-b1db-11eb-92cc-6730af64aea3.png)
