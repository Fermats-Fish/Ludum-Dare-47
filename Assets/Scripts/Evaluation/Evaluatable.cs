using System;
using UnityEngine;

public abstract class Evaluatable {

    public int numArgs;

    public abstract Value Evaluate(Robot robot, Value[] args);

}
