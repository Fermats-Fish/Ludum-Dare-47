using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    public static UIController instance;

    public Robot selectedRobot { get; protected set; }

    public GameObject robotView;
    public InputField programInputField;
    public Button compileButton;
    public Text compileButtonText;
    public Text autoCompleteText;
    public Button startButton;
    public Text startButtonText;
    public Image startButtonImage;

    public Text resourcesText;

    int caretPos;

    void Start() {
        if (instance != null) {
            throw new System.Exception("Can't have two UI Controllers");
        }
        instance = this;
        UpdateResourcesDisplay();
    }

    public void SelectRobot(Robot robot) {
        selectedRobot = robot;
        robotView.SetActive(true);
        programInputField.text = robot.programText;
        compileButtonText.text = "Already Compiled";
        UpdateDisplay();
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
        UpdateDisplay();
    }

    public void UpdateDisplay() {
        CameraController.instance.disableKeyboardControls = !selectedRobot.running;
        programInputField.readOnly = selectedRobot.running;
        startButton.interactable = !selectedRobot.running;
        compileButton.interactable = !selectedRobot.running;
        string statusText;
        Color color;
        if (selectedRobot.running) {
            if (selectedRobot.error) {
                autoCompleteText.text = selectedRobot.lastErrorMessage;
                statusText = "Error";
                color = new Color(1f, 0.5f, 0.5f);
            } else {
                statusText = "Running";
                color = new Color(0.5f, 1f, 0.5f);
            }
        } else {
            statusText = "Start Robot";
            color = new Color(0.7f, 0.7f, 1f);
        }
        startButtonText.text = statusText;
        startButtonImage.color = color;
    }

    public void UpdateResourcesDisplay() {
        resourcesText.text = "Score: " + GameController.Score + "\n" + "Money: $" + GameController.Money;
    }

    public void DeselectRobot() {
        selectedRobot = null;
        robotView.SetActive(false);
        CameraController.instance.disableKeyboardControls = false;
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
                        autoCompleteText.text += func.name;
                        if (func.numArgs > 0) {
                            autoCompleteText.text += " (" + func.numArgs + " arg";
                            if (func.numArgs != 1) {
                                autoCompleteText.text += "s";
                            }
                            autoCompleteText.text += ")";
                        }
                        autoCompleteText.text += " - " + func.description;
                        return;
                    }
                }
                autoCompleteText.text += "No function containing the text: " + curWord;
            }

        }
    }
}
