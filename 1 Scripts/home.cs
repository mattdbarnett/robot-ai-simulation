using Godot;
using System;

public class home : Area2D
{
    Area2D colArea;
     Globals globals = null;

    public override void _Ready() {
        globals = (Globals)GetNode("/root/GM");
        colArea = GetNode<Area2D>("colarea");
    }

    public void setMonitoring(bool value) {
        Monitorable = value;
        Monitoring = value;
        colArea.Monitorable = value;
        colArea.Monitoring = value;
    }

    public void _on_homeroot_body_entered(CollisionObject2D obj) {
        if((globals.currentMode == "winter") && 
        (globals.foodList.Count == 0) && 
        obj.Name.Contains("robotroot")) {
            var objcol = obj.GetNode("robotCol");
            objcol.SetDeferred("disabled", true);
        }
    }

    public void _on_colarea_body_entered(CollisionObject2D obj) {
        if((globals.currentMode == "winter") && 
        (globals.foodList.Count == 0) && 
        obj.Name.Contains("robotroot")) {
            robot currentRobot = (robot)obj;
            globals.homeResidents[globals.homeList.IndexOf(this)].Add(currentRobot);
            if(!globals.homeRobots.Contains(currentRobot)) {
                globals.homeRobots.Add(currentRobot);
            }
        }
    }
}
