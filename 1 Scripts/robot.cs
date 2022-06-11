using Godot;
using System;

public class robot : KinematicBody2D
{
    float speed = 0.5f;
	int hunger = 5;

    Area2D robotArea;
    Sprite robotSprite;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        robotArea = GetNode<Area2D>("robotarea");
        robotSprite = GetNode<Sprite>("robotSprite");
        //robotSprite.Modulate = Color.Color8(100, 255, 100);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        
    }

    public void _on_Area2D_area_entered(Area2D area)
    {
        if(area.Name == "foodroot") {
            hunger += 1;
        }
    }

    public float getSpeed() {
        return speed;
    }

    public void setSpeed(float newSpeed) {
        speed = newSpeed;
    }

}
