using UnityEngine;

public class GameTile : MonoBehaviour {
	
    [SerializeField]
    Transform arrow = default;
    
    GameTile north, east, south, west;
    
    public static void MakeEastWestNeighbors (GameTile east, GameTile west) {
	    Debug.Assert(
		    west.east == null && east.west == null, "Redefined neighbors!"
	    );
	    west.east = east;
	    east.west = west;
    }
}