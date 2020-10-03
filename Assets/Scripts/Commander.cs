using System;

public static class Commander {
  public static Func<Robot, string, bool> Always = (Robot robot, string param) => true;
  public static Func<Robot, string, bool> RobotXLessThan = (Robot robot, string param) => robot.transform.position.x < float.Parse(param);
  public static Func<Robot, string, bool> RobotXMoreThan = (Robot robot, string param) => robot.transform.position.x > float.Parse(param);
  public static Func<Robot, string, bool> RobotYLessThan = (Robot robot, string param) => robot.transform.position.y < float.Parse(param);
  public static Func<Robot, string, bool> RobotYMoreThan = (Robot robot, string param) => robot.transform.position.y > float.Parse(param);

  public static void AddCommand(Robot robot, Command command, Predicate predicateEnum, string param) {

    var predicate = GetPredicate(predicateEnum);
    switch(command) {
      case Command.Print:
        robot.AddInstruction(new PrintInstruction(predicate, ""));
        break;
      case Command.MoveEast:
        robot.AddInstruction(new MoveEastInstruction(predicate, param));
        break;
      case Command.MoveWest:
        robot.AddInstruction(new MoveWestInstruction(predicate, param));
        break;
      case Command.MoveNorth:
        robot.AddInstruction(new MoveNorthInstruction(predicate, param));
        break;
      case Command.MoveSouth:
        robot.AddInstruction(new MoveSouthInstruction(predicate, param));
        break;
    }
  }

  private static Func<Robot, string, bool> GetPredicate(Predicate predicate) {
    switch (predicate) {
      case Predicate.Always:
        return Always;
      case Predicate.XLessThan:
        return RobotXLessThan;
      case Predicate.XMoreThan:
        return RobotXMoreThan;
      case Predicate.YLessThan:
        return RobotYLessThan;
      case Predicate.YMoreThan:
        return RobotYMoreThan;
    }

    return Always;
  }
}

public enum Predicate {
  Always,
  XLessThan,
  YLessThan,
  XMoreThan,
  YMoreThan,
}

public enum Command {
  Print,
  MoveEast,
  MoveNorth,
  MoveWest,
  MoveSouth
};