using UnityEngine;

public enum GameTileContentType {
    Empty, 
    Destination,
    Wall
}

public class GameTileContent : MonoBehaviour {
    
    GameTileContentFactory originFactory;

    [SerializeField]
    GameTileContentType type = default;

    public GameTileContentType Type => type;
    
    public GameTileContentFactory OriginFactory {
        get => originFactory;
        set {
            Debug.Assert(originFactory == null, "Redefined origin factory!");
            originFactory = value;
        }
    }

    public void Recycle () {
        originFactory.Reclaim(this);
    }
}