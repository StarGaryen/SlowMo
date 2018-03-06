using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour {
    
    public static GameManager singelton { get; private set; }
    [SerializeField]
    public MatchSettings matchSetting { get; private set; }
    

    [SerializeField]
    private GameObject sceneCamera;

   
   /// <summary>
   /// MatchSetting load and singleton 
   /// </summary>
    void Awake()
    {
        if(singelton!= null)
        {
            Debug.LogError("More than one Game Manager");
            Destroy(this);
        }
        else
        {
            singelton = this;
        }
        matchSetting = new MatchSettings();
        matchSetting.RespawnDelay = 3f;
    }

    /// <summary>
    /// return player count
    /// </summary>
    /// <returns> Number of players currently registered</returns>
    public int  GetPlayerCount()
    {
        return players.Count;
    }


    #region Player Traker
    private const string PLAYER_ID_PREFIX = "Player ";
    private static Dictionary<string, PlayerManager> players = new Dictionary<string, PlayerManager>();


    public static void RegisterPlayer(string _netID, PlayerManager _player)
    {
        
        string _playerID = PLAYER_ID_PREFIX + _netID;
        players.Add(_playerID, _player);
        _player.transform.name = _playerID;

        if (NetworkServer.active)
        {
            OverrideNeworkDiscovery.singleton.UpdateData();
        }
        
    }

    public static void UnRegisterPlayer(string _playerID)
    {
        players.Remove(_playerID);
        
    }

    public static PlayerManager GetPlayer(string _playerID)
    {
        return players[_playerID];
    }

    public void AddPlayerToScorecard(GameObject _parent)
    {
        foreach (string _playerId in players.Keys)
        {
            //UnityEngine.UI.Text playerslot = Instantiate(UnityEngine., _parent);
            //(_playerId + " health " + players[_playerId].GetCurrentHealth())
        }
        
    }
    #endregion

    public void SetSceneCameraActive(bool  isActive)
    {
        if (sceneCamera == null)
            return;
        sceneCamera.SetActive(isActive);

    }

    
}
