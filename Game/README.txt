Orbital project 2022

A top-down 2D puzzle game inspired by Baba is You.

Rules:

Movement: Every level will include a "You" tile, the object (e.g. tomato) that is on top of the "You" tile causes every tomato in that level to be the player and can move around using wasd/arrow keys. However, if the tomato is on top of a tile or is cut, it cannot move even if it's a player.

Collision: The object acting as the player can push other objects around. Unless the object is next to a wall or is "Heavy".

Tiles: Tiles interact with objects on top of them. If the object is a player it won't be able to move, but can still be pushed off from another object as the player. Properties given to the object through tiles would not be given to objects while they are on top of a tile.

Win tile: Detects all objects on top of it (E.g. Win tile is a 1 x 2, with a tomato on the left and cheese on the right). Win tile will search entire level for another tomato on the left side of a cheese object, not including the objects on the win tile, and the player completes the level through this win tile. The win tile must be filled completely to detect a win (E.g. a 2x2 win tile must have 4 objects on top of it).

Heavy tile: An object on top of the Heavy tile causes all other objects of the same type to be "Heavy" and block player movement as if it were a wall. Heavy objects on top of tiles will not act as heavy and can be pushed off. If an object is both "You" and "Heavy" it can still move and push around other objects normally.

Knife: Knife is an object that cuts food objects that collides into knife. The knife can also be placed on top of tiles such as "You" or "Win" and functions the same as other objects.

Cut: Cut food cannot move around if they are "You", but can still be pushed around. If a cut food (e.g. cut tomato) is pushed onto a "You" tile, all tomatoes will still be "You", similar with other property tiles. However for win tiles, cut food will be counted as different. (E.g. the win tile has a normal tomato and cheese, and in the level is a cut tomato and normal cheese, it would not be a win).

Hot and Cold: An object on top of the Heavy or Cold tile causes all other objects of the same type to be "Hot" and "Cold" respectively. An object that is hot causes all other objects it collides into to be cooked. An object that is cold causes all cooked objects it collides into to be uncooked. When a hot and cold object collides, they destroy each other.

Cooked: Cooked objects similar to cut objects are a different for the win condition and behave normally with other tiles. However can still move if they are "You".

Undo: Press Z to undo the previous move. 
Restart: R to restart the level. 
Pause Menu: ESC to access Main menu, Level select, Quit