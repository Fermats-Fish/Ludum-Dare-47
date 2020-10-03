
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{
    public Vector3 direction = Vector3.zero, target = Vector3.zero;
    public float maxSpeed;
    float speed;
    protected static Vector3 north = new Vector3(0, 1, 0);
    protected static Vector3 east = new Vector3(1, 0, 0);
    protected static Vector3 south = new Vector3(0, -1, 0);
    protected static Vector3 west = new Vector3(-1, 0, 0);
    protected static Vector3[] directions = new Vector3[] { north, east, south, west };
    
    List<Instruction> instructions = new List<Instruction>();

    int currentLine = 0;
    float timeStep = 0;

    // Start is called before the first frame update
    protected void Start()
    {
        Commander.AddCommand(this, Command.MoveEast, Predicate.Always, "");
        Commander.AddCommand(this, Command.MoveNorth, Predicate.XLessThan, "1.5");
        Commander.AddCommand(this, Command.MoveWest, Predicate.Always, "");
        Commander.AddCommand(this, Command.MoveEast, Predicate.Always, "");
        Commander.AddCommand(this, Command.MoveEast, Predicate.Always, "");
    }

    public void AddInstruction(Instruction instruction) {
        instructions.Add(instruction);
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

    // Update is called once per frame
    protected void Update()
    {
        if (timeStep < GameController.instance.timeStep) {
            timeStep += Time.deltaTime;
            return;
        }

        timeStep = 0;

        if (instructions.Count > 0) {
            var instruction = instructions[currentLine];

            instruction.Run(this);
        }

        currentLine++;
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

    private void MoveDirection(Vector3 direction) {
        target = transform.position + direction;
        target = new Vector3(Mathf.Round(target.x), Mathf.Round(target.y), Mathf.Round(target.z));
    }

    public void RunProgram()
    {
        transform.position = target;
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("enemy"))
        {
            transform.position = Vector3.zero;
            target = Vector3.zero;
            print("death");
        }
        if (collision.gameObject.CompareTag("wall"))
        {
            speed = 0;
            transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
            direction = -direction;
            target = transform.position + direction;

            print("wall");
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("enemy")) {
            transform.position = Vector3.zero;
            target = Vector3.zero;
            print("death");
        }
        if (collision.gameObject.CompareTag("wall"))
        {
            speed = 0;
            transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
            direction = -direction;
            target = transform.position + direction;
            
            print("wall");
        }
    }

}
