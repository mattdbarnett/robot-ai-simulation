using Godot;
using System;

public class robot : KinematicBody2D
{
    Random rnd = new Random();

    // Robot Characteristics
    float speed = 0.5f;
	int hunger = 0;
    byte[] robotColour = new byte[3];

    // Robot Elements
    Area2D robotArea;
    Sprite robotSprite;
    Globals globals = null;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        globals = (Globals)GetNode("/root/GM");

        // Get elements of current instance
        robotArea = GetNode<Area2D>("robotarea");
        robotSprite = GetNode<Sprite>("robotSprite");

        // Pick random colour for current Robot
        robotColour[0] = (byte)rnd.Next(0, 255);
        robotColour[1] = (byte)rnd.Next(0, 255);
        robotColour[2] = (byte)rnd.Next(0, 255);
        
        robotSprite.Modulate = Color.Color8(
            robotColour[0],
            robotColour[1],
            robotColour[2]);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    // public override void _Process(float delta)
    // {
        
    // }

    public void _on_Area2D_area_entered(Area2D area) {
        if(area.Name.Contains("foodroot")) {
            hunger += 1;
            globals.foodList.Remove(area);
            area.QueueFree();
        }
    }

    public void _on_robotarea_body_entered(Area2D area) {
        if(area.Name == "foodroot") {
            hunger += 1;
            globals.foodList.Remove(area);
            area.QueueFree();
        }
    }

    public float getSpeed() {
        return speed;
    }

    public void setSpeed(float newSpeed) {
        speed = newSpeed;
    }

    public float getHunger() {
        return hunger;
    }

    public void setHunger(int newHunger) {
        hunger = newHunger;
    }

    public void killSelf() {
        globals.robotList.Remove(this);
        QueueFree();
    }

}
