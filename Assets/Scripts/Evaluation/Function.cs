using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Function : Evaluatable {

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

    static CoordValue GetClosest(Robot robot, Func < (int, int), bool > predicate) {
        int range = 0;
        while (true) {
            range += 1;
            for (int x = -range; x <= range; x++) {
                foreach (int y in new int[] {-range, range }) {
                    var coord = GameController.Bound(robot.curPos.x + x, robot.curPos.y + y);
                    if (predicate(coord)) {
                        return ToRelCoord(robot, coord);
                    }
                }
            }
            for (int y = -(range - 1); y <= range - 1; y++) {
                foreach (int x in new int[] {-range, range }) {
                    var coord = GameController.Bound(robot.curPos.x + x, robot.curPos.y + y);
                    if (predicate(coord)) {
                        return ToRelCoord(robot, coord);
                    }
                }
            }
            if (range > 1000) {
                throw new Exception("Couldn't find closest thing");
            }
        }
    }

    public static readonly Function[] functionsArray = new Function[] {
        // Some constants
        new Function("Resource", 0, (Robot robot, Value[] args) => new IntValue(1)),
            new Function("Wall", 0, (Robot robot, Value[] args) => new IntValue(2)),
            new Function("Enemy", 0, (Robot robot, Value[] args) => new IntValue(3)),
            new Function("True", 0, (Robot robot, Value[] args) => new BoolValue(true)),
            new Function("False", 0, (Robot robot, Value[] args) => new BoolValue(false)),

            // Getters
            new Function("GetPosition", 0, (Robot robot, Value[] args) => {
                return new CoordValue(robot.curPos);
            }),
            new Function("Forwards", 0, (Robot robot, Value[] args) => {
                return new CoordValue(Entity.directions[robot.directionFacing]);
            }),
            new Function("GetClosest", 1, (Robot robot, Value[] args) => {
                var arg = (IntValue) args[0];

                // Get closest resource
                if (arg.value == 1) {
                    return GetClosest(robot, ((int x, int y) c) => GameController.instance.resources.ContainsKey(c));
                }

                // Get closest wall
                else if (arg.value == 2) {
                    return GetClosest(robot, ((int x, int y) c) => GameController.instance.walls.ContainsKey(c));
                }

                // Get closest enemy
                else if (arg.value == 3) {
                    throw new NotImplementedException();
                }

                throw new Exception("Invalid argument supplied to GetClosest");
            }),
            new Function("GetBasePos", 0, (Robot robot, Value[] args) => {
                return ToRelCoord(robot, robot.basePos);
            }),
            new Function("SolidAt", 1, (Robot robot, Value[] args) => {
                var coord = (CoordValue) args[0];
                return new BoolValue(GameController.instance.walls.ContainsKey((coord.x, coord.y)));
            }),

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
            }),
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
            }),
            new Function("GetYCoordOf", 1, (Robot robot, Value[] args) => {
                return new IntValue(((CoordValue) args[0]).y);
            }),
            new Function("GetXCoordOf", 1, (Robot robot, Value[] args) => {
                return new IntValue(((CoordValue) args[0]).x);
            }),
            new Function("LessThan", 2, (Robot robot, Value[] args) => {
                return new BoolValue(((IntValue) args[0]).value < ((IntValue) args[1]).value);
            }),
            new Function("LessThanOrEq", 2, (Robot robot, Value[] args) => {
                return new BoolValue(((IntValue) args[0]).value <= ((IntValue) args[1]).value);
            }),
            new Function("Equals", 2, (Robot robot, Value[] args) => {
                return new BoolValue(((IntValue) args[0]).value == ((IntValue) args[1]).value);
            }),
            new Function("Not", 1, (Robot Robot, Value[] args) => {
                return new BoolValue(!((BoolValue) args[0]).value);
            }),
            new Function("Coord", 2, (Robot Robot, Value[] args) => {
                return new CoordValue(
                    ((IntValue) args[0]).value,
                    ((IntValue) args[1]).value
                );
            }),
            new Function("RandomRange", 2, (Robot robot, Value[] args) => {
                return new IntValue(UnityEngine.Random.Range(((IntValue) args[0]).value, ((IntValue) args[1]).value));
            }),
            new Function("RandomDir", 0, (Robot robot, Value[] args) => {
                if (UnityEngine.Random.Range(0, 2) == 0) {
                    return new IntValue(-1);
                } else {
                    return new IntValue(1);
                }
            }),
            new Function("SetVar", 2, (Robot robot, Value[] args) => {
                robot.memory[((IntValue) args[0]).value] = args[1];
                return args[1];
            }),
            new Function("GetVar", 1, (Robot robot, Value[] args) => {
                return robot.memory[((IntValue) args[0]).value];
            }),

            // Quick actions

            // If statement will cause the next statement to be run if the condition is met, or 
            new ActionFunc("If", 1, (Robot robot, Value[] args) => {
                var doAction = ((BoolValue) args[0]).value;
                if (!doAction) {
                    robot.currentLine += 1;
                }
                robot.lastIfEvaluated = doAction;
            }),

            // Else statement causes the next statement to be run only if the last evaluated if statement didn't have the condition met
            new ActionFunc("Else", 0, (Robot robot, Value[] args) => {
                if (robot.lastIfEvaluated) {
                    robot.currentLine += 1;
                }
            }),
            new ActionFunc("Goto", 1, (Robot Robot, Value[] args) => {
                Robot.currentLine = ((IntValue) args[0]).value - 1;
            }),
            new ActionFunc("TurnRight", 0, (Robot robot, Value[] args) => {
                robot.TurnRight();
            }),
            new ActionFunc("TurnLeft", 0, (Robot robot, Value[] args) => {
                robot.TurnLeft();
            }),
            new ActionFunc("TurnDir", 1, (Robot robot, Value[] args) => {
                if (((IntValue) args[0]).value < 0) {
                    robot.TurnLeft();
                } else if (((IntValue) args[0]).value > 0) {
                    robot.TurnRight();
                }
            }),

            // Slow actions
            new SlowActionFunc("MoveForward", 0, (Robot robot, Value[] args) => {
                robot.MoveForward();
            }),
            new SlowActionFunc("MoveDirection", 1, (Robot robot, Value[] args) => {
                robot.MoveDirection(((IntValue) args[0]).value);
            }),
            new SlowActionFunc("Wait", 0, (Robot robot, Value[] args) => { })
    };

    public Function(string name, int numArgs, Func<Robot, Value[], Value> evaluateFunction) {
        this.numArgs = numArgs;
        this.evaluateFunction = evaluateFunction;
        functions.Add(name.ToLower(), this);
    }

    public override Value Evaluate(Robot robot, Value[] args) {
        return evaluateFunction(robot, args);
    }
}

public class ActionFunc : Function {
    public ActionFunc(string name, int numArgs, Action<Robot, Value[]> evaluateFunction) : base(name, numArgs, (Robot robot, Value[] args) => {
        evaluateFunction(robot, args);
        return null;
    }) { }
}

public class SlowActionFunc : ActionFunc {
    public SlowActionFunc(string name, int numArgs, Action<Robot, Value[]> evaluateFunction) : base(name, numArgs, evaluateFunction) { }
}
