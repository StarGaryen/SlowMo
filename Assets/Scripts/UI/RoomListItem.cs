using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class RoomListItem : MonoBehaviour {

    private MatchJoiner match;
    public delegate void ToJoin(MatchJoiner match);
    private ToJoin toJoin;


    [SerializeField]
    private Text roomNameText;

	
	
    public void Setup(MatchJoiner _match,ToJoin _toJoin)
    {
        match = _match;
        toJoin = _toJoin;
        roomNameText.text = match.GetName() + "(" + match.GetPlayerCount() + "/" + match.GetSize() + ")";
    }
 

    public void JoinGame()
    {
        if (match != null)
        {
            toJoin.Invoke(match);
        }
    }
}
