using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : Entity {

    public const int baseRescueCost = 10;
    public const int fixCost = 20;

    bool ignition = false;
    public bool running = false;
    int actionsSinceSlowAction = 0;

    public int directionFacing = 3;
    public(int x, int y) basePos;

    public bool lastIfEvaluated = true;

    public Value[] memory = new Value[10];

    public string programText { get; protected set; }
    List<FunctionInstance> instructions = new List<FunctionInstance>();

    bool hasResource = false;

    public bool error = false;
    public bool destroyed = false;

    public bool autoStart = false;

    SpriteRenderer sr;

    public string lastErrorMessage;

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

        // programText = "If LessThan 0 GetYCoordOf ToRelCoord GetClosest Resource\n" +
        //     "Goto 5\n" +
        //     "Else\n" +
        //     "TurnDir RandomDir\n" +
        //     "Goto 0\n" +
        //     "If SolidAt Add GetPos Forwards\n" +
        //     "TurnDir RandomDir\n" +
        //     "If Not SolidAt Add GetPos Forwards\n" +
        //     "MoveForward";

        programText = "If LessThan 0 GetYCoordOf ToRelCoord GetClosest Resource\n" +
            "MoveForward\n" +
            "Else\n" +
            "TurnDir RandomDir\n";

        // programText = "";

        Compile(programText);

        UpdateColor();

        // Force the rest of the update.
        if (UIController.instance.selectedRobot == this) {
            UIController.instance.SelectRobot(this);
        }
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

    public int GetRescueCost() {
        return Robot.baseRescueCost + (destroyed ? Robot.fixCost : 0);
    }

    static readonly Color resourceColor = new Color(0.7f, 0.7f, 0f);
    static readonly Color normalColor = new Color(91 / 255f, 87 / 255f, 245 / 255f);
    static readonly Color errorColor = new Color(1f, 20 / 255f, 30 / 255f);

    public void UpdateColor() {
        if (sr != null) {
            if (error) {
                sr.color = errorColor;
            } else if (hasResource) {
                sr.color = resourceColor;
            } else {
                sr.color = normalColor;
            }
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

    void OnResourceDropOff() {
        hasResource = false;
        GameController.Score += 10;
        GameController.Money += 30;
    }

    protected override void RunProgram() {

        error = false;

        if (destroyed) {
            OnError("Robot was destroyed");
        }

        if (instructions.Count == 0) {
            running = false;
        }

        if (running && !ignition) {
            // Turn off if we are now back at base and there is no ignition.
            if (curPos == basePos) {
                running = false;

                // Also drop off any resource we are carrying.
                if (hasResource) {
                    OnResourceDropOff();
                }

                if (autoStart) {
                    TurnOn();
                }

                if (UIController.instance.selectedRobot == this) {
                    UIController.instance.UpdateDisplay();
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
            OnError("Runtime error: " + e.Message + "!");
        } catch {
            OnError("Runtime error!");
        }

        currentLine += 1;

        transform.rotation = rotations[directionFacing];

        // Recursive section.
        int recurseLimit = Random.Range(100, 200);
        if (!instruction.IsSlowAction() && !error && actionsSinceSlowAction < recurseLimit) {
            actionsSinceSlowAction += 1;
            RunProgram();
        }

        // Finishing off (only run once) section.
        else {
            if (actionsSinceSlowAction >= recurseLimit) {
                OnError("Infinite loop detected!");
            }
            actionsSinceSlowAction = 0;

            UpdateColor();

            if (ignition) {
                ignition = false;
            }
        }

    }

    void OnError(string message) {
        error = true;
        lastErrorMessage = currentLine + ": " + message;
        if (UIController.instance.selectedRobot == this) {
            UIController.instance.UpdateDisplay();
        }
    }

    void OnMouseDown() {
        UIController.instance.SelectRobot(this);
    }

    public void Rescue() {
        // First check we have the cash.
        if (GameController.Money >= GetRescueCost()) {
            // Pay
            GameController.Money -= GetRescueCost();

            // If broken, fix.
            destroyed = false;

            // Drop resource if we have one.
            if (hasResource) {
                hasResource = false;
                GameController.instance.SpawnResourceAt(curPos);
            }

            // Go home.
            GoHome();
        }
    }

    public void GoHome() {
        target = basePos;
        direction = Vector3.zero;
        GoToTarget();
        running = false;
        if (UIController.instance != null && UIController.instance.selectedRobot == this) {
            UIController.instance.UpdateDisplay();
        }
        UpdateColor();
    }

    public void TurnOn() {
        currentLine = 0;
        running = true;
        ignition = true;
        if (UIController.instance.selectedRobot == this) {
            UIController.instance.UpdateDisplay();
        }
    }

    public override void Die() {
        destroyed = true;
    }
}
