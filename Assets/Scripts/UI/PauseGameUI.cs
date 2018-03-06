using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class PauseGameUI : MonoBehaviour {
    

    [HideInInspector]
    public static PauseGameUI singleton { get; private set; }    
    // OnResume callback to HUD
    [HideInInspector]
    public delegate void OnResume();
    private OnResume onResume; 

    
    void Awake()
    {
        Debug.Log("Setting PauseGameUI singleton");
        //Setup singleton
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        onResume = null;
    }

	
    //Don't initialize in Start() as Pause() is called first
    //Because this gameobject is disabled by default and Pause() enables it for the first time.
    //And Pause() saves some values.
	//void Start () {
    //    
	//	
	//}

	
	// Update is called once per frame

    
	void Update () {

        // Return to game
        if (Input.GetButtonDown("Cancel"))
        {
            Resume();
        }
	}

    public void Pause(OnResume _onResume)
    {
        onResume = _onResume;
        //Enable Cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        gameObject.SetActive(true);
    }

    public void Resume()
    {
        gameObject.SetActive(false);

        // Cursor lock
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        //Invoke playercontroller to resume game
        onResume.Invoke();
    }

    public void LeaveRoom()
    {
       
        MatchInfo matchInfo = NetworkManager.singleton.matchInfo;
        if (NetworkManager.singleton.matchMaker != null)
            NetworkManager.singleton.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, matchInfo.domain, NetworkManager.singleton.OnDropConnection);

        if (OverrideNeworkDiscovery.singleton.running)
            OverrideNeworkDiscovery.singleton.StopBroadcast();
        NetworkManager.singleton.StopHost();
    }

}
