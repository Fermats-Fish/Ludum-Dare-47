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
    }

    protected virtual void Update() {
        var pos = transform.position;
        pos += direction / GameController.timeStep * Time.deltaTime;
        transform.position = pos;
    }

    public void TakeStep() {
        direction = Vector3.zero;
        GoToTarget();
        if (GameController.instance.entities.Find(x => x is Enemy && x.curPos == curPos && x != this) != null) {
            Die();
        }
        if (this is Enemy) {
            var other = GameController.instance.entities.Find(x => x.curPos == curPos && x != this);
            if (other != null) {
                other.Die();
            }
        }
        RunProgram();
    }

    public void GoToTarget() {
        curPos = target;
        transform.position = GameController.instance.TileToWorldCoord(curPos);
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
        target = GameController.Bound(curPos.x + dirCoord.x, curPos.y + dirCoord.y);

        // Check for walls
        if (GameController.instance.walls.ContainsKey(target)) {
            target = curPos;
            direction = Vector3.zero;
        }
    }

    public virtual void Die() { }
}
