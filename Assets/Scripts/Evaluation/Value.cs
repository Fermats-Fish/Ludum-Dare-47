using System;
using UnityEngine;

public class Value : Evaluatable {

    public Value() {
        numArgs = 0;
    }

    public override Value Evaluate(Robot robot, Value[] args) {
        return this;
    }

}

public class CoordValue : Value {

    public int x;
    public int y;

    public CoordValue(int x, int y) {
        this.x = x;
        this.y = y;
    }

}

public class IntValue : Value {
    public int value;

    public IntValue(int value) {
        this.value = value;
    }
}

public class BoolValue : Value {
    public bool value;

    public BoolValue(bool value) {
        this.value = value;
    }
}
