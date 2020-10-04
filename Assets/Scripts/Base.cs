using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour {
    public Robot robot;

    void OnMouseDown() {
        if (!robot.running) {
            UIController.instance.SelectRobot(robot);
        }
    }

}
