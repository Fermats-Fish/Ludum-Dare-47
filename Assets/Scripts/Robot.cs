using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : Entity {

    public int directionFacing = 3;

    // List<Instruction> instructions = new List<Instruction>();
    List<FunctionInstance> instructions = new List<FunctionInstance>();

    public int currentLine;

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
        AddInstruction("If SolidAt CoordPlus GetPosition Forwards");
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
        MoveDirection(directionFacing);
    }

    protected override void RunProgram() {
        var instruction = instructions[currentLine];
        instruction.Evaluate();

        currentLine += 1;

        if (currentLine >= instructions.Count) {
            currentLine = 0;
        }
    }
}
