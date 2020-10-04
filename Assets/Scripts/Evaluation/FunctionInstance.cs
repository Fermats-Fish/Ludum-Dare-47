using System;
using System.Linq;

public class FunctionInstance {
    Evaluatable eval;
    Robot robot;
    FunctionInstance[] functionArgs;

    public bool IsSlowAction() {
        return eval is SlowActionFunc;
    }

    public static FunctionInstance Compile(Robot robot, string[] inputs, FunctionInstance[] availableArgs = null) {
        if (availableArgs == null) {
            availableArgs = new FunctionInstance[] { };
        }

        // Get last string value.
        var valueToParse = inputs[inputs.Length - 1];

        // Get other strings.
        var remainingInputs = inputs.SubArray(0, -1);

        // Get the function for this value.
        Evaluatable func;
        if (Int32.TryParse(valueToParse, out var res)) {
            func = new IntValue(res);
        } else {
            try {
                func = Function.functions[valueToParse.ToLower()];
            } catch {
                throw new CompileError(valueToParse + " is not a valid value/function");
            }
        }

        // Get remaining available args.
        if (availableArgs.Length < func.numArgs) {
            throw new CompileError("Not enough arguments on line");
        }
        var usedArgs = availableArgs.SubArray(0, func.numArgs);
        var remainingArgs = availableArgs.SubArray(func.numArgs);

        // Create a function instance.
        var lastFuncInstance = new FunctionInstance(robot, func, usedArgs);

        // Insert as first value in remaining args.
        remainingArgs = new FunctionInstance[] { lastFuncInstance }.Concat(remainingArgs).ToArray();

        // Depending on whether there are any remaining strings or not recurse.
        if (remainingInputs.Length == 0) {
            if (remainingArgs.Length != 1) {
                throw new CompileError("Too many arguments on line");
            }
            return lastFuncInstance;
        } else {
            return Compile(robot, remainingInputs, remainingArgs);
        }
    }

    public FunctionInstance(Robot robot, Evaluatable eval, FunctionInstance[] functionArgs) {
        this.robot = robot;
        this.eval = eval;
        this.functionArgs = functionArgs;
    }

    public Value Evaluate() {
        return eval.Evaluate(robot, functionArgs.Select(x => x.Evaluate()).ToArray());
    }
}
