using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerHUD : MonoBehaviour {
    [SerializeField]
    private RectTransform thrusterFuelFill;
    [SerializeField]
    private GameObject scorecard;
    [SerializeField]
    private GameObject Textprefab;
    private GameObject scorecardTable;
    //Controllers to local Player
    private PlayerController playerController;
    private PlayerShoot playerShoot;
	
	void Start()
    {
        //disable scorecard
        scorecard.SetActive(false);
        //disable PauseMenu
        PauseGameUI.singleton.gameObject.SetActive(false);
    }
	// Update is called once per frame
	void Update () {
        //TODO remove 
        Debug.Log("Broadcasting " + OverrideNeworkDiscovery.singleton.broadcastData);


        //update thuster fuel level in UI
		SetFuelAmount(playerController.GetThrusterFuelAmount());

        //ScoreCard    
            
        scorecard.SetActive(Input.GetButton("Tab"));
        if (Input.GetButton("Tab")){
            updateScorecard();
        }

        //Pause Game
        if (Input.GetButtonDown("Cancel"))
            PauseGame();

	}

    private void updateScorecard()
    {
        if (scorecardTable == null)
        {

        }
       
    }

    private void PauseGame()
    {
        
        if (PauseGameUI.singleton == null)
        {
            Debug.Log("No Pause Menu");
            return;
        }
        
        //Start Pause Menu
        PauseGameUI.singleton.Pause(CompleteResume);

        //Disable HUD
        gameObject.SetActive(false);

        //Stop PlayerMovement
        playerController.enabled = false;

        //StopPlayerShooting
        playerShoot.enabled = false;

    }

    public void CompleteResume()
    {
        //Enable HUD
        gameObject.SetActive(true);


        //Start Player movement
        playerController.enabled = true;


        //Start Player Shooting
        playerShoot.enabled = true;

    }


    // Called when player is Instantiated by PlayerSetup
    public void SetPlayerControllers(PlayerController _controller,PlayerShoot _playerShoot)
    {
        if (_controller == null)
            Debug.Log("No PlayerController");
        if (_playerShoot == null)
            Debug.Log("NO playerShoot");
        playerController = _controller;
        playerShoot = _playerShoot;
    }

    void SetFuelAmount(float _amount)
    {
        thrusterFuelFill.localScale = new Vector3(1f, _amount, 1f);
    }

}
