using System;

public abstract class Instruction {
  public Instruction(Func<Robot, string, bool> predicate, string predicateParam, string actionParam) {
    this.predicateParam = predicateParam;
    this.actionParam = actionParam;
    this.predicate = predicate;
  }
  protected string predicateParam;
  protected string actionParam;
  protected Func<Robot, string, bool> predicate { get; set; }
  public abstract void Run(Robot robot, string actionParam);
}

public interface PredicateFn {
  bool Check(Robot robot);
}

public class PrintInstruction : Instruction {
  public PrintInstruction(Func<Robot, string, bool> predicate, string predicateParam, string actionParam) : base(predicate, predicateParam, actionParam)
  {
  }

  public override void Run(Robot robot, string actionParam) {
    if (predicate.Invoke(robot, param)) {
      Console.WriteLine("Print X");
    }
  }
}

public class MoveEastInstruction : Instruction {
  public MoveEastInstruction(Func<Robot, string, bool> predicate, string predicateParam, string actionParam) : base(predicate, predicateParam, actionParam)
  {
  }

  public override void Run(Robot robot, string actionParam) {
    if (predicate.Invoke(robot, param)) {
      robot.MoveEast();
    }
  }
}

public class MoveWestInstruction : Instruction {
  public MoveWestInstruction(Func<Robot, string, bool> predicate, string predicateParam, string actionParam) : base(predicate, predicateParam, actionParam)
  {
  }

  public override void Run(Robot robot, string actionParam) {
    if (predicate.Invoke(robot, param)) {
      robot.MoveWest();
    }
  }
}

public class MoveNorthInstruction : Instruction {
  public MoveNorthInstruction(Func<Robot, string, bool> predicate, string predicateParam, string actionParam) : base(predicate, predicateParam, actionParam)
  {
  }

  public override void Run(Robot robot, string actionParam) {
    if (predicate.Invoke(robot, param)) {
      robot.MoveNorth();
    }
  }
}

public class MoveSouthInstruction : Instruction {
  public MoveSouthInstruction(Func<Robot, string, bool> predicate, string predicateParam, string actionParam) : base(predicate, predicateParam, actionParam)
  {
  }

  public override void Run(Robot robot, string actionParam) {
    if (predicate.Invoke(robot, param)) {
      robot.MoveSouth();
    }
  }
}

public class JumpInstruction : Instruction {
  public JumpInstruction(Func<Robot, string, bool> predicate, string predicateParam, string actionParam) : base(predicate, predicateParam, actionParam)
  {
  }

  public override void Run(Robot robot, string actionParam) {
    if (predicate.Invoke(robot, param)) {
      robot.SetInstruction(Int32.Parse(actionParam));
    }
  }
}