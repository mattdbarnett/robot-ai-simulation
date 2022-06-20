using Godot;
using System;

public class Globals : Node
{
    public int currentRound;
    public String currentMode;
    public int homeSum = 3;

    public Godot.Collections.Array<Area2D> foodList = new Godot.Collections.Array<Area2D>();
    public Godot.Collections.Array<robot> robotList = new Godot.Collections.Array<robot>();
    public Godot.Collections.Array<home> homeList = new Godot.Collections.Array<home>();
    public Godot.Collections.Array<Godot.Collections.Array<robot>> homeResidents = 
    new Godot.Collections.Array<Godot.Collections.Array<robot>>();

    public Godot.Collections.Array<robot> homeRobots = new Godot.Collections.Array<robot>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        currentRound = 1;
        currentMode = "spring";

        for(int i = 0; i < homeSum; i++) {
            Godot.Collections.Array<robot> homeResidentsArray = 
            new Godot.Collections.Array<robot>();
            homeResidents.Add(homeResidentsArray);
        }
    }

    public void resetHomeResidents() {
        for(int i = 0; i < homeSum; i++) {
            homeResidents[i].Clear();
        }
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
}
