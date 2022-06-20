using Godot;
using System;

public class home : Area2D
{
    homecenter center;
    Globals globals = null;

    public override void _Ready() {
        globals = (Globals)GetNode("/root/GM");
        center = GetNode<homecenter>("homecenter");
    }

    public void setMonitoring(bool value) {
        Monitorable = value;
        Monitoring = value;
        center.Monitorable = value;
        center.Monitoring = value;
    }

    public void _on_homeroot_body_entered(CollisionObject2D obj) {
        if((globals.currentMode == "winter") && 
        (globals.foodList.Count == 0) && 
        obj.Name.Contains("robotroot")) {
            var objcol = obj.GetNode("robotCol");
            objcol.SetDeferred("disabled", true);
        }
    }

    public void _on_homeroot_area_exited(Area2D area) {
        if(area.Name.Contains("robotarea")) {
            robot obj = (robot)area.GetParent();
            if(obj.Name.Contains("robotroot") &&
            (globals.currentMode != "winter")) {
                var objcol = obj.GetNode("robotCol");
                objcol.SetDeferred("disabled", false);
            }
        }
    }
}
