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
}
