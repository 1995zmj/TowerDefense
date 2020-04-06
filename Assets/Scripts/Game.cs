using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class Game : MonoBehaviour {

    [SerializeField]
    Vector2Int boardSize = new Vector2Int(11, 11);

    [SerializeField]
    GameBoard board = default;
    
    [SerializeField]
    GameTileContentFactory tileContentFactory = default;
    [SerializeField]
    WarFactory warFactory = default;
    // [SerializeField]
    // EnemyFactory enemyFactory = default;
    //
    GameBehaviorCollection enemies = new GameBehaviorCollection();
    GameBehaviorCollection nonEnemies = new GameBehaviorCollection();
    [SerializeField]
    GameScenario scenario = default;

    GameScenario.State activeScenario;
    
    [SerializeField, Range(0, 100)]
    int startingPlayerHealth = 10;
    int playerHealth;
    // [SerializeField, Range(0.1f, 10f)]
    // float spawnSpeed = 1f;
    [SerializeField, Range(1f, 10f)]
    float playSpeed = 1f;
    // float spawnProgress;
    const float pausedTimeScale = 0f;
    TowerType selectedTowerType;
    Ray TouchRay => Camera.main.ScreenPointToRay(Input.mousePosition);
    static Game instance;
    void OnEnable () {
        instance = this;
    }
    void Start () {
        board.Initialize(boardSize, tileContentFactory);
        board.ShowGrid = true;
        activeScenario = scenario.Begin();
        playerHealth = startingPlayerHealth;
    }
    
    void OnValidate () {
        if (boardSize.x < 2) {
            boardSize.x = 2;
        }
        if (boardSize.y < 2) {
            boardSize.y = 2;
        }
    }
    
    void BeginNewGame () {
        enemies.Clear();
        nonEnemies.Clear();
        board.Clear();
        activeScenario = scenario.Begin();
        playerHealth = startingPlayerHealth;
    }
    
    void Update () {
        if (Input.GetMouseButtonDown(0)) {
            HandleTouch();
        }else if (Input.GetMouseButtonDown(1)) {
            HandleAlternativeTouch();
        }else if (Input.GetKeyDown(KeyCode.V)) {
            board.ShowPaths = !board.ShowPaths;
        }else if (Input.GetKeyDown(KeyCode.G)) {
            board.ShowGrid = !board.ShowGrid;
        }else if (Input.GetKeyDown(KeyCode.B)) {
            BeginNewGame();
        }
        
        
        if (Input.GetKeyDown(KeyCode.Space)) {
            Time.timeScale =
                Time.timeScale > pausedTimeScale ? pausedTimeScale : 1f;
        }else if (Time.timeScale > pausedTimeScale) {
            Time.timeScale = playSpeed;
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            selectedTowerType = TowerType.Laser;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            selectedTowerType = TowerType.Mortar;
        }

        // spawnProgress += spawnSpeed * Time.deltaTime;
        // while (spawnProgress >= 1f) {
        //     spawnProgress -= 1f;
        //     SpawnEnemy();
        // }
        if (playerHealth <= 0 && startingPlayerHealth > 0) {
            Debug.Log("Defeat!");
            BeginNewGame();
        }
        
        if (!activeScenario.Progress() && enemies.IsEmpty) {
            Debug.Log("Victory!");
            BeginNewGame();
            activeScenario.Progress();
        }

        enemies.GameUpdate();
        nonEnemies.GameUpdate();
        Physics.SyncTransforms();
        board.GameUpdate();
    }
    public static void EnemyReachedDestination () {
        instance.playerHealth -= 1;
    }
    public static void SpawnEnemy (EnemyFactory factory, EnemyType type) {
    		GameTile spawnPoint = instance.board.GetSpawnPoint(
    			Random.Range(0, instance.board.SpawnPointCount)
    		);
    		Enemy enemy = factory.Get(type);
    		enemy.SpawnOn(spawnPoint);
    		instance.enemies.Add(enemy);
    	}
    
    public static Shell SpawnShell () {
        Shell shell = instance.warFactory.Shell;
        instance.nonEnemies.Add(shell);
        return shell;
    }
    
    public static Explosion SpawnExplosion () {
        Explosion explosion = instance.warFactory.Explosion;
        instance.nonEnemies.Add(explosion);
        return explosion;
    }
    void HandleTouch () {
        GameTile tile = board.GetTile(TouchRay);
        if (tile != null)
        {
            if (Input.GetKey((KeyCode.LeftShift)))
            {
                board.ToggleTower(tile, selectedTowerType);
            }
            else
            {
                board.ToggleWall(tile);
            }
        }
    }
    
    void HandleAlternativeTouch () {
        GameTile tile = board.GetTile(TouchRay);
        if (tile != null) {
            if (Input.GetKey(KeyCode.LeftShift)) {
                board.ToggleDestination(tile);
            }
            else {
                board.ToggleSpawnPoint(tile);
            }
        }
    }
}