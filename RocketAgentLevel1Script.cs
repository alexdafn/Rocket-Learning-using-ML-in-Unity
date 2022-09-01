using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class RocketAgentLevel1Script : Agent
{
    // Rocket movement parameters
    [SerializeField] float mainThrust = 350f;
    [SerializeField] float rotationThrust = 60f;
    Rigidbody rb;
    
    public GameObject finishPad;
    // Use of its 3D coordinates so the rest objects will be placed in space according to it
    public GameObject launchPad;

    // Used to end the episode of the training
    bool episodeDoneFlag=false;
    bool rocketLandedFlag=false;

    // LandingPosition() 
    public bool landingPadIsOnRight=true;

    // UIScript.cs
    public Text rewardText;

    // Initialization of the training
    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Collects the agent's observation from its environment
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(rb.velocity);
        sensor.AddObservation(transform.position);
        sensor.AddObservation(transform.rotation.z);
        sensor.AddObservation(finishPad.transform.position);

    }

    // Action selected from the agent and its penalty/reward assignment
    public override void OnActionReceived(float[] vectorAction)
    {
        float thrustingAction = vectorAction[0];
        float rotationAction = vectorAction[1];

        // Possible actions for the turbine
        if(thrustingAction==1){rb.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime); print("thrust");}
        if(thrustingAction==0){rb.AddRelativeForce(Vector3.up * 0 * Time.deltaTime); print("not thrusting"); AddReward(-0.0001f);} // If no action is taken, penalize the agent

        // Possible actions for the rotation of the rocket
        if(rotationAction==1) // Left rotation
        {
            ApplyRotation(rotationThrust); 
            print("left rotation");
            if (!landingPadIsOnRight) // When LandingPad is on the left
            {
                AddReward(0.001f); // Reward when the agent goes left
                
            }else
            {
                AddReward(-0.001f); // Penalty when the agent goes left
            }
        }

        if(rotationAction==2) // Right rotation
        {
            ApplyRotation(-rotationThrust); 
            print("right rotation");
            if (landingPadIsOnRight) // When LandingPad is on the right
            {
                AddReward(0.001f) ; // Reward when the agent goes right
            }else
            {
                AddReward(-0.001f); // Penalty when the agent goes right
            }
        }
        if(rotationAction==0){ApplyRotation(0); print("no rotation");} // No action

        // Penalty when the agent gets past the x coordinate of the LandinPad
        if (landingPadIsOnRight)
        {
            if (transform.position.x>finishPad.transform.position.x+3.5f) // If agent goes past WHOLE LandingPad
            {
                print("works: right");
                AddReward(-0.0001f);
            }
        }else
        {
            if (transform.position.x<finishPad.transform.position.x-3.5f) // If agent goes past WHOLE LandingPad
            {
                AddReward(-0.0001f);
            }
        }

        // Penalty if the agent gets our of boundries and restarts the episode
        if((transform.position.y>launchPad.transform.position.y+22) || (transform.position.y<launchPad.transform.position.y-5) || (transform.position.x>launchPad.transform.position.x+55) || transform.position.x<launchPad.transform.position.x-45)
        {
            AddReward(-1f);
            EndEpisode();
        }

        // Big reward if the agent reaches the FinishPad
        if(rocketLandedFlag)
        {
            AddReward(5f);
            EndEpisode();
        }

        // Penalty if the agents hits on the ground/obstacle
        if(episodeDoneFlag)
        {
            AddReward(-1f);
            EndEpisode();
        }

        // Current reward of the episode display on screen
        rewardText.text=GetCumulativeReward().ToString("0.000");
    }
    
    // When not training, programemr can get control of the rocket's movement
    public override void Heuristic(float[] actionsOut)
    {
        bool isThrusting= Input.GetKey(KeyCode.Space);
        bool leftRotationgKey = Input.GetKey(KeyCode.A);
        bool rightRotationKey = Input.GetKey(KeyCode.D);

        if(isThrusting)
        {
            actionsOut[0]=1;
        }else
        {
            actionsOut[0]=0;
        }

        if(leftRotationgKey)
        {
            actionsOut[1]=1;
        }else if (rightRotationKey)
        {
            actionsOut[1]=2;
        }else
        {
            actionsOut[1]=0;
        }
    }

    // Resets the values, at the beginning of each episode
    public override void OnEpisodeBegin()
    {
        episodeDoneFlag=false;
        rocketLandedFlag=false;
        // Reposition of the rocket on top of the LaunchPad
        transform.position= new Vector3(launchPad.transform.position.x,launchPad.transform.position.y+1.7f,launchPad.transform.position.z);
        transform.rotation= Quaternion.Euler(0,0,0);
        rb.velocity=new Vector3(0,0,0); // Reset at 0 of the rocket speed
        
        LandingPadPosition(); 
    }
    
    // Calculation of the relative position of the LandingPad compared to LaunchPad
    private void LandingPadPosition()
    {
        if(finishPad.transform.position.x>launchPad.transform.position.x) 
        {
            landingPadIsOnRight=true;
        }else
        {
            landingPadIsOnRight=false;
        }
    }

    // Rocket Rotation
    void ApplyRotation(float rotationThisFrame)
    {
        rb.freezeRotation = true; // freezing rotation
        transform.Rotate(Vector3.forward * rotationThisFrame * Time.deltaTime);
        rb.freezeRotation = false; // UNfreezing rotation
    }

    // Collision handling of the agent, with the other objects in scene
    private void OnCollisionEnter(Collision other) 
    {
        switch(other.gameObject.tag) 
        {
            case "Friendly":
                Debug.Log("This thing is friendly");
                break;
            case "Finish":
                Debug.Log("Congratulations, you finished!");
                rocketLandedFlag=true;
                break;
            default:
                episodeDoneFlag=true;
                break;
        }
    }
}
