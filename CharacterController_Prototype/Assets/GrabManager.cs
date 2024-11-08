using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Controls the arms for the pole grab mechanic
public class GrabManager : MonoBehaviour
{
    PlayerInput playerControls;

    public CarController carController;
    public vfxHandler vfxHandler;

    public GameObject leftArmDefault;   //left arm in default state
    public GameObject leftArmGrab;      //left arm in grab state
    public GameObject rightArmDefault;  //right arm in default state
    public GameObject rightArmGrab;     //right arm in grab state

    public Transform lookatRef;         //Transform reference for lookat while pole grabbing
    public Transform stationaryCamRef;  //Stationary cam reference for when grabbing the pole

    private GameObject activeHand;      //Which hand is currently active

    private InputAction leftGrab;       //Left trigger pressed
    private InputAction rightGrab;      //Right trigger presseed


    // Input manager setup
    void Start()
    {
        playerControls = new PlayerInput();
        playerControls.Enable();

        leftGrab = playerControls.Gameplay.PoleGrabLeft;
        leftGrab.Enable();
        leftGrab.performed += StartLeftPoleGrabInput;
        leftGrab.canceled += ExitLeftPoleGrabInput;

        rightGrab = playerControls.Gameplay.PoleGrabRight;
        rightGrab.Enable();
        rightGrab.performed += StartRightPoleGrabInput;
        rightGrab.canceled += ExitRightPoleGrabInput;
    }

    // Update is called once per frame
    void Update()
    {
        //If needed can add code that will run in update if one of the arm is active

        /*if (rightGrab.IsPressed() && activeHand == rightArmGrab)
        {
            //Debug.Log("R");
        }
        if (leftGrab.IsPressed() && activeHand == leftArmGrab)
        {
            //Debug.Log("L");
        }*/
    }

    //If no arm currently active, activate left arm
    private void StartLeftPoleGrab()
    {
        if (activeHand == null)
        {
            //Debug.Log("Start Left");
            leftArmDefault.SetActive(false);
            leftArmGrab.SetActive(true);

            activeHand = leftArmGrab;
        }
    }

    //deactivate left arm; enable other arm if its button is already pressed
    private void ExitLeftPoleGrab()
    {
        //Debug.Log("Exit Left");
        leftArmDefault.SetActive(true);
        leftArmGrab.SetActive(false);

        activeHand = null;
        if (rightGrab.IsPressed())
        {
            StartRightPoleGrab();
        }

        StopGrabPole();
    }

    //If no arm currently active, activate right arm
    private void StartRightPoleGrab()
    {
        if (activeHand == null)
        {
            //Debug.Log("Right");
            rightArmDefault.SetActive(false);
            rightArmGrab.SetActive(true);

            activeHand = rightArmGrab;
        }
    }

    //deactivate right arm; enable other arm if its button is already pressed
    private void ExitRightPoleGrab()
    {
        //Debug.Log("Exit Right");
        rightArmDefault.SetActive(true);
        rightArmGrab.SetActive(false);

        activeHand = null;
        if (leftGrab.IsPressed())
        {
            StartLeftPoleGrab();
        }

        StopGrabPole();
    }

    //Inititate the pole holding state
    public void GrabPole(Transform poleRef)
    {
        if (carController.cartState != CarController.CartState.PoleHolding)
        {
            Vector3 handPos = activeHand.GetComponentInChildren<ArmGrabCollider>().gameObject.transform.position;

            //Get the distance from the hand to the pole and move the car by that amount
            //Allows the hand to visualy hold on to the pole
            Vector2 DistToPole = new Vector2(poleRef.position.x, poleRef.position.z) - new Vector2(handPos.x, handPos.z);
            carController.transform.position += new Vector3(DistToPole.x, 0, DistToPole.y);


            lookatRef.gameObject.SetActive(true);
            //Set the lookat ref to the hand trigger
            lookatRef.position = activeHand.GetComponentInChildren<ArmGrabCollider>().gameObject.transform.position;

            //Save the camera position to the position it was when the pole was grabbed
            stationaryCamRef.position = carController.gameObject.transform.position;
            stationaryCamRef.rotation = carController.gameObject.transform.rotation;
            Camera.main.transform.parent = stationaryCamRef;

            //Switch state to the pole holding
            carController.SwitchCartState(CarController.CartState.PoleHolding);
        }
    }

    //Leave the pole grabbing state
    private void StopGrabPole()
    {
        if (carController.cartState == CarController.CartState.PoleHolding)
        {
            //Reset camera to follow the car
            carController.transform.parent = null;
            Camera.main.transform.parent = carController.transform;
            Camera.main.transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles.x, carController.transform.rotation.eulerAngles.y, Camera.main.transform.rotation.eulerAngles.z);

            vfxHandler.PlayFireTrail(); // PLAY FIRE TRAIL (1.5 seconds long)

            lookatRef.gameObject.SetActive(false);
            carController.SwitchCartState(CarController.CartState.InCart);
        }
    }





    private void StartLeftPoleGrabInput(InputAction.CallbackContext context)
    {
        StartLeftPoleGrab();
    }
    private void ExitLeftPoleGrabInput(InputAction.CallbackContext context)
    {
        ExitLeftPoleGrab();
    }
    private void StartRightPoleGrabInput(InputAction.CallbackContext context)
    {
        StartRightPoleGrab();
    }
    private void ExitRightPoleGrabInput(InputAction.CallbackContext context)
    {
        ExitRightPoleGrab();
    }
}
