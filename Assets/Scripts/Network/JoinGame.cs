using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class JoinGame : MonoBehaviour {
    //Bose
    private NetworkManager networkManager;
    private OverrideNeworkDiscovery networkDiscovery;

    List<GameObject> roomList = new List<GameObject>();

    [Header("Serverlist Config")]
    [SerializeField]
    private Text ServerListStatus;
    [SerializeField]
    private GameObject roomListItemPrefab;
    [SerializeField]
    private GameObject roomListParent;
    


    [Header("Waiting Display Elements")]    
    [SerializeField]
    private GameObject waitScreen;


    private float requestStartTime = 0f;
	// Use this for initialization
	void Start () {
        
        networkManager = NetworkManager.singleton;
        networkDiscovery = OverrideNeworkDiscovery.singleton;
        if (networkManager.matchMaker == null)
        {
            Debug.Log("Enabling Matchmaker");
            networkManager.StartMatchMaker();
        }
        if (networkDiscovery != null)
        {
            Debug.Log("Started Listening for LAN Servers");
            networkDiscovery.Initialize();
            networkDiscovery.StartListeningForServers();
        }
    }

    public void OnEnable()
    {
        if (networkManager != null)
        {
            Debug.Log("Join Game Enabled.");
            if (networkManager.matchMaker == null)
            {
                Debug.Log("Enabling Matchmaker");
                networkManager.StartMatchMaker();
            }
            Refresh();
        }
        else
        {
            Debug.LogError("OnEnable :: NetworkManager not found");
        }
        if (networkDiscovery!=null)
        {
            Debug.Log("Started Listening for LAN Servers");
            networkDiscovery.Initialize();
            networkDiscovery.StartListeningForServers();
        }
        else
        {
            Debug.LogError("OnEnable :: NetworkDiscovery not found");
        }

    }
    
    public void OnDisable()
    {
        if (networkManager != null)
        {
          
            if (networkManager.matchMaker != null)
            {
                Debug.Log("Disabling Matchmaker");
                networkManager.StopMatchMaker();
                
            }
            ClearRoomList();
        }
        if (networkDiscovery != null)
        {
            Debug.Log("Stoping Listening for LAN Servers");
            if(networkDiscovery.isClient)
                networkDiscovery.StopBroadcast();
        }
    }

    public void Refresh()
    {
        if (!ServerListStatus.text.Equals("Loading..."))
        {
            ServerListStatus.text = "Loading...";
            ClearRoomList();
            networkManager.matchMaker.ListMatches(0, 20, "", true, 0, 0, OnMatchList);                  
            
        }
        else
        {
            Debug.Log("Already loading server list");
        }
            
    }

    // Callback funtion for request to get online matchList
    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
    {
        
        Debug.Log("Got Response for Internet Server List");
        OnLocalMatchList();
        if (success)
        {   
            foreach (MatchInfoSnapshot matchInfo in matchList)
            {
                MatchJoiner match = OnlineMatch.GetMatchJoiner(matchInfo);
                AddToList(match);
            }
            if (roomListParent.transform.childCount == 1)
            {
                ServerListStatus.text = "No active servers at the moment.";
            }
            else
            {
                ServerListStatus.text = "Server list ready.";
            }
        }        
        else
        {
            if (roomListParent.transform.childCount == 1)
            {
                ServerListStatus.text = "Couldn't connect to Internet. No LAN servers at the moment.";
            }else
            {
                ServerListStatus.text = "Couldn't connect to Internet. Showing Local Serves.";
            }
            
            return;
        }
    }

    private void OnLocalMatchList()
    {
        MatchJoiner[] LANMatches = networkDiscovery.GetMatchList();

        foreach (MatchJoiner match in LANMatches)
        {
            AddToList(match);
        }           
    }

    private void AddToList(MatchJoiner match)
    {
        GameObject _roomListItemInst = Instantiate(roomListItemPrefab, roomListParent.transform);
        RoomListItem roomListItemScript = _roomListItemInst.GetComponent<RoomListItem>();
        if (roomListItemScript != null)
        {
            roomListItemScript.Setup(match, JoinRoom);
            roomList.Add(_roomListItemInst);
        }
    }

    private void ClearRoomList()
    {
        foreach(GameObject room in roomList)
        {
            Destroy(room);
        }
        roomList.Clear();
    }

    public void JoinRoom(MatchJoiner match)
    {
        match.connect(networkManager);
        waitScreen.SetActive(true);
        StartCoroutine(WaitForHost());
        ClearRoomList();
    }

    IEnumerator WaitForHost()
    {
        int countdown = 8;
        while (countdown > 0)
        {
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        Debug.Log("Failed to join Game");
        networkManager.StopHost();

        if (networkDiscovery.isClient)
            networkDiscovery.StopBroadcast();

        waitScreen.SetActive(false);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            UnityEngine.Application.Quit();
        #endif
    }

}