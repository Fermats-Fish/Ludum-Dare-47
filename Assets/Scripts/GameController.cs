﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public static GameController instance;

    public Base playerBase;

    public List<Entity> entities;

    Dictionary < (int, int), Resource > resources;
    public const float timeStep = 0.2f;
    float time = timeStep;

    public GameObject robotPrefab, floorPrefab, wallPrefab, resourcePrefab, enemyPrefab;

    public(int x, int y) gridSize = (10, 10);
    public int wallCount = 3, resourceCount = 3;

    public Dictionary < (int, int), Wall > walls = new Dictionary < (int, int), Wall > ();

    // Start is called before the first frame update
    void Start() {
        if (instance == null) {
            instance = this;
        }
        InitGrid();
        SpawnRobot();
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

        for (int i = 0; i < wallCount; i++) {
            Wall wall = Instantiate(wallPrefab).GetComponent<Wall>();
            var pos = RandomPosition();
            if (!walls.ContainsKey(pos)) {
                walls.Add(pos, wall);
                wall.transform.position = TileToWorldCoord(pos);
            }
        }

        SpawnResource();

    }

    public void SpawnResource() {
        int tries = 0;
        for (int i = 0; i < resourceCount; i++) {
            Resource resource = Instantiate(resourcePrefab).GetComponent<Resource>();
            var pos = RandomPosition();
            while (walls.ContainsKey(pos) && tries < 100) {
                pos = RandomPosition();
                tries++;
            }
            if (tries >= 100) {
                return;
            }
            Vector3 worldPos = TileToWorldCoord(pos);
            resource.transform.position = worldPos;
        }
    }

    public Robot SpawnRobot() {
        Robot robot = Instantiate(robotPrefab).GetComponent<Robot>();
        return robot;
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
