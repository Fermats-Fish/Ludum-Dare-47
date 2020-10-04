﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    public static UIController instance;

    Robot selectedRobot = null;

    public GameObject robotView;
    public InputField programInputField;
    public Text compileButtonText;
    public Text autoCompleteText;

    int caretPos;

    void Start() {
        if (instance != null) {
            throw new System.Exception("Can't have two UI Controllers");
        }
        instance = this;
    }

    public void SelectRobot(Robot robot) {
        selectedRobot = robot;
        robotView.SetActive(true);
        programInputField.text = robot.programText;
        compileButtonText.text = "Already Compiled";
    }

    public void Compile() {
        try {
            selectedRobot.Compile(programInputField.text);
            compileButtonText.text = "Compiled Successfully";
        } catch (CompileError error) {
            compileButtonText.text = error.Message;
        } catch {
            compileButtonText.text = "Compile Error!";
        }
    }

    public void OnTypeProgram() {
        compileButtonText.text = "Compile";
        Compile();
    }

    public void StartRobot() {
        selectedRobot.TurnOn();
        DeselectRobot();
    }

    public void DeselectRobot() {
        selectedRobot = null;
        robotView.SetActive(false);
    }

    void Update() {

        // Whenever the caret moves, update stuff.
        if (caretPos != programInputField.caretPosition) {

            caretPos = programInputField.caretPosition;

            // We need to find which line the caret is on.
            var line = 0;
            var scanPos = caretPos - 1;
            while (scanPos >= 0) {
                if (programInputField.text[scanPos] == '\n') {
                    line++;
                }
                scanPos--;
            }

            autoCompleteText.text = line.ToString() + ": ";

            // We need to find the word containing the caret.
            int start = caretPos - 1;
            while (start >= 0 && programInputField.text[start] != ' ' && programInputField.text[start] != '\n' && programInputField.text[start] != '\r') {
                start--;
            }
            int end = caretPos;
            while (end < programInputField.text.Length && programInputField.text[end] != ' ' && programInputField.text[end] != '\n' && programInputField.text[end] != '\r') {
                end++;
            }
            var curWord = programInputField.text.Substring(start + 1, end - start - 1);

            // Now try find any function containing that word.
            if (Int32.TryParse(curWord, out _)) {
                autoCompleteText.text += curWord;
            } else {
                // Loop through all functions until this one is contained inside one.
                foreach (var func in Function.functionsArray) {
                    if (func.name.ToLower().Contains(curWord.ToLower())) {
                        autoCompleteText.text += func.name + " - " + func.numArgs + " args.";
                        return;
                    }
                }
                autoCompleteText.text += "No function containing the text: " + curWord;
            }

        }
    }
}
