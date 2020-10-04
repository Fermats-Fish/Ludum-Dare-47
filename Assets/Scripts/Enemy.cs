using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity {

    protected override void Start() {
        base.Start();
        direction = directions[Random.Range(0, 4)].ToVector3();
        target = GameController.instance.RandomEmptyPos();
        transform.position = GameController.instance.TileToWorldCoord(target);
    }

    protected override void RunProgram() {
        MoveDirection(Random.Range(0, 4));
    }
}
