using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour {
    public Vector3 direction = Vector3.zero, target = Vector3.zero;
    public float maxSpeed;
    float speed;
    public static readonly Vector3 north = new Vector3(0, 1, 0);
    public static readonly Vector3 east = new Vector3(1, 0, 0);
    public static readonly Vector3 south = new Vector3(0, -1, 0);
    public static readonly Vector3 west = new Vector3(-1, 0, 0);

    public static readonly Vector3[] directions = new Vector3[] { north, east, south, west };
    public int directionFacing = 3;

    // List<Instruction> instructions = new List<Instruction>();
    List<FunctionInstance> instructions = new List<FunctionInstance>();

    public int currentLine;
    float timeStep = 0;

    // Start is called before the first frame update
    protected void Start() {
        AddInstruction("If LessThan GetXCoordOf GetPosition 0");
        AddInstruction("TurnRight");
        AddInstruction("MoveForward");
    }

    public void AddInstruction(string instruction) {
        instructions.Add(
            FunctionInstance.Compile(this, instruction.Split(' '))
        );
    }

    public void TurnLeft() {
        directionFacing -= 1;
        if (directionFacing < 0) {
            directionFacing += 4;
        }
    }

    public void TurnRight() {
        directionFacing += 1;
        if (directionFacing >= 4) {
            directionFacing -= 4;
        }
    }

    public void MoveForward() {
        Debug.Log("Move in dir " + directionFacing);
        MoveDirection(Robot.directions[directionFacing]);
    }

    public void MoveNorth() {
        MoveDirection(Robot.north);
    }

    public void MoveSouth() {
        MoveDirection(Robot.south);
    }

    public void MoveEast() {
        MoveDirection(Robot.east);
    }

    public void MoveWest() {
        MoveDirection(Robot.west);
    }

    protected void Update() {
        if (timeStep < GameController.instance.timeStep) {
            timeStep += Time.deltaTime;
            return;
        }

        Debug.Log("CurrentLine: " + currentLine);

        timeStep = 0;

        if (instructions.Count <= 0) {
            return;
        }

        var instruction = instructions[currentLine];
        instruction.Evaluate();

        currentLine += 1;

        if (currentLine >= instructions.Count) {
            currentLine = 0;
        }

        speed = Mathf.Min(Vector2.Distance(transform.position, target), 1) * maxSpeed / GameController.instance.timeStep;

        if (direction != null) {
            transform.position += speed * direction * Time.deltaTime;
        }

        GameController.instance.CheckBounds(this);
        direction = Vector3.zero;
    }

    public void MoveDirection(Vector3 direction) {
        target = transform.position + direction;
        target = new Vector3(Mathf.Round(target.x), Mathf.Round(target.y), Mathf.Round(target.z));
    }

    public void RunProgram() {
        transform.position = target;
    }

    protected void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("enemy")) {
            transform.position = Vector3.zero;
            target = Vector3.zero;
            print("death");
        }
        if (collision.gameObject.CompareTag("wall")) {
            speed = 0;
            transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
            direction = -direction;
            target = transform.position + direction;

            print("wall");
        }
    }

}
