using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class OverrideNeworkDiscovery : NetworkDiscovery {

    public static OverrideNeworkDiscovery singleton { get; private set; }
    private Dictionary<string,Tuple<MatchJoiner,float> > localMatches; //float is expire time
    private NetworkManager networkManager;

    private LANMatch localMatch; // data to broadcast as server
 
   
    void Awake()
    {        
        
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Destroy(this);
        }
        localMatches = new Dictionary<string, Tuple<MatchJoiner, float> >();
        
    }

    public void UpdateData()
    {
       broadcastData = localMatch.GetDataString();
    }

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        base.OnReceivedBroadcast(fromAddress, data);
        Debug.Log("broadcast received from xyx " + fromAddress + " data = " + data);        
        bool validData = false;
        MatchJoiner match = LANMatch.GetMatchJoiner(fromAddress, data,out validData);
        if (validData)
        {
            Debug.Log("Valid Data");
            var tuple = new  Tuple<MatchJoiner, float>(match, Time.time + broadcastInterval + 1f);
            if (!localMatches.ContainsKey(fromAddress))
            {
                localMatches.Add(fromAddress,tuple);
            }
            else
            {
                localMatches[fromAddress] = tuple;
            }
        }
        else
        {
            Debug.Log("currupt data received");
        }
    }

    
    private IEnumerator CleanUpExpiredServers()
    {       
        while (true)
        {
            
            foreach(string match in localMatches.Keys)
            {
                if(localMatches[match].Item2 >= Time.time)
                {
                    localMatches.Remove(match);
                }
            }
            yield return new WaitForSeconds(2f*broadcastInterval);
        }

        
    }

    void OnDisable()
    {
        if(running)
            base.StopBroadcast();

    }

   

    public void StartBroadcasting(LANMatch _localMatch)
    {
        if (running)
        {
            base.StopBroadcast();
        }
        localMatch = _localMatch;
        UpdateData();
        base.Initialize();
        Debug.Log("Stating as Server");
        base.StartAsServer();
       
        
    }

    public void StartListeningForServers()
    {
        if(isServer && running)
            base.StopBroadcast();
        if (!isClient)
        {
            base.Initialize();
            base.StartAsClient();
        
        }
        
    }

    public  MatchJoiner[] GetMatchList()
    {
        MatchJoiner[] matches = new MatchJoiner[localMatches.Count];
        int i = 0;
        foreach (string match in localMatches.Keys)
        {
            matches[i++] = localMatches[match].Item1;
            
        }
        return matches;

    }

}
