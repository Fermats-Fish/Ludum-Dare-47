using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : Entity {

    public bool running = true;
    int actionsSinceSlowAction = 0;

    public int directionFacing = 3;
    public(int x, int y) basePos;

    public bool lastIfEvaluated = true;

    public Value[] memory = new Value[] { null, null, null };

    public string programText { get; protected set; }
    List<FunctionInstance> instructions = new List<FunctionInstance>();

    public static readonly Quaternion[] rotations = new Quaternion[] {
        Quaternion.Euler(0f, 0f, 0f),
        Quaternion.Euler(0f, 0f, -90f),
        Quaternion.Euler(0f, 0f, -180f),
        Quaternion.Euler(0f, 0f, -270f)
    };

    public int currentLine;

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
        programText = "If LessThan 0 GetYCoordOf GetClosest Resource\n" +
            "Goto 7\n" +
            "Else\n" +
            "TurnDir RandomDir\n" +
            "Goto 0\n" +
            "If SolidAt Add GetPos Forwards\n" +
            "TurnDir RandomDir\n" +
            "If Not SolidAt Add GetPos Forwards\n" +
            "MoveForward";
        // programText = "";
        Compile(programText);
        // AddInstruction("If SolidAt Add GetPosition Forwards");
        // AddInstruction("TurnRight");
        // AddInstruction("MoveForward");
    }

    public void Compile(string program) {
        List<FunctionInstance> newInstructions = new List<FunctionInstance>();
        var lines = program.Split('\n');
        foreach (var line in lines) {
            if (line != "") {
                newInstructions.Add(FunctionInstance.Compile(this, line.Split(' ')));
            } else {
                newInstructions.Add(new FunctionInstance(this, new Value(), new FunctionInstance[] { }));
            }
        }
        instructions = newInstructions;
        programText = program;
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
        MoveDirection(directionFacing);
    }

    protected override void RunProgram() {

        if (instructions.Count == 0) {
            running = false;
        }

        if (!running) {
            return;
        }

        if (currentLine >= instructions.Count || currentLine < 0) {
            currentLine = 0;
        }

        var instruction = instructions[currentLine];
        instruction.Evaluate();

        currentLine += 1;

        transform.rotation = rotations[directionFacing];

        if (!instruction.IsSlowAction()) {
            actionsSinceSlowAction += 1;
            if (actionsSinceSlowAction >= 100) {
                Debug.Log("Infinite loop detected! - Line " + currentLine);
                actionsSinceSlowAction = 0;
            } else {
                RunProgram();
            }
        } else {
            actionsSinceSlowAction = 0;
        }

        // Turn off if we are going back to base.
        if (target == basePos) {
            running = false;
        }

    }

    public void GoHome() {
        target = basePos;
        GoToTarget();
        running = false;
    }

    public void TurnOn() {
        currentLine = 0;
        running = true;
    }

    public override void Die() {
        GoHome();
    }
}
