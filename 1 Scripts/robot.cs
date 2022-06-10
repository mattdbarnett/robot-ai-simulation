using Godot;
using System;

public class robot : KinematicBody2D
{
    float speed = 0.5f;
	

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public float getSpeed() {
        return speed;
    }

    public void setSpeed(float newSpeed) {
        speed = newSpeed;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
// public override void _Process(float delta)
//{
//
//}
}
