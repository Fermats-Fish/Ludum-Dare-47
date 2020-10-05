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
    public Button leftButton;
    public Text leftButtonText;
    public Text autoCompleteText;
    public Button startButton;
    public Text startButtonText;
    public Image startButtonImage;

    public Text resourcesText;

    public Text spawnResourcesButtonText;
    public Text buyRobotButtonText;

    public GameObject scriptingReferenceRoot;
    public Transform scriptingReferenceMenu;
    public GameObject scriptingReferencePrefab;

    public Toggle autoStartToggle;

    int caretPos;

    void Start() {
        if (instance != null) {
            throw new System.Exception("Can't have two UI Controllers");
        }
        instance = this;
        UpdateResourcesDisplay();
        spawnResourcesButtonText.text = "Spawn Resources - $" + GameController.SPAWN_RESOURCES_COST;
        buyRobotButtonText.text = "Buy Robot - $" + GameController.BUY_ROBOT_COST;

        // Init scripting reference.
        foreach (var function in Function.functionsArray) {
            var text = Instantiate(scriptingReferencePrefab, scriptingReferenceMenu).GetComponent<Text>();
            text.text = function.GetDocumentation();
        }
    }

    public void SelectRobot(Robot robot) {
        selectedRobot = robot;
        robotView.SetActive(true);
        programInputField.text = robot.programText;
        autoStartToggle.isOn = robot.autoStart;
        UpdateDisplay();
    }

    public void UpdateAutoStart() {
        selectedRobot.autoStart = autoStartToggle.isOn;
    }

    public void OnLeftButtonClick() {
        if (selectedRobot.running) {
            // Salvage
            selectedRobot.Rescue();
        } else {
            // Compile
            Compile();
        }
    }

    public void Compile() {
        try {
            selectedRobot.Compile(programInputField.text);
            leftButtonText.text = "Compiled Successfully";
        } catch (CompileError error) {
            leftButtonText.text = error.Message;
        } catch {
            leftButtonText.text = "Compile Error!";
        }
    }

    public void OnTypeProgram() {
        leftButtonText.text = "Compile";
        Compile();
    }

    public void StartRobot() {
        selectedRobot.TurnOn();
    }

    public void UpdateDisplay() {
        CameraController.instance.disableKeyboardControls = !selectedRobot.running;
        programInputField.readOnly = selectedRobot.running;
        startButton.interactable = !selectedRobot.running;
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
        leftButtonText.text = selectedRobot.running ? "Rescue Robot - $" + selectedRobot.GetRescueCost() : "Already Compiled";
        UpdateLeftButtonClickable();
    }

    public void UpdateLeftButtonClickable() {
        leftButton.interactable = selectedRobot != null && (!selectedRobot.running || selectedRobot.GetRescueCost() <= GameController.Money);
    }

    public void UpdateResourcesDisplay() {
        resourcesText.text = "Score: " + GameController.Score + "\n" + "Money: $" + GameController.Money;
        UpdateLeftButtonClickable();
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
                        autoCompleteText.text += func.GetDocumentation();
                        return;
                    }
                }
                autoCompleteText.text += "No function containing the text: " + curWord;
            }

        }
    }

    public void SpawnResources() {
        if (GameController.SPAWN_RESOURCES_COST <= GameController.Money) {
            GameController.Money -= GameController.SPAWN_RESOURCES_COST;
            GameController.instance.SpawnExtraResources();
        }
    }

    public void BuyRobot() {
        if (GameController.BUY_ROBOT_COST <= GameController.Money) {
            GameController.Money -= GameController.BUY_ROBOT_COST;
            GameController.instance.SpawnRobot();
        }
    }

    public void ToggleScriptingReference() {
        var wasOpen = scriptingReferenceRoot.activeInHierarchy;
        scriptingReferenceRoot.SetActive(!wasOpen);
        CameraController.instance.DisableMouseControls = !wasOpen;
    }
}
