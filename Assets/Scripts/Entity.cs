using UnityEngine;

public class Entity : MonoBehaviour {
    public(int x, int y) curPos;
    public Vector3 direction;
    public(int x, int y) target = (0, 0);

    public static readonly(int x, int y) north = (0, 1);
    public static readonly(int x, int y) east = (1, 0);
    public static readonly(int x, int y) south = (0, -1);
    public static readonly(int x, int y) west = (-1, 0);
    public static readonly(int x, int y) [] directions = new(int x, int y) [] { north, east, south, west };

    protected virtual void Start() {
        GameController.instance.entities.Add(this);
        target = ((int) transform.position.x, (int) transform.position.y);
    }

    protected virtual void Update() {
        var pos = transform.position;
        pos += direction / GameController.timeStep * Time.deltaTime;
        transform.position = pos;
    }

    public void TakeStep() {
        direction = Vector3.zero;
        if (GameController.instance.entities.Find(x => x is Enemy && x.curPos == curPos && x != this) != null) {
            Debug.Log("Death");
        }
        curPos = target;
        transform.position = GameController.instance.TileToWorldCoord(curPos);
        RunProgram();
    }

    protected virtual void RunProgram() {

    }

    public void MoveDirection(int dir) {
        dir %= 4;
        if (dir < 0) {
            dir += 4;
        }
        var dirCoord = directions[dir];
        direction = dirCoord.ToVector3();
        target = (curPos.x + dirCoord.x, curPos.y + dirCoord.y);

        if (target.x < 0) {
            target.x += GameController.instance.gridSize.x;
        }

        if (target.y < 0) {
            target.y += GameController.instance.gridSize.y;
        }

        if (target.x >= GameController.instance.gridSize.x) {
            target.x -= GameController.instance.gridSize.x;
        }

        if (target.y >= GameController.instance.gridSize.y) {
            target.y -= GameController.instance.gridSize.y;
        }

        // Check for walls
        if (GameController.instance.walls.ContainsKey(target)) {
            target = curPos;
            direction = Vector3.zero;
        }
    }
}
