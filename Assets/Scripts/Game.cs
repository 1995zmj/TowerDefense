﻿using System.Collections;
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
    [SerializeField]
    EnemyFactory enemyFactory = default;
    
    GameBehaviorCollection enemies = new GameBehaviorCollection();
    GameBehaviorCollection nonEnemies = new GameBehaviorCollection();

    [SerializeField, Range(0.1f, 10f)]
    float spawnSpeed = 1f;

    float spawnProgress;
    
    TowerType selectedTowerType;
    Ray TouchRay => Camera.main.ScreenPointToRay(Input.mousePosition);
    static Game instance;
    void OnEnable () {
        instance = this;
    }
    void Start () {
        board.Initialize(boardSize, tileContentFactory);
        board.ShowGrid = true;
    }
    
    void OnValidate () {
        if (boardSize.x < 2) {
            boardSize.x = 2;
        }
        if (boardSize.y < 2) {
            boardSize.y = 2;
        }
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
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            selectedTowerType = TowerType.Laser;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            selectedTowerType = TowerType.Mortar;
        }

        spawnProgress += spawnSpeed * Time.deltaTime;
        while (spawnProgress >= 1f) {
            spawnProgress -= 1f;
            SpawnEnemy();
        }
        
        enemies.GameUpdate();
        nonEnemies.GameUpdate();
        Physics.SyncTransforms();
        board.GameUpdate();
    }
    
    void SpawnEnemy () {
        GameTile spawnPoint =
            board.GetSpawnPoint(Random.Range(0, board.SpawnPointCount));
        Enemy enemy = enemyFactory.Get((EnemyType)(Random.Range(0, 3)));
        enemy.SpawnOn(spawnPoint);
        enemies.Add(enemy);
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