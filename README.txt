New Level Creation Steps
==========================
Nomenclature: [GameObject/Component/Property]

* Create (Copy) new Level and add to build settings
* Adjust [Wind] Direction
* Decide to [shipyard/ShipYard/allowCrew]
* Tweak procedual island generation params in [islands/*/]
* Add dict entry to [/Quest/Levels]
* Set [spawn//] locations and select [spawn/Port/enemy]. Optionally choose [spawn/Port/bubbleOnSpawn] for story
* Choose number of treasure for this level and add entry to [/Treasure/total] 
* Distribute this amount of treasure accross the level
* Press Play and check for errors
* Activate Level Button in "Title" scene & adjust its callback
* Add "title.campaign.details.bay.###(.description)" key.



General TODOs
===============
* Unity 2017.2 seems to have depreciated [Camera/GUILayer/]. Remove from all scenes.

* Try out new "Camera Zoomer" from 2017.2 (name?) instead of [Camera/PlayerFollower/]

* Change "game.bubble.quest.00" -> "game.bubble.quest.A" for all levels