using Godot;
using System;

public class robot : KinematicBody2D
{
    Random rnd = new Random();

    // Robot Characteristics
    float speed = 0.5f;
	int hunger = 5;
    int home = 0;
    bool atHome = false;
    public byte[] robotColour = new byte[3];

    // Robot Elements
    Area2D robotArea;
    Sprite robotSprite;
    CollisionShape2D robotCol;
    Globals globals = null;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        globals = (Globals)GetNode("/root/GM");

        // Get elements of current instance
        robotArea = GetNode<Area2D>("robotarea");
        robotSprite = GetNode<Sprite>("robotSprite");
        robotCol = GetNode<CollisionShape2D>("robotCol");

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
        } else if(area.Name.Contains("homeroot")) {
            if((globals.currentMode == "winter") && (globals.foodList.Count == 0)) {
                globals.homeResidents[getHome()].Add(this);
                robotCol.SetDeferred("disabled", true);
                setAtHome(true);
            }
        }
    }

    public void _on_robotarea_area_exited(Area2D area)  {
        if(area.Name.Contains("homeroot")) {
            robotCol.SetDeferred("disabled", false);
            setAtHome(false);
        }
    }

    public float getSpeed() {
        return speed;
    }

    public void setSpeed(float newSpeed) {
        speed = newSpeed;
    }

    public int getHunger() {
        return hunger;
    }

    public void setHunger(int newHunger) {
        hunger = newHunger;
    }

    public int getHome() {
        return home;
    }

    public void setHome(int newHome) {
        home = newHome;
    }

    public bool getAtHome() {
        return atHome;
    }

    public void setAtHome(bool state) {
        atHome = state;
    }

    public void setColour(byte[] newColour) {
        robotColour = newColour;
    }

    public void killSelf() {
        globals.robotList.Remove(this);
        QueueFree();
    }

}
