using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : Entity {

    bool ignition = false;
    public bool running = false;
    int actionsSinceSlowAction = 0;

    public int directionFacing = 3;
    public(int x, int y) basePos;

    public bool lastIfEvaluated = true;

    public Value[] memory = new Value[] { null, null, null };

    public string programText { get; protected set; }
    List<FunctionInstance> instructions = new List<FunctionInstance>();

    bool hasResource = false;

    bool error = false;

    SpriteRenderer sr;

    public static readonly Quaternion[] rotations = new Quaternion[] {
        Quaternion.Euler(0f, 0f, 0f),
        Quaternion.Euler(0f, 0f, -90f),
        Quaternion.Euler(0f, 0f, -180f),
        Quaternion.Euler(0f, 0f, -270f)
    };

    public int currentLine;

    // Start is called before the first frame update
    protected override void Start() {
        sr = GetComponent<SpriteRenderer>();

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

        UpdateColor();
    }

    public void Compile(string program) {
        List<FunctionInstance> newInstructions = new List<FunctionInstance>();
        var lines = program.Split('\n');
        for (var i = 0; i < lines.Length; i++) {
            var line = lines[i];
            if (line != "") {
                try {
                    newInstructions.Add(FunctionInstance.Compile(this, line.Replace("\r", "").Split(' ')));
                } catch (CompileError e) {
                    throw new CompileError("Line " + i + ": " + e.Message);
                }
            } else {
                newInstructions.Add(new FunctionInstance(this, new Value(), new FunctionInstance[] { }));
            }
        }
        instructions = newInstructions;
        programText = program;
    }

    static readonly Color resourceColor = new Color(0.7f, 0.7f, 0f);
    static readonly Color normalColor = new Color(91 / 255f, 87 / 255f, 245 / 255f);
    static readonly Color errorColor = new Color(1f, 20 / 255f, 30 / 255f);

    public void UpdateColor() {
        if (error) {
            sr.color = errorColor;
        } else if (hasResource) {
            sr.color = resourceColor;
        } else {
            sr.color = normalColor;
        }
    }

    public bool GetHasResource() {
        return hasResource;
    }

    public void PickupResource() {
        // Check for resource at this pos.
        if (GameController.instance.resources.ContainsKey(curPos)) {
            // Destroy the resource.
            Destroy(GameController.instance.resources[curPos].gameObject);
            GameController.instance.resources.Remove(curPos);

            // Set hasResource true.
            hasResource = true;

            // Update our colour!
            UpdateColor();
        }
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

        error = false;

        if (instructions.Count == 0) {
            running = false;
        }

        if (running && !ignition) {
            // Turn off if we are now back at base and there is no ignition.
            if (curPos == basePos) {
                running = false;

                // Also drop off any resource we are carrying.
                if (hasResource) {
                    hasResource = false;
                }
            }
        }

        if (!running) {
            UpdateColor();
            return;
        }

        if (currentLine >= instructions.Count || currentLine < 0) {
            currentLine = 0;
        }

        var instruction = instructions[currentLine];
        try {
            instruction.Evaluate();
        } catch (RunTimeError e) {
            Debug.Log("Runtime error: " + e.Message + "!");
            error = true;
        } catch (System.Exception e) {
            Debug.Log("Runtime error!");
            error = true;
        }

        currentLine += 1;

        transform.rotation = rotations[directionFacing];

        // Recursive section.
        if (!instruction.IsSlowAction() && !error && actionsSinceSlowAction < 100) {
            actionsSinceSlowAction += 1;
            RunProgram();
        }

        // Finishing off (only run once) section.
        else {
            if (actionsSinceSlowAction >= 100) {
                Debug.Log("Infinite loop detected! - Line " + currentLine);
                error = true;
            }
            actionsSinceSlowAction = 0;

            UpdateColor();

            if (ignition) {
                ignition = false;
            }
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
        ignition = true;
    }

    public override void Die() {
        GoHome();
    }
}
