using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Function : Evaluatable {

    public string name;
    public string description;

    Func<Robot, Value[], Value> evaluateFunction;

    public static Dictionary<string, Function> functions = new Dictionary<string, Function>();

    static CoordValue ToRelCoord(Robot robot, (int x, int y) coord) {

        // First translate based on robot pos.
        coord.x -= robot.curPos.x;
        coord.y -= robot.curPos.y;

        // Now rotate based on robot rot.

        // North doesn't require anything.

        // South means flip both. Also flip if east since flip plus rotate clockwise = rotate anticlockwise.
        if (robot.directionFacing == 2 || robot.directionFacing == 1) {
            coord.x = -coord.x;
            coord.y = -coord.y;
        }

        // West means rotate clockwise. Same for east since if east we already flipped.
        if (robot.directionFacing == 1 || robot.directionFacing == 3) {
            var y = coord.y;
            coord.y = -coord.x;
            coord.x = y;
        }

        return new CoordValue(GameController.BoundRelPos(coord));
    }

    static(int x, int y) FromRelCoord(Robot robot, (int x, int y) coord) {

        // First rotate.

        // South means flip both. Also flip if west since flip plus rotate clockwise = rotate anticlockwise.
        if (robot.directionFacing == 2 || robot.directionFacing == 3) {
            coord.x = -coord.x;
            coord.y = -coord.y;
        }

        // East means rotate clockwise. Same for west since if west we already flipped.
        if (robot.directionFacing == 1 || robot.directionFacing == 3) {
            var y = coord.y;
            coord.y = -coord.x;
            coord.x = y;
        }

        // Now translate
        coord.x += robot.curPos.x;
        coord.y += robot.curPos.y;

        // Finally bound
        return GameController.Bound(coord);

    }

    static CoordValue GetClosest(Robot robot, Func < (int, int), bool > predicate) {
        if (predicate(robot.curPos)) {
            return new CoordValue(0, 0);
        }
        int range = 0;
        while (true) {
            range += 1;
            for (int y = range; y >= -range; y--) {
                var absY = y > 0 ? y : -y;
                foreach (int x in new int[] {-range + absY, range - absY }) {
                    // This is a relative coordinate. We need to convert from it to a real coord.
                    var coord = FromRelCoord(robot, (x, y));
                    if (predicate(coord)) {
                        return new CoordValue(GameController.BoundRelPos(x, y));
                    }
                }
            }

            if (range > GameController.instance.gridSize.x + GameController.instance.gridSize.y) {
                throw new RunTimeError("Couldn't find closest thing");
            }
        }
    }

    public static readonly Function[] functionsArray = new Function[] {
        // Some constants
        new Function("Resource", 0, (Robot robot, Value[] args) => new IntValue(0), "Constant for input into GetClosest"),
            new Function("Wall", 0, (Robot robot, Value[] args) => new IntValue(1), "Constant for input into GetClosest"),
            new Function("Enemy", 0, (Robot robot, Value[] args) => new IntValue(2), "Constant for input into GetClosest"),
            new Function("True", 0, (Robot robot, Value[] args) => new BoolValue(true), "Constant value True"),
            new Function("False", 0, (Robot robot, Value[] args) => new BoolValue(false), "Constant value False"),

            // Getters
            new Function("GetPos", 0, (Robot robot, Value[] args) => {
                return new CoordValue(robot.curPos);
            }, "Returns the robots current position"),
            new Function("Forwards", 0, (Robot robot, Value[] args) => {
                return new CoordValue(Entity.directions[robot.directionFacing]);
            }, "The direction the robot is facing (e.g. (0,1) when the robot is facing north)"),
            new Function("GetClosest", 1, (Robot robot, Value[] args) => {
                var arg = (IntValue) args[0];

                // Get closest resource
                if (arg.value == 0) {
                    return GetClosest(robot, ((int x, int y) c) => GameController.instance.resources.ContainsKey(c));
                }

                // Get closest wall
                else if (arg.value == 1) {
                    return GetClosest(robot, ((int x, int y) c) => GameController.instance.walls.ContainsKey(c));
                }

                // Get closest enemy
                else if (arg.value == 2) {
                    throw new RunTimeError("GetClosest Enemy is not yet implimented");
                }

                throw new RunTimeError("Invalid argument supplied to GetClosest");
            }, "Returns the relative coordinates of the closest Resource/Wall"),
            new Function("GetBasePos", 0, (Robot robot, Value[] args) => {
                return ToRelCoord(robot, robot.basePos);
            }, "Gives the relative coordinates of this robot's base"),
            new Function("SolidAt", 1, (Robot robot, Value[] args) => {
                var coord = (CoordValue) args[0];
                return new BoolValue(GameController.instance.walls.ContainsKey((coord.x, coord.y)));
            }, "Returns whether the tile at the specified absolute coordinates has a wall or not"),
            new Function("CarryingResource", 0, (Robot robot, Value[] args) => {
                return new BoolValue(robot.GetHasResource());
            }, "Returns whether or not the robot is currently carrying a resource"),

            // Functions
            new Function("Add", 2, (Robot robot, Value[] args) => {
                if (args[0] is CoordValue && args[1] is CoordValue) {
                    var coord1 = (CoordValue) args[0];
                    var coord2 = (CoordValue) args[1];

                    return new CoordValue(GameController.Bound(coord1.x + coord2.x, coord1.y + coord2.y));
                } else if (args[0] is IntValue && args[1] is IntValue) {
                    var value1 = (IntValue) args[0];
                    var value2 = (IntValue) args[1];

                    return new IntValue(value1.value + value2.value);
                }
                throw new Exception("Can't add values of these types");
            }, "Adds the next two values (e.g. Add 1 2 returns 3)"),
            new Function("Subtract", 2, (Robot robot, Value[] args) => {
                if (args[0] is CoordValue && args[1] is CoordValue) {
                    var coord1 = (CoordValue) args[0];
                    var coord2 = (CoordValue) args[1];

                    return new CoordValue(coord1.x - coord2.x, coord1.y - coord2.y);
                } else if (args[0] is IntValue && args[1] is IntValue) {
                    var value1 = (IntValue) args[0];
                    var value2 = (IntValue) args[1];

                    return new IntValue(value1.value - value2.value);
                }
                throw new Exception("Can't subtract values of these types");
            }, "Subtracts the next two values (e.g. Subtract 1 2 returns -1)"),
            new Function("GetYCoordOf", 1, (Robot robot, Value[] args) => {
                return new IntValue(((CoordValue) args[0]).y);
            }, "Returns the y value of an absolute/relative coordinate"),
            new Function("GetXCoordOf", 1, (Robot robot, Value[] args) => {
                return new IntValue(((CoordValue) args[0]).x);
            }, "Returns the x value of an absolute/relative coordinate"),
            new Function("LessThan", 2, (Robot robot, Value[] args) => {
                return new BoolValue(((IntValue) args[0]).value < ((IntValue) args[1]).value);
            }, "Returns whether the first argument is less than the second (e.g. LessThan 1 2 returns True)"),
            new Function("LessThanOrEq", 2, (Robot robot, Value[] args) => {
                return new BoolValue(((IntValue) args[0]).value <= ((IntValue) args[1]).value);
            }, "Returns whether the first argument is less than or equal to the second (e.g. LessThanOrEq 1 2 returns True)"),
            new Function("Equals", 2, (Robot robot, Value[] args) => {
                if (args[0] is CoordValue && args[1] is CoordValue) {
                    var coord1 = (CoordValue) args[0];
                    var coord2 = (CoordValue) args[1];
                    return new BoolValue(coord1.x == coord2.x && coord1.y == coord2.y);
                } else if (args[0] is IntValue && args[1] is IntValue) {
                    return new BoolValue(((IntValue) args[0]).value == ((IntValue) args[1]).value);
                }
                throw new RunTimeError("Can't compare values of these types");

            }, "Returns whether the first argument equals the second. Both arguments must be the same type"),
            new Function("Not", 1, (Robot Robot, Value[] args) => {
                return new BoolValue(!((BoolValue) args[0]).value);
            }, "Takes a boolean value to its opposite"),
            new Function("Coord", 2, (Robot Robot, Value[] args) => {
                return new CoordValue(
                    ((IntValue) args[0]).value,
                    ((IntValue) args[1]).value
                );
            }, "Creates a cordinate from two integers"),
            new Function("RandomRange", 2, (Robot robot, Value[] args) => {
                return new IntValue(UnityEngine.Random.Range(((IntValue) args[0]).value, ((IntValue) args[1]).value));
            }, "Returns a random integer between two integers, exclusive of the maximum"),
            new Function("RandomDir", 0, (Robot robot, Value[] args) => {
                if (UnityEngine.Random.Range(0, 2) == 0) {
                    return new IntValue(-1);
                } else {
                    return new IntValue(1);
                }
            }, "Returns either -1 or 1, for input into TurnDir"),
            new Function("SetVar", 2, (Robot robot, Value[] args) => {
                robot.memory[((IntValue) args[0]).value] = args[1];
                return args[1];
            }, "Sets the value in argument two to the variable of index according to argument one"),
            new Function("GetVar", 1, (Robot robot, Value[] args) => {
                return robot.memory[((IntValue) args[0]).value];
            }, "Gets the current value in the variable with index according to the argument of this function"),

            // Quick actions

            // If statement will cause the next statement to be run if the condition is met, or 
            new ActionFunc("If", 1, (Robot robot, Value[] args) => {
                var doAction = ((BoolValue) args[0]).value;
                if (!doAction) {
                    robot.currentLine += 1;
                }
                robot.lastIfEvaluated = doAction;
            }, "Skips the next line of code if the condition isn't met"),

            // Else statement causes the next statement to be run only if the last evaluated if statement didn't have the condition met
            new ActionFunc("Else", 0, (Robot robot, Value[] args) => {
                if (robot.lastIfEvaluated) {
                    robot.currentLine += 1;
                }
            }, "Skips the next line of code if the previous if condition was met"),
            new ActionFunc("Goto", 1, (Robot Robot, Value[] args) => {
                Robot.currentLine = ((IntValue) args[0]).value - 1;
            }, "Jumps to the specified line in the program"),
            new ActionFunc("TurnRight", 0, (Robot robot, Value[] args) => {
                robot.TurnRight();
            }, "Turns the robot right"),
            new ActionFunc("TurnLeft", 0, (Robot robot, Value[] args) => {
                robot.TurnLeft();
            }, "Turns the robot left"),
            new ActionFunc("TurnDir", 1, (Robot robot, Value[] args) => {
                if (((IntValue) args[0]).value < 0) {
                    robot.TurnLeft();
                } else if (((IntValue) args[0]).value > 0) {
                    robot.TurnRight();
                }
            }, "Turns the robot right if the argument is > 0, and left if it is < 0"),

            // Slow actions
            new SlowActionFunc("MoveForward", 0, (Robot robot, Value[] args) => {
                robot.MoveForward();
            }, "Moves the robot forwards"),
            new SlowActionFunc("MoveDirection", 1, (Robot robot, Value[] args) => {
                robot.MoveDirection(((IntValue) args[0]).value);
            }, "Moves the robot in the specified direction (0 for north, 1 for east, etc.)"),
            new SlowActionFunc("PickupResource", 0, (Robot robot, Value[] args) => {
                robot.PickupResource();
            }, "Gets the robot to pick up the resource it is on top of"),
            new SlowActionFunc("Wait", 0, (Robot robot, Value[] args) => { }, "Gets the robot to wait"),
    };

    public Function(string name, int numArgs, Func<Robot, Value[], Value> evaluateFunction, string description) {
        this.name = name;
        this.numArgs = numArgs;
        this.evaluateFunction = evaluateFunction;
        this.description = description;
        functions.Add(name.ToLower(), this);
    }

    public override Value Evaluate(Robot robot, Value[] args) {
        return evaluateFunction(robot, args);
    }
}

public class ActionFunc : Function {
    public ActionFunc(string name, int numArgs, Action<Robot, Value[]> evaluateFunction, string description) : base(name, numArgs, (Robot robot, Value[] args) => {
        evaluateFunction(robot, args);
        return null;
    }, description) { }
}

public class SlowActionFunc : ActionFunc {
    public SlowActionFunc(string name, int numArgs, Action<Robot, Value[]> evaluateFunction, string description) : base(name, numArgs, evaluateFunction, description) { }
}
