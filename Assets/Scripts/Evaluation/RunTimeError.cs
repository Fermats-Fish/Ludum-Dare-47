using System;

public class RunTimeError : Exception {
    public RunTimeError(string message) : base(message) { }
}
