using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Function : Evaluatable {

    Func<Robot, Value[], Value> evaluateFunction;

    public static Dictionary<string, Function> functions = new Dictionary<string, Function>();

    public static readonly Function[] functionsArray = new Function[] {
        new Function("GetPosition", 0, (Robot robot, Value[] args) => {
            Debug.Log(robot.transform.position);
            return new CoordValue((int) robot.transform.position.x, (int) robot.transform.position.y);
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
        // If statement will cause the next statement to be run if the condition is met, or 
        new ActionFunc("If", 1, (Robot robot, Value[] args) => {
            var doAction = ((BoolValue) args[0]).value;
            Debug.Log(doAction);
            if (!doAction) {
                robot.currentLine += 1;
            }
        }),
        new ActionFunc("MoveForward", 0, (Robot robot, Value[] args) => {
            robot.MoveForward();
        }),
        new ActionFunc("TurnRight", 0, (Robot robot, Value[] args) => {
            robot.TurnRight();
        }),
        new ActionFunc("TurnLeft", 0, (Robot robot, Value[] args) => {
            robot.TurnLeft();
        }),
        new ActionFunc("MoveDirection", 1, (Robot robot, Value[] args) => {
            robot.MoveDirection(Robot.directions[((IntValue) args[0]).value]);
        }),
        new ActionFunc("Goto", 1, (Robot Robot, Value[] args) => {
            Robot.currentLine = ((IntValue) args[0]).value - 1;
        }),
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
