using Godot;
using System;

public class food : Area2D
{
    Globals globals = null;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        globals = (Globals)GetNode("/root/GM");
    }

    public void _on_foodroot_area_entered(Area2D area)
    {
        //If robot eats me, delete self
        if(area.Name == "robotarea") {
            globals.foodList.Remove(this);
            QueueFree();
        }
    }
}
