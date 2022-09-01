# Rocket-Learning-using-ML-in-Unity
This project was part of the course "Computanional Intelligence", for my Computer Science studies. There is more detail in greek, on my official project report.

## Abstract

This project is about creating a simple environment, where the agent-rocket will start fron an initial base, at the center of the scene and will try to reach a target base, trying to overcome and learn each level's difficulties. The difficuclty on every scenario increases gradually. If the agent touches anything but the target base or if the time of exploration expires, the level will be restarted. After creating the simulation environment of each level and properly programming the rocket's movement system, with the help of **Reinforcement Learning** and ML-Agent toolkit that Unity offers, the rocket will get trained to successfully finish every level.


## RocketAgentLevel1Script.cs
A simple level, with one *LaunchPad* at the center of the screen and one *LandingPad* on the right side. 

In this script, the agent-rocket collects its environmental observations (speed, 3D position, z rotation, finishPad location) with `CollectObservations(VectorSensor sensor)`. The rocket uses the *RayCasting 3D* component that Unity offers from the editor, to track the objects of its environment, with 2 sets of Rays, one on top of it and one on the bottom. It also randomly selects an action (UpKey-Turbine activation, LeftKey-Left rotation, RightKey-Right rotation), which will reward it, according to the policy that has been given to it, using `OnActionReceived(float[] vectorAction)`. The function `Heuristic(float[] actionsOut)` is used for testing the rocket's movements with the keyboard, in combination with *OnActionReceived()*. Finally, the function `OnEpisodeBegin()` is used at the beginning of each episode, to reset every value before the episode ends and places the rocket at the initial position. On every new episode, the training cicle described before, starts again.

The core and logic on every other script is similar to this one. The changes that have been made, are about the policy of the agent, with different reward values for some actions. The placement of the target bases is different among the levels.

### RocketAgentLevel2Script.cs

Now the *LandingPad* is being spawned on a random position, on the right side with every successful episode.

### RocketAgentLevel3Script.cs

Now the *LandingPad* is being spawned on a random position, on the right or left side with every successful episode.

### RocketAgentLevel4Script.cs

Now the *LandingPad* is being spawned on a random position, on the right or left side, as well as up and down with every successful episode.

## RocketAgentLevel5Script.cs

This level was the hardest one for the agent to train. Its similar to level 4, but there are two target bases that have to be reached, in order the agent successfully finishes the episode.