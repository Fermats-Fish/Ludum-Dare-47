using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : Entity {

    int actionsSinceSlowAction = 0;

    public int directionFacing = 3;
    public(int x, int y) basePos;

    public bool lastIfEvaluated = true;

    public Value[] memory = new Value[] { null, null, null };

    // List<Instruction> instructions = new List<Instruction>();
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
        // AddInstruction("If SolidAt Add GetPosition Forwards");
        // AddInstruction("TurnRight");
        // AddInstruction("MoveForward");
        AddInstruction("If LessThan 0 GetYCoordOf GetBasePos");
        AddInstruction("Goto 5");
        AddInstruction("Else");
        AddInstruction("TurnDir RandomDir");
        AddInstruction("Goto 0");
        AddInstruction("If SolidAt Add GetPosition Forwards");
        AddInstruction("TurnDir RandomDir");
        AddInstruction("If Not SolidAt Add GetPosition Forwards");
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
        MoveDirection(directionFacing);
    }

    protected override void RunProgram() {

        if (currentLine > instructions.Count || currentLine < 0) {
            currentLine = 0;
        }

        var instruction = instructions[currentLine];
        instruction.Evaluate();

        currentLine += 1;

        transform.rotation = rotations[directionFacing];

        if (!instruction.IsSlowAction()) {
            actionsSinceSlowAction += 1;
            if (actionsSinceSlowAction >= 100) {
                Debug.Log("Infinite loop detected!");
                actionsSinceSlowAction = 0;
            } else {
                RunProgram();
            }
        } else {
            actionsSinceSlowAction = 0;
        }
    }
}
