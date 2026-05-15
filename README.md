# NYC DOT Truck Safety Game
An educational truck driving simulation game developed under the **New York City Department of Transportation (NYC DOT)**. Players drive a truck through a city environment and are challenged to navigate safely -- watching for pedestrians, obeying traffic laws, and managing blind spots. The goal is to raise awareness of real-world truck safety hazards in urban settings. 

The game places players behind the wheel of a large truck to simulate the difficulties of urban driving and demonstrate how limited visibility can create dangerous situations for pedestrians, and nearby vehicles. Features such as  traffic light violations, blind spot detection, and pedestrian collision feedback are designed to raise awareness of truck safety hazards commonly found on city streets. By experiencing these scenarios in an interactive environment, players gain a better understanding of safe driving practices and the importance of sharing the road responsibly in urban areas.

---
## Set Up Instruction
**Requirements**
- Unity Editor `6000.3.10f1` (Unity 6)
- Unity Hub

**Steps**
1. Clone the repository: `git clone https://github.com/KJohnson-prof/Truck_Safety_Game.git`
2. Open **Unity Hub** and click **Open Project**
3. Navigate to the cloned folder and select it
4. Once the project loads in the Unity Editor, open the main scene from the **Project** panel
5. Press the **Play** button to run the game in the Editor, or go to **File → Build and Run** to create a standalone build
---

# City Map
The game map was imported from the Unity Asset store. The asset store page can be found [here](https://assetstore.unity.com/packages/3d/environments/urban/demo-city-by-versatile-studio-mobile-friendly-269772). The scene's lighting settings were changed to give the map a daytime look. The puddles on the road were also removed because of their odd looking reflections with the new lighting settings.

# Traffic Lights
The entirety of the traffic lights system, including the code, was imported from the Unity Asset Store. the asset store page can be found [here](https://assetstore.unity.com/packages/tools/game-toolkits/traffic-lights-system-se-124136). The traffic light prefabs were placed on the map and the code was setup following the tutorial found on the store page.

# Pedestrian Spawner
Pedestrians were made to cross at intersections across the map. Some pedestrians obey the traffic light while others ignore it. The pedestrian prefabs and animations were imported from the Unity Asset Store. The asset store page can be found [here](https://assetstore.unity.com/packages/3d/characters/city-people-free-samples-260446). 
The code related to this can be found in [Pedestrian Controller](Assets/Scripts/PedestrianController.cs), [Pedestrian Spawner](Assets/Scripts/PedestrianSpawner.cs), and [Phone Person Controller](Assets/Scripts/PhonePersonController.cs).

# Blind Spot Detection
The game includes realistic truck blind spot zones on the sides and rear of the vehicle to simulate the limited visibility experienced by real truck drivers. If a pedestrian enters these blind spot areas and is hit by the truck, the game triggers a game over screen that displays a visual blind spot diagram to show where the collision occurred. This feature is designed to raise player awareness about the dangers of driving near large trucks in urban environments and to promote safer road-sharing behavior. The related code can be found in [Blind Spot Detector](Assets/Scripts/BlindSpotDetector.cs).

# Leaderboard System
The game includes an online leaderboard system that records and displays the top player scores. Player scores are determined by the total distance traveled while driving safely through the city. Avoiding collisions, pedestrians, and repeated traffic violations allows players to survive longer and achieve higher scores. The leaderboard is accessible from both the main menu and the game over screen, encouraging replayability and competition among players. The related code can be found in [Leaderboard](Assets/Scripts/Leaderboard.cs)

# Game Over
To trigger a game over certain conditions must be met. The player can either run three red lights, hit a pedestrian, or hit an obstacle on the map. When this happens, the game will stop and the game over UI appears.
This screen contains a restart button, a guide button, a leaderboard button, and a menu button. The restart button will reset the scene. The guide button shows safety tips for trucks.
The leaderboard displays the top 10 players and their scores. The menu button returns the player to the main menu. The related code can be found in [Game Over UI](Assets/Scripts/GameOverUI.cs) and [Leaderboard](Assets/Scripts/Leaderboard.cs).

# NPC Trucks/Cars
To simulate traffic, random car prefabs spawn at designated location every 5 to 15 seconds. The car moves forward for 60 units. The related code could be found in [Simple Car Mover](Assets/Scripts/SimpleCarMover.cs) and [Simple Car Spawner](Assets/Scripts/SimpleCarSpawner.cs). The prefabs were imported from the Unity Asset store and the asset store page can be found [here](https://assetstore.unity.com/packages/3d/environments/simplepoly-city-low-poly-assets-58899).

# Side Mirrors
The game is played from first-person inside the truck cab or third-person perspective, making gaming experience feel immersive and true to real truck driving. The side mirrors consist of two plane game objects that reflect a camera placed behind the truck, giving the player a functional rear view. This design helps players understand and experience real-world truck blind spots firsthand. The related code can be found in Assets/Truck Stuff/Assets/MirrorPlane.cs.

# Sound Effects
Sound effects were addded to the truck and NPC cars to make the game feel more immersive. Related code can be found in [Simple Car Mover](Assets/Scripts/SimpleCarMover.cs) and  [Truck Controller](Assets/Truck Stuff/Assets/Truck Controller.cs).

# Warning System
A warning system alerts the player when they run a red light. This gives players real-time feedback on unsafe driving behavior and counts toward the violation limit that triggers a game over. Related code can be found in [WarningTrigger](Assets/Scripts/WarningTrigger.cs).

# Main Menu
The main menu is a separate scene that exist as a hub of sorts. It contains the play button, that takes you to the main scene, a quit button, that exists the game, and a tutorial button, that displays the controls for the game. 
In this scene you can also find a Truck Safety Guide button, that gives helpful tips aboout driving the truck, and a button that leads to the leaderboard. The related code can be found in [Main Menu UI](Assets/Scripts/MainMenuUi.cs) and [Leaderboard Button](Assets/Scripts/LeaderboardButton.cs)

# Education Module
The Educational Module is set to show whenever the main menu scene plays. It is there to help spread information about the dangers of being careless around trucks.
The related code can be found in [Education Module](Assets/Scripts/EducationModule.cs).

# Online Leaderboard
The online leaderboard gives the game a competitive edge and adds replay value. The playerID is tied to an instance of a game and if past data is found the instance will automatically reconnect to the playerID.
A player can only hold one space on the leaderboard. The leaderboard is stored on unity's cloud and can be managed there, so a unity account is required.
The related code can be found in [Leaderoard](Assets/Scripts/Leaderboard.cs).
