using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
[RequireComponent(typeof(PlayerManager))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerShoot))]
public class PlayerSetup : NetworkBehaviour {
    [SerializeField]
    private Behaviour[] componentsToDisable;
    
    [SerializeField]
    private string remoteLayerName="RemotePlayer";
    [SerializeField]
    private string dontDrawLayerName = "DontDraw";
    [SerializeField]
    private GameObject[] playerGraphics;

    [SerializeField]
    GameObject playerUIPrefab;
    [HideInInspector]
    public GameObject playerUIInstance;

	// Use this for initialization
	void Start () {
        Debug.Log("PlayerSetup started");
        if (!isLocalPlayer)
        {
            Debug.Log("Remote player");
            for(int i = 0; i < componentsToDisable.Length; i++)
            {
                componentsToDisable[i].enabled = false;
                AssignRemoteLayer();
            }
            
        }
        else
        {
            Debug.Log("Local Player");

            // Cursor lock
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            //Disable Player Graphics for Local Player
            foreach (GameObject graphics in playerGraphics)
                SetLayerRecursively(graphics, LayerMask.NameToLayer(dontDrawLayerName)); //todo change to Util

            //CreatePlayerUI
            playerUIInstance =Instantiate(playerUIPrefab);
            playerUIInstance.name = playerUIPrefab.name;
            // Config playerUI
            PlayerHUD HUD = playerUIInstance.GetComponentInChildren<PlayerHUD>();
            if (HUD == null)
            {
                Debug.LogError("No PlayerHUD component on" + playerUIInstance.name);
            }
            else
            {
                Debug.Log("Got HUD");
                HUD.SetPlayerControllers(GetComponent<PlayerController>(),GetComponent<PlayerShoot>());
            }


            GetComponent<PlayerManager>().SetupPlayer();
        }
        

    }
	
	void OnDisable()
    {
        if (isLocalPlayer)
        {
            GameManager.singelton.SetSceneCameraActive(true);
            Destroy(playerUIInstance);
            Cursor.visible = true;
        }           

        GameManager.UnRegisterPlayer(transform.name);
        
    }

    void AssignRemoteLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        string _netID = GetComponent<NetworkIdentity>().netId.ToString();
        PlayerManager _player = GetComponent<PlayerManager>();
        GameManager.RegisterPlayer(_netID, _player);
    }

    void SetLayerRecursively(GameObject obj,LayerMask mask)
    {
        obj.layer = mask;
        foreach(Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, mask);
        }
    }
}
