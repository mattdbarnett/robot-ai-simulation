using Godot;
using System;

public class homecenter : Area2D
{
    Globals globals = null;
    home parent = null;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        globals = (Globals)GetNode("/root/GM");
        parent = (home)GetParent();
    }

    public void _on_homecenter_area_entered(Area2D area) {

        if(area.Name.Contains("robotarea")) {
            robot obj = (robot)area.GetParent();
            if((globals.currentMode == "winter") && 
            (globals.foodList.Count == 0) && 
            obj.Name.Contains("robotroot")) {
                robot currentRobot = (robot)obj;
                globals.homeResidents[globals.homeList.IndexOf(parent)].Add(currentRobot);
                if(!globals.homeRobots.Contains(currentRobot)) {
                    globals.homeRobots.Add(currentRobot);
                    currentRobot.setAtHome(true);
                }
            }
        }
    }

    public void _on_homecenter_area_exited(Area2D area) {
        if((area.Name.Contains("robotarea")) &&
            (globals.currentMode != "winter")) {
            robot obj = (robot)area.GetParent();
            obj.setAtHome(false);
        }
    }
}
