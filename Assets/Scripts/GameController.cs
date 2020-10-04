using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public static GameController instance;

    public Base playerBase;

    public List<Entity> entities;

    Dictionary < (int, int), Resource > resources;
    public const float timeStep = 0.6f;
    float time = timeStep;

    public GameObject robotPrefab, floorPrefab, wallPrefab, resourcePrefab, enemyPrefab, basePrefab;

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
        robot.curPos = RandomPosition();
        robot.basePos = robot.curPos;
        robot.directionFacing = Random.Range(0, 4);
        Instantiate(basePrefab, TileToWorldCoord(robot.curPos), Quaternion.identity);
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
