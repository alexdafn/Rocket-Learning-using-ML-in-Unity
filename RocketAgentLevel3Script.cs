using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class RocketAgentLevel3Script : Agent
{
    //παράμετροι, υπεύθυνει για την κίνηση
    [SerializeField] float mainThrust = 350f;
    [SerializeField] float rotationThrust = 60f;
    Rigidbody rb;
    
    public GameObject finishPad;
    //Χρήση των συντεταγμένων του ως σημείο αναφοράς για την σχετική τοποθέτηση των υπολοίπων στοιχείων
    //κατα την δημιουργία του κάθε επεισοδίου
    public GameObject launchPad;

    //Χρήση για τον τερματισμό του επεισοδίου
    bool episodeDoneFlag=false;
    bool rocketLandedFlag=false;

    //LandingPosition() 
    public bool landingPadIsOnRight=true;

    //gia UI
    public Text rewardText;

    // RandomLandingPadRepositionOnTheRight()
    private int randomNumber;

    //Κατα την αρχικοποίηση της εκπαίδευσης
    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    //Συλλογή παρατηρήσεων του περιβάλλοντος από τον πράκτορα
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(rb.velocity);
        sensor.AddObservation(transform.position);
        sensor.AddObservation(transform.rotation.z);
        sensor.AddObservation(finishPad.transform.position);

    }

    //Επιλογή δράσης απο τον πράκτορα και κατάλληλη απόδοση ποινών και ανταμοιβών
    public override void OnActionReceived(float[] vectorAction)
    {
        float thrustingAction = vectorAction[0];

        float rotationAction = vectorAction[1];

        //πιθανές δράσεις για τον κινητήρα
        if(thrustingAction==1){rb.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime); print("thrust");}
        if(thrustingAction==0){rb.AddRelativeForce(Vector3.up * 0 * Time.deltaTime); print("not thrusting"); AddReward(-0.0001f);}//Καμία δράση να τιμωρείται λίγο

        //πιθανές δράσεις για την περιστροφή του πυραύλου
        if(rotationAction==1)//περιστροφή προς τα αριστερά
        {
            ApplyRotation(rotationThrust); 
            print("left rotation");
            if (!landingPadIsOnRight)//Όταν είναι αριστερά το LandingPad
            {
                AddReward(0.002f);//Επιβράβευση όταν πηγαίνει προς τα αριστερά
                
            }else
            {
                AddReward(-0.002f);// Ποινή όταν πηγαίνει προς τα δεξιά
            }
        }

        if(rotationAction==2)//περιστροφή προς τα δεξιά
        {
            ApplyRotation(-rotationThrust); 
            print("right rotation");
            if (landingPadIsOnRight)//Όταν είναι δεξιά το LandingPad
            {
                AddReward(0.002f);//Επιβράβευση όταν πηγαίνει προς τα δεξιά
            }else
            {
                AddReward(-0.002f);// Ποινή όταν πηγαίνει προς τα αριστερά
            }
        
        }


        if(rotationAction==0){ApplyRotation(0); print("no rotation");} //Δε κάνει τίποτα αν επιλεγεί αυτή η δράση

        //Τιμωρία αν περνάει την χ συντεταγμένη του landingPad
        if (landingPadIsOnRight)
        {
            if (transform.position.x>finishPad.transform.position.x+3.5f)//Αν προσπεράσει ΟΛΟΚΛΗΡΟ το landingPad
            {
                print("doylevei deksia");
                AddReward(-0.0002f);
            }
        }else
        {
            if (transform.position.x<finishPad.transform.position.x-3.5f)//Αν προσπεράσει ΟΛΟΚΛΗΡΟ το landingPad
            {
                AddReward(-0.0002f);
            }
        }

        //Πρωτότυπο: if((transform.position.y>30) || (transform.position.y<-5) || (transform.position.x>50) || transform.position.x<-50)
        //Ποινή αν βγεί εκτός ορίων της πίστας ο πύραυλος και επανεκκίνηση επεισοδίου
        if((transform.position.y>launchPad.transform.position.y+30) || (transform.position.y<launchPad.transform.position.y-5) || (transform.position.x>launchPad.transform.position.x+55) || transform.position.x<launchPad.transform.position.x-45)
        {
            AddReward(-1f);
            EndEpisode();
        }

        //Επιβράβευση αν νικήσει= αν βρει τον στόχο ο πύραυλος, και επανεκκίνηση επεισοδίου
        if(rocketLandedFlag)
        {
            AddReward(8f);
            EndEpisode();
        }

        //Ποινή αν χτυπήσει σε μη φιλικό έδαφος/εμπόδιο/όχι στο landingPad
        if(episodeDoneFlag)
        {
            AddReward(-1f);
            EndEpisode();
        }

        //Επιδειξη του Reward του παρόντος επεισοδίου, στην οθόνη του παίχτη
        rewardText.text=GetCumulativeReward().ToString("0.000");
    }
    
    //Για δοκιμή των κινήσεων με το πάτημα πλήκτρων όταν δεν γίνεται η εκπαίδευση
    public override void Heuristic(float[] actionsOut)
    {

    bool isThrusting= Input.GetKey(KeyCode.Space);
    bool leftRotationgKey = Input.GetKey(KeyCode.A);
    bool rightRotationKey = Input.GetKey(KeyCode.D);

    if(isThrusting)
    {
        actionsOut[0]=1;
        //print("space pressed");
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
    

    //Οι κατάλληλες αρχικοποιήσεις στην αρχή κάθε επεισοδίου.
    public override void OnEpisodeBegin()
    {
        RandomLandingPadRepositionOnTheRight();//Level2 //Level 3

        episodeDoneFlag=false;
        rocketLandedFlag=false;
        //Σωστή τοποθέτηση του πυραύλου στην αρχή της πίστας
        transform.position= new Vector3(launchPad.transform.position.x,launchPad.transform.position.y+1.7f,launchPad.transform.position.z); //Πρωτότυπο: new Vector3(-5,1.7f,0);
        transform.rotation= Quaternion.Euler(0,0,0);
        rb.velocity=new Vector3(0,0,0); // μηδενισμός της ταχύτητας που έιχε ο πύραυλος λίγο πριν την λήξη του επεισοδίου
        
        LandingPadPosition();//Υπολογισμός σχετικής θέσης του landing με το launch pad
    }

    //Level2, Level3
    private void RandomLandingPadRepositionOnTheRight()
    {
        
        randomNumber= Random.Range(5,45);// Πρωτότυπο Random.Range(0,40); //LEVEL2
        //Level3 
        if(randomNumber%2==1)//Δεξιά μεριά
        {
            finishPad.transform.position= new Vector3(launchPad.transform.position.x+randomNumber,launchPad.transform.position.y,launchPad.transform.position.z);
        }else //Αριστερή μεριά
        {
            finishPad.transform.position= new Vector3(launchPad.transform.position.x-randomNumber,launchPad.transform.position.y,launchPad.transform.position.z);
        }
        //Πρωτότυπο Vector3(randomNumber,0,0);
        //LEVEL2finishPad.transform.position= new Vector3(launchPad.transform.position.x+randomNumber,launchPad.transform.position.y,launchPad.transform.position.z);
    }
    
    private void LandingPadPosition()
    {
        // Να έχω τον νου μου μήπως υπάρχει πρόβλημα με το = ή αν συμπέφτουν καμιά φορά
        if(finishPad.transform.position.x>launchPad.transform.position.x) //Πρωτότυπο if(finishPad.transform.position.x>-5)
        {
            landingPadIsOnRight=true;
        }else
        {
            landingPadIsOnRight=false;
        }
    }


    // Περιστροφή πυραύλου Movement.cs 
    void ApplyRotation(float rotationThisFrame)
    {
        rb.freezeRotation = true; // freezing rotation
        transform.Rotate(Vector3.forward * rotationThisFrame * Time.deltaTime);
        rb.freezeRotation = false; // UNfreezing rotation
    }

    //Διαχείριση συγκρούσεων του πράκτορα με τα διάφορα αντικείμενα του περιβάλλοντος
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
