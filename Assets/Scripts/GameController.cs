using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    private List<Vector3> gridPositions = new List<Vector3>();
    public Base playerBase;
    public List<Robot> robots;
    public List<Floor> floor;
    public List<Wall> walls;
    public List<Enemy> enemies;
    public List<Resource> resources;
    public float timeStep, time;

    public GameObject robotPrefab, floorPrefab, wallPrefab, resourcePrefab, enemyPrefab;

    public Vector2 gridSize = new Vector2(0, 0);
    public int wallCount = 3, resourceCount = 3;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        InitGrid();

        for(int i = 0;i<robots.Count;i++) {
            robots[i] = Instantiate(robotPrefab, transform).GetComponent<Robot>();
        }
    }

    void InitGrid()
    {
        gridPositions.Clear();

        for (int i = 0; i <= gridSize.x; i++)
        {
            for (int j = 0; j <= gridSize.y; j++)
            {
                Vector3 pos = new Vector3(i - gridSize.x / 2, j - gridSize.y / 2, 0);
                gridPositions.Add(pos);
                Floor floorTile = Instantiate(floorPrefab, transform).GetComponent<Floor>();
                float c = (Random.value + 9) / 10;
                floorTile.GetComponent<SpriteRenderer>().color = new Color(c, c, c);
                floorTile.transform.position = pos;
                floor.Add(floorTile);
            }
        }

        for (int i = 0; i < wallCount; i++) {
            Wall wall = Instantiate(wallPrefab).GetComponent<Wall>();
            wall.transform.position = RandomPosition();
        }

        SpawnResource();

    }

    public void SpawnResource() {
        int tries = 0;
        for (int i = 0; i < resourceCount; i++)
        {
            Resource resource = Instantiate(resourcePrefab).GetComponent<Resource>();
            Vector3 pos = RandomPosition();
            for (int j = 0; j < walls.Count; j++)
            {
                while (pos.Equals(walls[j]) || tries < 100)
                {
                    pos = RandomPosition();
                    tries++;
                }
                if (tries >= 100)
                {
                    return;
                }
            }
            resource.transform.position = pos;
        }
    }

    public Robot SpawnRobot()
    {
        Robot robot = Instantiate(robotPrefab).GetComponent<Robot>();
        robots.Add(robot);
        return robot;
    }

    public Vector3 RandomPosition() {
        Vector2 pos = Vector2.zero;
        while (pos.Equals(Vector2.zero))
        {
            pos = new Vector3((int)(gridSize.x * Random.value - gridSize.x / 2), (int)(gridSize.y * Random.value - gridSize.y / 2), 0);
        }
        return pos;
    }

    public void CheckBounds(Enemy e)
    {
        Vector2 pos = e.transform.position;
        Vector2 newPos = pos;
        Vector2 target = e.target;
        Vector2 newTarget = target;
        bool outOfBounds = false;

        if (target.x > gridSize.x / 2 && pos.x > gridSize.x / 2)
        {
            newPos.x = -gridSize.x / 2 - 1;
            newTarget.x = -gridSize.x / 2;
            outOfBounds = true;
        }
        if (target.x < -gridSize.x / 2 && pos.x < -gridSize.x / 2)
        {
            newPos.x = gridSize.x / 2 + 1;
            newTarget.x = gridSize.x / 2;
            outOfBounds = true;
        }
        if (target.y > gridSize.y / 2 && pos.y > gridSize.y / 2)
        {
            newPos.y = -gridSize.y / 2 - 1;
            newTarget.y = -gridSize.y / 2;
            outOfBounds = true;
        }
        if (target.y < -gridSize.y / 2 && pos.y < -gridSize.y / 2)
        {
            newPos.y = gridSize.y / 2 + 1;
            newTarget.y = gridSize.y / 2;
            outOfBounds = true;
        }
        if (outOfBounds)
        {
            e.target = newTarget;
            e.transform.position = newPos;
        }
    }

    public void CheckBounds(Robot robot)
    {
        Vector2 pos = robot.transform.position;
        Vector2 newPos = pos;
        Vector2 target = robot.target;
        Vector2 newTarget = target;
        bool outOfBounds = false;

        if (target.x > gridSize.x / 2 && pos.x > gridSize.x / 2)
        {
            newPos.x = -gridSize.x / 2 - 1;
            newTarget.x = -gridSize.x / 2;
            outOfBounds = true;
        }
        if (target.x < -gridSize.x / 2 && pos.x < -gridSize.x / 2)
        {
            newPos.x = gridSize.x / 2 + 1;
            newTarget.x = gridSize.x / 2;
            outOfBounds = true;
        }
        if (target.y > gridSize.y / 2 && pos.y > gridSize.y / 2)
        {
            newPos.y = -gridSize.y / 2 - 1;
            newTarget.y = -gridSize.y / 2;
            outOfBounds = true;
        }
        if (target.y < -gridSize.y / 2 && pos.y < -gridSize.y / 2)
        {
            newPos.y = gridSize.y / 2 + 1;
            newTarget.y = gridSize.y / 2;
            outOfBounds = true;
        }
        if (outOfBounds)
        {
            robot.target = newTarget;
            robot.transform.position = newPos;
        }
    }

    public void CheckBounds(Enemy e)
    {
        Vector2 pos = e.transform.position;
        Vector2 newPos = pos;
        Vector2 target = e.target;
        Vector2 newTarget = target;
        bool outOfBounds = false;

        if (target.x > gridSize.x / 2 && pos.x > gridSize.x / 2)
        {
            newPos.x = -gridSize.x / 2 - 1;
            newTarget.x = -gridSize.x / 2;
            outOfBounds = true;
        }
        if (target.x < -gridSize.x / 2 && pos.x < -gridSize.x / 2)
        {
            newPos.x = gridSize.x / 2 + 1;
            newTarget.x = gridSize.x / 2;
            outOfBounds = true;
        }
        if (target.y > gridSize.y / 2 && pos.y > gridSize.y / 2)
        {
            newPos.y = -gridSize.y / 2 - 1;
            newTarget.y = -gridSize.y / 2;
            outOfBounds = true;
        }
        if (target.y < -gridSize.y / 2 && pos.y < -gridSize.y / 2)
        {
            newPos.y = gridSize.y / 2 + 1;
            newTarget.y = gridSize.y / 2;
            outOfBounds = true;
        }
        if (outOfBounds)
        {
            e.target = newTarget;
            e.transform.position = newPos;
        }
    }

    // Update is called once per frame
    protected void Update()
    {
        if (time > timeStep)
        {
            
            time = 0;
            for (int i = 0; i < robots.Count; i++)
            {
                Robot robot = robots[i];
                robot.RunProgram();
            }
            for (int i = 0; i < enemies.Count; i++)
            {
                Enemy enemy = enemies[i];
                enemy.RunProgram();
            }
        }
        time += Time.deltaTime;
    }
}
