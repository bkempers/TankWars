Ben Kempers & Anish Narayanaswamy
Prof. Kopta
CS3500
April 9, 2021

README - Design and Features (PS8)

Our TankWars client (PS8) assignment helped to create the world that we were supposed to draw and be able to play on detailed in the PS8 assignment page on Canvas.
We kept our design fairly similar to the client that was provided to us in the Examples repository to make sure we could fully implement what was required to us and have it be as similar to the client as 
we could make it. Our TankWars design started by implementing the basic View controls and code that was needed to help draw, handle user inputs, allow the user to connect to a certain server with their name, and 
to update the model from the controller. We then implemented our Control which held all the code from conecting to the server, initial server handshake, and processing all data that was recieved from the server and updating
our model accordingly. Our Model held all of the world elements that would be needed for creating, drawing, and updating so the View could properly see what was happening while the game was taking place. Our DrawingPanel was
also a large portion of our code as it held the majority of what was being drawn on screen as the Model was being updated from the Control. The DrawingPanel changed the players view, drew the background world image as well as walls
around the map, the different tanks, projectiles, powerups, and beams that came from either the user or other clients on the server.

DESIGN DECISIONS:
VIEW - Our view portion held minimal code to help MVC work properly as well as seperate concerns from the other portions of the code. The view was responsible for handling user inputs, initial user connection to the server, as well as
holding the DrawingPanel to help draw things that were visable on screen.
CONTROL - Our control section was the main "brain" of our TankWars client as it connected to the server designated by the user, supplemented their name and the controls from their keyboard and mouse to be sent to the server to help control
the tank. It also was responsible for processing the incoming data from the server to update our model that held all of the components of the world.
MODEL - The model was responsible for holding all of the world's tanks, projectiles, walls, beams, powerups and the map background. This model was consistently being updated from the server data that was processed by the control portion.

FEATURES:
This TankWars client holds all of the required features that was detailed in the PS8 assignment page. From connecting to a designated server, allowing the user to input their name, drawing the game world with tanks, walls, background and other
view / world related objects. The user of our client will be able to move their tank with the WASD keys (up, left, down, right) to control where their tanks moves. To fire a projectile from your tank simple click on the left mouse button - if you
find a yellow start on the world -> that is a powerup which will fire a beam that will go through walls with the right mouse button. If you die the tank will be removed from the view until you respawn in a different location. The name of your
tank and the score of players defeated will show up at the bottom of your tank, with the health of your tank floating above your tank at all times.


----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
Ben Kempers & Anish Narayanaswamy
Prof. Kopta
CS3500
April 23, 2021

README - Design and Features (PS9)

For our TankWars server (PS9) assignment we created the internal logic for the world we drew in the previous assignment. Our design decisions were kept reletively simple as we expected this project to be very difficult and time 
consuming. The first week of our assignment we decided to lay a basic foundation for the logic we were going to write in the second week. The first week was soley spent on getting the server to read the settings XML file, accept clients,
send a new tank to the client, and start the eventual main event loop of recieving data from the clients. The second week of this assignment we focuses on the logic of the game world as well as polishing and making our server run 
as similarly as we could to the provided server we recieved. 

PROBLEMS:
Overall our server runs as expected, with the only problem we ran into and couldn't fix was the client sometimes firing two projectiles very rarley in quick sucessesion to each other. We tried to remedy this situation by giving the Tank (client)
the provided input - that either being main, alt, or none for the attack styles and then checking that input once we were updating the world on every frame. Other than this issue, our server behaves almost identically to the provided server
(in our opinion).

FEATURES:
The features of our game server are all features present in the 'normal' TankWars server provided to us. Tanks are allowed to fire projectiles that don't go through walls and damage any tank that gets into its way which then gets rid of that
projectile. If a tank were to roll over a powerup they are allowed 1 beam attack that goes through any wall and kills a tank on impact. Tanks aren't allowed to go past a wall but will 'teleport' to the opposite side of the map if the world isn't
blocked off by walls (like in Pac Man). Normal projectiles are also removed if they go past the game world. 


EXTRA GAME MODE (PowerUp Party!):
To implement our extra game mode either create a new XML element titled "Mode" in the settings XML file or change the gameMode variable at the top of the Server class to 'extra' [or vice-versa change back to 'basic']. Our extra game mode, titled
PowerUp Party!, increases the power of the powerups. This game mode gives two extra features to the beam special attack. If you have rolled over a powerup, simply right click on your mouse to randomly be given one of three special abilities.
1.) A beam attack identical to the one implemented in the normal TankWars game mode.
2.) You will have your health restored to maximum health.
3.) A permenant 2x speed boost (until your tank dies)
