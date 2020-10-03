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
  public abstract void Run(Robot robot);
}

public interface PredicateFn {
  bool Check(Robot robot);
}

public class PrintInstruction : Instruction {
  public PrintInstruction(Func<Robot, string, bool> predicate, string predicateParam, string actionParam) : base(predicate, predicateParam, actionParam)
  {
  }

  public override void Run(Robot robot) {
    if (predicate.Invoke(robot, predicateParam)) {
      Console.WriteLine("Print X");
    }
  }
}

public class MoveEastInstruction : Instruction {
  public MoveEastInstruction(Func<Robot, string, bool> predicate, string predicateParam, string actionParam) : base(predicate, predicateParam, actionParam)
  {
  }

  public override void Run(Robot robot) {
    if (predicate.Invoke(robot, predicateParam)) {
      robot.MoveEast();
    }
  }
}

public class MoveWestInstruction : Instruction {
  public MoveWestInstruction(Func<Robot, string, bool> predicate, string predicateParam, string actionParam) : base(predicate, predicateParam, actionParam)
  {
  }

  public override void Run(Robot robot) {
    if (predicate.Invoke(robot, predicateParam)) {
      robot.MoveWest();
    }
  }
}

public class MoveNorthInstruction : Instruction {
  public MoveNorthInstruction(Func<Robot, string, bool> predicate, string predicateParam, string actionParam) : base(predicate, predicateParam, actionParam)
  {
  }

  public override void Run(Robot robot) {
    if (predicate.Invoke(robot, predicateParam)) {
      robot.MoveNorth();
    }
  }
}

public class MoveSouthInstruction : Instruction {
  public MoveSouthInstruction(Func<Robot, string, bool> predicate, string predicateParam, string actionParam) : base(predicate, predicateParam, actionParam)
  {
  }

  public override void Run(Robot robot) {
    if (predicate.Invoke(robot, predicateParam)) {
      robot.MoveSouth();
    }
  }
}

public class JumpInstruction : Instruction {
  public JumpInstruction(Func<Robot, string, bool> predicate, string predicateParam, string actionParam) : base(predicate, predicateParam, actionParam)
  {
  }

  public override void Run(Robot robot) {
    if (predicate.Invoke(robot, predicateParam)) {
      int lineNumber = 0;
      if(Int32.TryParse(actionParam, out lineNumber)) {
        robot.SetCurrentLine(lineNumber);   
      }
      
    }
  }
}