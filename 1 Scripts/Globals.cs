using Godot;
using System;

public class Globals : Node
{
    public int currentRound;
    public String currentMode;
    public int homeSum = 3;

    public Godot.Collections.Array<Area2D> foodList = null;
    public Godot.Collections.Array<robot> robotList = null;
    public Godot.Collections.Array<home> homeList = null;
    public Godot.Collections.Array<Godot.Collections.Array<robot>> homeResidents = null;

    public Godot.Collections.Array<robot> homeRobots = null;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        initGlobals();
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

    public void initGlobals() {
        foodList = new Godot.Collections.Array<Area2D>();
        robotList = new Godot.Collections.Array<robot>();
        homeList = new Godot.Collections.Array<home>();
        homeRobots = new Godot.Collections.Array<robot>();
        homeSum = 3;
        initHomeResidents();
        currentRound = 1;
        currentMode = "spring";
    }

    public void initHomeResidents() {
        homeResidents = new Godot.Collections.Array<Godot.Collections.Array<robot>>();
        for(int i = 0; i < homeSum; i++) {
            Godot.Collections.Array<robot> homeResidentsArray = new Godot.Collections.Array<robot>();
            homeResidents.Add(homeResidentsArray);
        }
    }
}
