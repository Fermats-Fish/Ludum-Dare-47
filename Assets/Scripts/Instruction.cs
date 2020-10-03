using System;

public abstract class Instruction {
  public Instruction(Func<Robot, string, bool> predicate, string param) {
    this.param = param;
    this.predicate = predicate;
  }
  protected string param;
  protected Func<Robot, string, bool> predicate { get; set; }
  public abstract void Run(Robot robot);
}

public interface PredicateFn {
  bool Check(Robot robot);
}

public class PrintInstruction : Instruction {
  public PrintInstruction(Func<Robot, string, bool> predicate, string param) : base(predicate, param)
  {
  }

  public override void Run(Robot robot) {
    if (predicate.Invoke(robot, param)) {
      Console.WriteLine("Print X");
    }
  }
}

public class MoveEastInstruction : Instruction {
  public MoveEastInstruction(Func<Robot, string, bool> predicate, string param) : base(predicate, param)
  {
  }

  public override void Run(Robot robot) {
    if (predicate.Invoke(robot, param)) {
      robot.MoveEast();
    }
  }
}

public class MoveWestInstruction : Instruction {
  public MoveWestInstruction(Func<Robot, string, bool> predicate, string param) : base(predicate, param)
  {
  }

  public override void Run(Robot robot) {
    if (predicate.Invoke(robot, param)) {
      robot.MoveWest();
    }
  }
}

public class MoveNorthInstruction : Instruction {
  public MoveNorthInstruction(Func<Robot, string, bool> predicate, string param) : base(predicate, param)
  {
  }

  public override void Run(Robot robot) {
    if (predicate.Invoke(robot, param)) {
      robot.MoveNorth();
    }
  }
}


public class MoveSouthInstruction : Instruction {
  public MoveSouthInstruction(Func<Robot, string, bool> predicate, string param) : base(predicate, param)
  {
  }

  public override void Run(Robot robot) {
    if (predicate.Invoke(robot, param)) {
      robot.MoveSouth();
    }
  }
}