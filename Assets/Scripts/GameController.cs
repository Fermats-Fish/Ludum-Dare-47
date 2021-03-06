﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public static GameController instance;

    public Base playerBase;

    public const int SPAWN_RESOURCES_COST = 30;
    public const int BUY_ROBOT_COST = 50;

    private static int score = 0;
    public static int Score {
        get {
            return score;
        }
        set {
            score = value;
            UIController.instance.UpdateResourcesDisplay();
        }
    }
    private static int money = 100;
    public static int Money {
        get {
            return money;
        }
        set {
            money = value;
            UIController.instance.UpdateResourcesDisplay();
        }
    }

    public List<Entity> entities;

    public Dictionary < (int, int), Resource > resources = new Dictionary < (int, int), Resource > ();
    public const float timeStep = 0.6f;
    float time = timeStep;

    public GameObject robotPrefab, floorPrefab, wallPrefab, resourcePrefab, enemyPrefab, basePrefab;

    public(int x, int y) gridSize = (30, 30);
    const float wallDensity = 0.1f;
    const int resourceCount = 3;

    public Dictionary < (int, int), Wall > walls = new Dictionary < (int, int), Wall > ();
    public HashSet < (int, int) > bases = new HashSet < (int, int) > ();

    // Start is called before the first frame update
    void Start() {
        if (instance == null) {
            instance = this;
        }
        InitGrid();
        var robot = SpawnRobot();
        UIController.instance.SelectRobot(robot);

        // Center camera on starting robot.
        var pos = robot.transform.position;
        pos.z = -1000;
        CameraController.instance.transform.position = pos;

        SpawnEnemy();
    }

    public void SpawnExtraResources() {
        for (int i = 0; i < resourceCount; i++) {
            SpawnResource();
        }
        SpawnEnemy();
    }

    public Vector3 TileToWorldCoord((int x, int y) coord) {
        return TileToWorldCoord(coord.x, coord.y);
    }

    public Vector3 TileToWorldCoord(int x, int y) {
        return new Vector3(x - gridSize.x / 2, y - gridSize.y / 2, 0);
    }

    public(int x, int y) WorldToTileCoord(Vector3 pos) {
        return ((int) (pos.x + gridSize.x / 2), (int) (pos.y + gridSize.y / 2));
    }

    void InitGrid() {

        for (int i = 0; i < gridSize.x; i++) {
            for (int j = 0; j < gridSize.y; j++) {
                Vector3 pos = TileToWorldCoord(i, j);
                var floorTile = Instantiate(floorPrefab, transform);
                float c = Random.Range(0.5f, 0.6f);
                floorTile.GetComponent<SpriteRenderer>().color = new Color(c, c, c);
                floorTile.transform.position = pos;
            }
        }

        var wallCount = (int) (gridSize.x * gridSize.y * wallDensity);
        for (int i = 0; i < wallCount; i++) {
            var pos = RandomEmptyPos();
            if (!walls.ContainsKey(pos)) {
                Wall wall = Instantiate(wallPrefab).GetComponent<Wall>();
                walls.Add(pos, wall);
                wall.transform.position = TileToWorldCoord(pos);
            }
        }

        for (int i = 0; i < resourceCount; i++) {
            SpawnResource();
        }

    }

    public(int x, int y) RandomEmptyPos() {
        int tries = 0;
        var pos = RandomPosition();
        while ((walls.ContainsKey(pos) || resources.ContainsKey(pos) || bases.Contains(pos)) && tries < 100) {
            pos = RandomPosition();
            tries++;
        }
        if (tries >= 100) {
            return pos;
        }
        return pos;
    }

    public void SpawnResource() {
        var pos = RandomEmptyPos();
        SpawnResourceAt(pos);
    }

    public void SpawnResourceAt((int x, int y) pos) {
        Resource resource = Instantiate(resourcePrefab).GetComponent<Resource>();
        Vector3 worldPos = TileToWorldCoord(pos);
        resource.transform.position = worldPos;
        resources.Add(pos, resource);
    }

    public static(int x, int y) Bound(int x, int y) {
        return Bound((x, y));
    }

    public static(int x, int y) Bound((int x, int y) coord) {
        if (coord.x < 0) {
            coord.x += GameController.instance.gridSize.x;
        }

        if (coord.y < 0) {
            coord.y += GameController.instance.gridSize.y;
        }

        if (coord.x >= GameController.instance.gridSize.x) {
            coord.x -= GameController.instance.gridSize.x;
        }

        if (coord.y >= GameController.instance.gridSize.y) {
            coord.y -= GameController.instance.gridSize.y;
        }
        return coord;
    }

    // Bounding a relative pos bounds x between [-size/2, size/2] if size is odd, or [-size/2+1, size/2] if size is even.
    public static(int x, int y) BoundRelPos(int x, int y) {
        return BoundRelPos((x, y));
    }

    public static(int x, int y) BoundRelPos((int x, int y) coord) {
        var maxX = GameController.instance.gridSize.x / 2;
        var minX = maxX % 2 == 0 ? -maxX + 1 : -maxX;

        var maxY = GameController.instance.gridSize.y / 2;
        var minY = maxY % 2 == 0 ? -maxY + 1 : -maxY;

        if (coord.x < minX) {
            coord.x += GameController.instance.gridSize.x;
        } else if (coord.x > maxX) {
            coord.x -= GameController.instance.gridSize.x;
        }

        if (coord.y < minY) {
            coord.y += GameController.instance.gridSize.y;
        } else if (coord.y > maxY) {
            coord.y -= GameController.instance.gridSize.y;
        }
        return coord;
    }

    public Robot SpawnRobot() {
        Robot robot = Instantiate(robotPrefab).GetComponent<Robot>();
        robot.basePos = RandomEmptyPos();
        bases.Add(robot.basePos);
        robot.directionFacing = Random.Range(0, 4);
        robot.GoHome();
        var rBase = Instantiate(basePrefab, TileToWorldCoord(robot.basePos), Quaternion.identity).GetComponent<Base>();
        rBase.robot = robot;
        return robot;
    }

    public void SpawnEnemy() {
        Instantiate(enemyPrefab).GetComponent<Enemy>();
    }

    public(int, int) RandomPosition() {
        return (Random.Range(0, gridSize.x), Random.Range(0, gridSize.y));
    }

    // Update is called once per frame
    protected void Update() {

        time += Time.deltaTime;

        while (time >= timeStep) {

            time -= timeStep;
            foreach (Entity entity in entities) {
                entity.TakeStep();
            }
        }
    }
}
