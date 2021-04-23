# Custom-Spawner (WIP)
 A system to replace the base game spawner of SCP:SL
 
[![Custom Spawner Thumbnail](https://img.youtube.com/vi/2gE_QsWnltM/0.jpg)](https://www.youtube.com/watch?v=2gE_QsWnltM)

https://www.youtube.com/watch?v=2gE_QsWnltM

This plugin requires a litte more effort than just dragging it in and calling it a day. There are 2 parts to this Server config and Plugin config, which will be described below. It is also likely going to have conflicts and not be outright compatible with several other plugins. Let me know which ones aren't compatible and which ones are in your testing and I'll add them in the list below.

# Server Config Setup
You need to go to your config_gameplay.txt file and change the team_respawn_queue to:

`team_respawn_queue: 55555555555555555555555555555555555555555555555555555555555555555555555`

This is done to ensure no one spawns at the start of the round and so it doesn't screw anything up.


# Plugin Config Setup
### 1. Go to your plugin config file and change the spawn queue to your server's needs. It is by default set up for a 40 slot server which will spawn all 7 SCPs. If your server isn't 40 slots or you don't want all 7 SCPs to spawn then you will have to change it.

| Number        | Role type         |
| ------------- |:-------------:|
| 4 | Class D |
| 3 | Scientist |
| 1 | Guard |
| 0 | SCP |

_**Once again it is brutally important to ensure that the total number is equal to your max player count**_

### 2. Change the other configs to your liking, bottom text is fine as it is but upper and discord invite will need to be changed.

-----

# List of incompatible plugins
1. Wait and Chill
