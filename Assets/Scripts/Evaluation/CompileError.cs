using System;

public class CompileError : Exception {
    public CompileError(string message) : base(message) { }
}
