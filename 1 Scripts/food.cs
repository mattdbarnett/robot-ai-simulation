using Godot;
using System;

public class food : Area2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    int id;
    Globals globals = null;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        globals = (Globals)GetNode("/root/GM");
    }

    public void _on_foodroot_area_entered(Area2D area)
    {
        if(area.Name == "robotarea") {
            GD.Print(id);
            globals.foodList.Remove(this);
            QueueFree();
        }
    }

    public int getID() {
        return id;
    }

    public void setID(int newID) {
        id = newID;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
