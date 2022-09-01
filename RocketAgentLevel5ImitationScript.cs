using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class RocketAgentLevel5ImitationScript : Agent
{
    //παράμετροι, υπεύθυνει για την κίνηση
    [SerializeField] float mainThrust = 350f;
    [SerializeField] float rotationThrust = 60f;
    Rigidbody rb;
    
    public GameObject finishPadRed;
    public GameObject finishPadGreen;
    //Χρήση των συντεταγμένων του ως σημείο αναφοράς για την σχετική τοποθέτηση των υπολοίπων στοιχείων
    //κατα την δημιουργία του κάθε επεισοδίου
    public GameObject launchPad;

    //Χρήση για τον τερματισμό του επεισοδίου
    bool episodeDoneFlag=false;
    bool rocketLandedRedFlag=false;
    bool rocketLandedGreenFlag=false;
    bool oneTimeFlag=false;

    //LandingPosition() 
    public bool landingPadIsOnRight=true;

    //gia UI
    public Text rewardText;
    public float episodeTimer;

    // RandomLandingPadRepositionOnTheRight()
    private int randomNumberRedX;
    private int randomNumberRedY;
    private int randomNumberGreenX;
    private int randomNumberGreenY;
    private float randomNumberRocket;

    //Level5 σχετικές θέσεις
    //FinishPad_First()
    private bool upLeft;
    private bool upRight;
    private bool downLeft;
    private bool downRight;

    //Για την βοήθεια υπολογισμού του χρόνου του κάθε επεισοδίου
    void Update()
    {
        episodeTimer+=Time.deltaTime;
    }
    
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
        sensor.AddObservation(finishPadRed.transform.position);
        sensor.AddObservation(finishPadGreen.transform.position);

    }

    //Επιλογή δράσης απο τον πράκτορα και κατάλληλη απόδοση ποινών και ανταμοιβών
    public override void OnActionReceived(float[] vectorAction)
    {
        float thrustingAction = vectorAction[0];

        float rotationAction = vectorAction[1];

        //πιθανές δράσεις για τον κινητήρα
        if(thrustingAction==1)
        {
            rb.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime); print("thrust");
            //if(episodeTimer<2) {AddReward(0.01f);}// για να δωθεί αρχικό κίνητρο να εξερευνήσει
            //if(episodeTimer>100) {AddReward(-0.001f);} // για να δωθεί κίνητρο να σταματήσει να πετάει σαν εκκρεμές

            //if(upLeft||upRight){AddReward(0.002f);}//Αν ο δευτερος στόχος είναι προς τα πάνω σε σχέση με τον πρώτο και πάει προς αυτόν
            //if(downLeft||downRight){AddReward(-0.002f);}//Αν ο δευτερος στόχος είναι προς τα κάτω σε σχέση με τον πρώτο και πάει αντίθετα από αυτόν
        }
        if(thrustingAction==0)//Καμία δράση να τιμωρείται λίγο
        {
            rb.AddRelativeForce(Vector3.up * 0 * Time.deltaTime);
            print("not thrusting");
            AddReward(-0.0001f);
            //if(downLeft||downRight){AddReward(0.002f);}//Αν ο δευτερος στόχος είναι προς τα κάτω σε σχέση με τον πρώτο και πάει προς αυτόν
            //if(upLeft||upRight){AddReward(-0.002f);}//Αν ο δευτερος στόχος είναι προς τα κάτω σε σχέση με τον πρώτο και πάει αντίθετα από αυτόν
        }

        //πιθανές δράσεις για την περιστροφή του πυραύλου
        if(rotationAction==1)//περιστροφή προς τα αριστερά
        {
            ApplyRotation(rotationThrust); 
            print("left rotation");
            //if(downLeft||upLeft){AddReward(0.002f);}//Αν ο δευτερος στόχος είναι προς τα αριστερά σε σχέση με τον πρώτο και πάει προς αυτόν
           // if(downRight||upRight){AddReward(-0.002f);}//Αν ο δευτερος στόχος είναι προς τα δεξιά σε σχέση με τον πρώτο και παει αντίθετα από αυτόν
            
        }

        if(rotationAction==2)//περιστροφή προς τα δεξιά
        {
            ApplyRotation(-rotationThrust); 
            print("right rotation");
            //if(downRight||upRight){AddReward(0.002f);}//Αν ο δευτερος στόχος είναι προς τα δεξιά σε σχέση με τον πρώτο και παει προς αυτόν
            //if(downLeft||upLeft){AddReward(-0.002f);}//Αν ο δευτερος στόχος είναι προς τα δεξιά σε σχέση με τον πρώτο και παει αντίθετα απο αυτόν
            
            /*Προσωρινή αφαίρεση, καθώς μπορεί να έχει και αριστερά και δεξιά landingPads
            if (landingPadIsOnRight)//Όταν είναι δεξιά το LandingPad
            {
                AddReward(0.002f);//Επιβράβευση όταν πηγαίνει προς τα δεξιά
            }else
            {
                AddReward(-0.002f);// Ποινή όταν πηγαίνει προς τα αριστερά
            }*/
        
        }


        if(rotationAction==0){ApplyRotation(0); print("no rotation");} //Δε κάνει τίποτα αν επιλεγεί αυτή η δράση

        //Τιμωρία αν περνάει την χ συντεταγμένη του landingPad
        /*Προσωρινή αφαίρεση, καθώς μπορεί να έχει και αριστερά και δεξιά landingPads
        if (landingPadIsOnRight)
        {
            if (transform.position.x>finishPadRed.transform.position.x+3.5f)//Αν προσπεράσει ΟΛΟΚΛΗΡΟ το landingPad
            {
                print("doylevei deksia");
                AddReward(-0.0002f);
            }
        }else
        {
            if (transform.position.x<finishPadRed.transform.position.x-3.5f)//Αν προσπεράσει ΟΛΟΚΛΗΡΟ το landingPad
            {
                AddReward(-0.0002f);
            }
        }*/

        //Πρωτότυπο: if((transform.position.y>60) || (transform.position.y<-5) || (transform.position.x>50) || transform.position.x<-50)
        //Ποινή αν βγεί εκτός ορίων της πίστας ο πύραυλος και επανεκκίνηση επεισοδίου
        if((transform.position.y>launchPad.transform.position.y+60) || (transform.position.y<launchPad.transform.position.y-5) || (transform.position.x>launchPad.transform.position.x+55) || transform.position.x<launchPad.transform.position.x-45)
        {
            AddReward(-1f);
            EndEpisode();
        }
        
        if((rocketLandedRedFlag || rocketLandedGreenFlag) && (oneTimeFlag))
        {
            oneTimeFlag=false;
            if(rocketLandedRedFlag){FinishPadRedFirst();}
            if(rocketLandedGreenFlag){FinishPadGreenFirst();}
            AddReward(8f);
        }

        //Επιβράβευση αν νικήσει= αν βρει τον στόχο ο πύραυλος, και επανεκκίνηση επεισοδίου
        if(rocketLandedRedFlag && rocketLandedGreenFlag)
        {
            AddReward(20f);
            EndEpisode();
        }

        //Ποινή αν χτυπήσει σε μη φιλικό έδαφος/εμπόδιο/όχι στο landingPad
        if(episodeDoneFlag)
        {
            AddReward(-1f);
            EndEpisode();
        }
        
        //μια φορά στην αρχή του επεισοδίου θα αποκρύψει την βάση
        if(episodeTimer>6f){launchPad.SetActive(false);}

        if(episodeTimer>120f){AddReward(-2f);EndEpisode();}

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
        finishPadRed.SetActive(true);
        finishPadGreen.SetActive(true);
        launchPad.SetActive(true);
        RandomLandingPadRepositionOnTheRight();//Level2 //Level 3

        episodeTimer=0;//για την διαφράφη του launchPad μετά απο 3 δευτερόλεπτα
        oneTimeFlag=true;
        episodeDoneFlag=false;
        rocketLandedRedFlag=false;
        rocketLandedGreenFlag=false;

        //Level5 σχετικές θέσεις
        //FinishPad_First()
        upLeft=false;
        upRight=false;
        downLeft=false;
        downRight=false;

        //Σωστή τοποθέτηση του πυραύλου στην αρχή της πίστας
        randomNumberRocket = Random.Range(-2,2);//Για τυχαία τοποθέτηση του πυράυλου στην αρχή
        transform.position= new Vector3(launchPad.transform.position.x+randomNumberRocket,launchPad.transform.position.y+1.7f,launchPad.transform.position.z); //Πρωτότυπο: new Vector3(-5,1.7f,0);
        transform.rotation= Quaternion.Euler(0,0,0);
        rb.velocity=new Vector3(0,0,0); // μηδενισμός της ταχύτητας που έιχε ο πύραυλος λίγο πριν την λήξη του επεισοδίου
        
        //LandingPadPosition();//Υπολογισμός σχετικής θέσης του landing με το launch pad
    }

    //Level2, Level3, Level4, Level5
    private void RandomLandingPadRepositionOnTheRight()
    {
        
        randomNumberRedX= Random.Range(5,45);// Πρωτότυπο Random.Range(0,40); //LEVEL2,
        randomNumberRedY= Random.Range(0,30);//Level4 για ύψος
        randomNumberGreenX= Random.Range(5,45);// Πρωτότυπο Random.Range(0,40); //LEVEL2,Level5
        randomNumberGreenY= Random.Range(0,30);//Level4 για ύψος,level5
        if(randomNumberRedX%2==1)//Τοποθέτηση του landingPad στα δεξιά
        {
            finishPadRed.transform.position= new Vector3(launchPad.transform.position.x+randomNumberRedX,launchPad.transform.position.y+randomNumberRedY,launchPad.transform.position.z);
        }else//Τοποθέτηση του landingPad στα αριστερά
        {
            finishPadRed.transform.position= new Vector3(launchPad.transform.position.x-randomNumberRedX,launchPad.transform.position.y+randomNumberRedY,launchPad.transform.position.z);
        }
        if(randomNumberGreenX%2==1)//Τοποθέτηση του landingPad στα δεξιά //Level5
        {
            finishPadGreen.transform.position= new Vector3(launchPad.transform.position.x+randomNumberGreenX,launchPad.transform.position.y+randomNumberGreenY,launchPad.transform.position.z);
        }else//Τοποθέτηση του landingPad στα αριστερά //Level5
        {
            finishPadGreen.transform.position= new Vector3(launchPad.transform.position.x-randomNumberGreenX,launchPad.transform.position.y+randomNumberGreenY,launchPad.transform.position.z);
        }
        
        //Πρωτότυπο Vector3(randomNumberRedX,0,0);
        //LEVEL2finishPad.transform.position= new Vector3(launchPad.transform.position.x+randomNumberRedX,launchPad.transform.position.y,launchPad.transform.position.z);
    }
    
    /*Προσωρινά ανενεργό
    private void LandingPadPosition()
    {
        // Να έχω τον νου μου μήπως υπάρχει πρόβλημα με το = ή αν συμπέφτουν καμιά φορά
        //if(finishPadRed.transform.position.x>launchPad.transform.position.x) //Πρωτότυπο 
        if(finishPadRed.transform.position.x>-5)
        {
            landingPadIsOnRight=true;
        }else
        {
            landingPadIsOnRight=false;
        }
    }*/



    private void FinishPadRedFirst()
    {
        
        if((finishPadRed.transform.position.x>finishPadGreen.transform.position.x) && (finishPadRed.transform.position.y<finishPadGreen.transform.position.y))
        {upLeft=true;}//Πανω και αριστερά
        else if((finishPadRed.transform.position.x<finishPadGreen.transform.position.x) && (finishPadRed.transform.position.y<finishPadGreen.transform.position.y))
        {upRight=true;}//Πάνω και δεξιά
        else if((finishPadRed.transform.position.x>finishPadGreen.transform.position.x) && (finishPadRed.transform.position.y>finishPadGreen.transform.position.y))
        {downLeft=true;}//Κάτω και αριστερά
        else if((finishPadRed.transform.position.x<finishPadGreen.transform.position.x) && (finishPadRed.transform.position.y>finishPadGreen.transform.position.y))
        {downRight=true;}//Κατω και δεξιά
        
        
        
    }

    private void FinishPadGreenFirst()
    {
        if((finishPadGreen.transform.position.x>finishPadRed.transform.position.x) && (finishPadGreen.transform.position.y<finishPadRed.transform.position.y))
        {upLeft=true;}//Πανω και αριστερά
        else if((finishPadGreen.transform.position.x<finishPadRed.transform.position.x) && (finishPadGreen.transform.position.y<finishPadRed.transform.position.y))
        {upRight=true;}//Πάνω και δεξιά
        else if((finishPadGreen.transform.position.x>finishPadRed.transform.position.x) && (finishPadGreen.transform.position.y>finishPadRed.transform.position.y))
        {downLeft=true;}//Κάτω και αριστερά
        else if((finishPadGreen.transform.position.x<finishPadRed.transform.position.x) && (finishPadGreen.transform.position.y>finishPadRed.transform.position.y))
        {downRight=true;}//Κατω και δεξιά
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