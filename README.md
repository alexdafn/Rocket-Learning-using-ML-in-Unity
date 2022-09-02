# Rocket-Learning-using-ML-in-Unity
This project was part of the course "Computanional Intelligence", for my Computer Science studies. There is more detail in greek, on my official project report and short presentation.

[Rocket Learning Report.pdf](https://github.com/alexdafn/Rocket-Learning-using-ML-in-Unity/files/9474076/Rocket.Learning.Report.pdf)

[Rocket Learning Presentation.pptx](https://github.com/alexdafn/Rocket-Learning-using-ML-in-Unity/files/9474077/Rocket.Learning.Presentation.pptx)

## Abstract

This project is about creating a simple environment, where the agent-rocket will start fron an initial base, at the center of the scene. The main goal is to reach a target base, trying to overcome and learn each level's difficulties. The difficuclty on every scenario increases gradually. If the agent touches anything but the target base or if the time of exploration expires, the level will be restarted. After creating the simulation environment of each level and properly programming the rocket's movement system, with the help of **Reinforcement Learning** and ML-Agents toolkit that Unity offers, the rocket will get trained to successfully finish every level.

## RocketAgentLevel1Script.cs
A simple level, with one *LaunchPad* at the center of the screen and one *LandingPad* on the right side. Successfully trained.

In this script, the agent-rocket collects its environmental observations (speed, 3D position, z rotation, finishPad location) with `CollectObservations(VectorSensor sensor)`. The rocket uses the *RayCasting 3D* component that Unity offers from the editor, to track the objects of its environment, with 2 sets of Rays, one on top of it and one on the bottom. It also randomly selects an action (UpKey-Turbine activation, LeftKey-Left rotation, RightKey-Right rotation), which will reward it, according to the policy that has been given to it, using `OnActionReceived(float[] vectorAction)`. The function `Heuristic(float[] actionsOut)` is used for testing the rocket's movements with the keyboard, in combination with *OnActionReceived()*. Finally, the function `OnEpisodeBegin()` is used at the beginning of each episode, to reset every value before the episode ends and places the rocket at the initial position. On every new episode, the training cicle described before, starts again.

The core code and logic on every other script is similar to this one. The changes that have been made, are about the policy of the agent, with different reward values for some actions. The placement of the target bases is different among the levels.

### RocketAgentLevel2Script.cs

In this level, the *LandingPad* is being spawned on a random position, on the right side with every successful episode. Successfully trained.

### RocketAgentLevel3Script.cs

In this level, the *LandingPad* is being spawned on a random position, on the right or left side with every successful episode. Successfully trained.

### RocketAgentLevel4Script.cs

In this level, the *LandingPad* is being spawned on a random position, on the right or left side, as well as up and down with every successful episode. 

### RocketAgentLevel5Script.cs

This level was the hardest one for the agent to get trained. Its similar to level 4, but there are two target bases that have to be reached, in order the agent successfully finishes the episode. The agent needs more time to reach the bases, with a 70% success in the end. 

### RocketAgentLevel5ImitationScript.cs

The exact same level as level 5 but the Imitation Learning method was used to try to get better results. Unity offers the option for **Imitation Learning**, with the recording of the gameplay from a player, and then that record can be used to support the neural network and get faster to convergence. The results where similar to level 5.

### RocketAgentLevel6Script.cs

In this level, there is an obstacle on the bottom part of the *LandingPad*, making it harder for the agent to reach the target base from below. Successfully trained.

### RocketAgentLevel7Script.cs

Its similar to level 6, but the target base, on top of the obstacle is a lot smaller, making it harder to reach from certain angles. Successfully trained.

### RocketAgentLevel8Script.cs

Its similar to level 6, but there are 2 obstacle-cubes that spawn randomly in space and make it harder for the agent to reach the target base. Successfully trained.

### trainer_config.yaml

These are the settings that I had on my training file. I used **PPO** algorithm for training. After the training process, a neural network file was produced, which was placed on the agent with drag and drop in Unity's editor. This way the agent was able to play the level according to its training.

## DEMO 1: Levels 1-4

https://user-images.githubusercontent.com/32633615/188035003-24a401fc-a4ed-47aa-94fc-06651811e693.mp4

## DEMO 2: Level 5

https://user-images.githubusercontent.com/32633615/188035108-e8383654-042d-40d9-80c4-988c6cf24eba.mp4

## DEMO 3: Levels 6-8

https://user-images.githubusercontent.com/32633615/188035146-02cbd3ef-24f2-46be-96ad-a4cf2c9574fe.mp4
