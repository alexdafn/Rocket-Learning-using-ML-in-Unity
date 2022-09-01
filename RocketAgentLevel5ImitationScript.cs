using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class RocketAgentLevel5ImitationScript : Agent
{
    // Rocket movement parameters
    [SerializeField] float mainThrust = 350f;
    [SerializeField] float rotationThrust = 60f;
    Rigidbody rb;
    
    public GameObject finishPadRed;
    public GameObject finishPadGreen; // Level5
    // Use of its 3D coordinates so the rest objects will be placed in space according to it
    public GameObject launchPad;

    // Used to end the episode of the training
    bool episodeDoneFlag=false;
    bool rocketLandedRedFlag=false;
    bool rocketLandedGreenFlag=false;
    bool oneTimeFlag=false;

    // LandingPosition() 
    public bool landingPadIsOnRight=true;

    // UIScript.cs
    public Text rewardText;
    public float episodeTimer;

    // RandomLandingPadReposition()
    private int randomNumberRedX;
    private int randomNumberRedY;
    private int randomNumberGreenX;
    private int randomNumberGreenY;
    private float randomNumberRocket;

    // Level5 relative position of the first LandingPad compared to the other
    // FinishPad_First()
    // USED for a non successful training, NOT IN USE
    private bool upLeft;
    private bool upRight;
    private bool downLeft;
    private bool downRight;

    // Timer for each episode
    void Update()
    {
        episodeTimer+=Time.deltaTime;
    }
    
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
        sensor.AddObservation(finishPadRed.transform.position);
        sensor.AddObservation(finishPadGreen.transform.position);
    }

    // Action selected from the agent and its penalty/reward assignment
    public override void OnActionReceived(float[] vectorAction)
    {
        float thrustingAction = vectorAction[0];

        float rotationAction = vectorAction[1];

        // Possible actions for the turbine
        if(thrustingAction==1)
        {
            rb.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime); print("thrust");
        }
        if(thrustingAction==0) // If no action is taken, penalize the agent
        {
            rb.AddRelativeForce(Vector3.up * 0 * Time.deltaTime);
            print("not thrusting");
            AddReward(-0.0001f);
        }

        // Possible actions for the rotation of the rocket
        if(rotationAction==1) // Left rotation
        {
            ApplyRotation(rotationThrust); 
            print("left rotation");  
        }

        if(rotationAction==2) // Right rotation
        {
            ApplyRotation(-rotationThrust); 
            print("right rotation");       
        }

        if(rotationAction==0){ApplyRotation(0); print("no rotation");} // No action

        // Penalty when the agent gets past the x coordinate of the LandinPad
        if((transform.position.y>launchPad.transform.position.y+60) || (transform.position.y<launchPad.transform.position.y-5) || (transform.position.x>launchPad.transform.position.x+55) || transform.position.x<launchPad.transform.position.x-45)
        {
            AddReward(-1f);
            EndEpisode();
        }
        
        // Checks if the first LandingPad has been found
        if((rocketLandedRedFlag || rocketLandedGreenFlag) && (oneTimeFlag))
        {
            oneTimeFlag=false;
            if(rocketLandedRedFlag){FinishPadRedFirst();}
            if(rocketLandedGreenFlag){FinishPadGreenFirst();}
            AddReward(8f);
        }

        // Big reward if the agent reaches the FinishPad
        if(rocketLandedRedFlag && rocketLandedGreenFlag)
        {
            AddReward(20f);
            EndEpisode();
        }

        // Penalty if the agents hits on the ground/obstacle
        if(episodeDoneFlag)
        {
            AddReward(-1f);
            EndEpisode();
        }
        
        // Hides the LaunchPad after a short period of time
        if(episodeTimer>6f){launchPad.SetActive(false);}

        // Max time limitation for the episode to end
        if(episodeTimer>120f){AddReward(-2f);EndEpisode();}

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
        finishPadRed.SetActive(true);
        finishPadGreen.SetActive(true);
        launchPad.SetActive(true);
        RandomLandingPadReposition(); //Level2 //Level3

        episodeTimer=0; // Deletion of LaunchPad after 3 seconds
        oneTimeFlag=true;
        episodeDoneFlag=false;
        rocketLandedRedFlag=false;
        rocketLandedGreenFlag=false;

        //Level5 relative positions
        //FinishPad_First()
        upLeft=false;
        upRight=false;
        downLeft=false;
        downRight=false;

        // Reposition of the rocket on top of the LaunchPad
        randomNumberRocket = Random.Range(-2,2); // Random reposition on top of the LaunchPad //Level5
        transform.position= new Vector3(launchPad.transform.position.x+randomNumberRocket,launchPad.transform.position.y+1.7f,launchPad.transform.position.z);
        transform.rotation= Quaternion.Euler(0,0,0);
        rb.velocity=new Vector3(0,0,0); // Reset at 0 of the rocket speed
    }

    //Level2, Level3, Level4, Level5
    private void RandomLandingPadReposition()
    {
        
        randomNumberRedX= Random.Range(5,45); // Level2
        randomNumberRedY= Random.Range(0,30); // Level4
        randomNumberGreenX= Random.Range(5,45); // Level2, Level5
        randomNumberGreenY= Random.Range(0,30); // Level4, Level5
        // Level3 
        if(randomNumberRedX%2==1)// Right side
        {
            finishPadRed.transform.position= new Vector3(launchPad.transform.position.x+randomNumberRedX,launchPad.transform.position.y+randomNumberRedY,launchPad.transform.position.z);
        }else // Left side
        {
            finishPadRed.transform.position= new Vector3(launchPad.transform.position.x-randomNumberRedX,launchPad.transform.position.y+randomNumberRedY,launchPad.transform.position.z);
        }
        // Level5
        if(randomNumberGreenX%2==1) // Right side
        {
            finishPadGreen.transform.position= new Vector3(launchPad.transform.position.x+randomNumberGreenX,launchPad.transform.position.y+randomNumberGreenY,launchPad.transform.position.z);
        }else // Left side
        {
            finishPadGreen.transform.position= new Vector3(launchPad.transform.position.x-randomNumberGreenX,launchPad.transform.position.y+randomNumberGreenY,launchPad.transform.position.z);
        }
    }

    private void FinishPadRedFirst()
    {
        if((finishPadRed.transform.position.x>finishPadGreen.transform.position.x) && (finishPadRed.transform.position.y<finishPadGreen.transform.position.y))
        {upLeft=true;} // UpLeft
        else if((finishPadRed.transform.position.x<finishPadGreen.transform.position.x) && (finishPadRed.transform.position.y<finishPadGreen.transform.position.y))
        {upRight=true;} // UpRight
        else if((finishPadRed.transform.position.x>finishPadGreen.transform.position.x) && (finishPadRed.transform.position.y>finishPadGreen.transform.position.y))
        {downLeft=true;} // DownLeft
        else if((finishPadRed.transform.position.x<finishPadGreen.transform.position.x) && (finishPadRed.transform.position.y>finishPadGreen.transform.position.y))
        {downRight=true;} // DownRight
    }

    private void FinishPadGreenFirst()
    {
        if((finishPadGreen.transform.position.x>finishPadRed.transform.position.x) && (finishPadGreen.transform.position.y<finishPadRed.transform.position.y))
        {upLeft=true;} // UpLeft
        else if((finishPadGreen.transform.position.x<finishPadRed.transform.position.x) && (finishPadGreen.transform.position.y<finishPadRed.transform.position.y))
        {upRight=true;} // UpRight
        else if((finishPadGreen.transform.position.x>finishPadRed.transform.position.x) && (finishPadGreen.transform.position.y>finishPadRed.transform.position.y))
        {downLeft=true;} // DownLeft
        else if((finishPadGreen.transform.position.x<finishPadRed.transform.position.x) && (finishPadGreen.transform.position.y>finishPadRed.transform.position.y))
        {downRight=true;} // DownRight
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
            case "FinishRed":
                //Debug.Log("Congratulations, you finished!");
                rocketLandedRedFlag=true;
                finishPadRed.SetActive(false);
                break;
            case "FinishGreen":
                //Debug.Log("Congratulations, you finished!");
                rocketLandedGreenFlag=true;
                finishPadGreen.SetActive(false);
                break;
            default:
                episodeDoneFlag=true;
                break;
        }
    }
}