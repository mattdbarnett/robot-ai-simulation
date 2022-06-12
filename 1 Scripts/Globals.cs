using Godot;
using System;

public class Globals : Node
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    public int currentRound;
    public String currentMode;

    public Godot.Collections.Array<Area2D> foodList = new Godot.Collections.Array<Area2D>();
    public Godot.Collections.Array<robot> robotList = new Godot.Collections.Array<robot>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        currentRound = 1;
        currentMode = "spring";
    }

    public void iterateRound() {
        currentRound += 1;
        switch(currentMode) {
            case "spring":
                currentMode = "summer";
                break;
            case "summer":
                currentMode = "autumn";
                break;
            case "autumn":
                currentMode = "winter";
                break;
            case "winter":
                currentMode = "spring";
                break;
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
