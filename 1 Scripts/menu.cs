using Godot;
using System;

public class menu : Control
{
    Button btnStart = null;
    Button btnHelp = null;

    public override void _Ready() {
        btnStart = (Button)GetNode("menumain/btnstart");
        btnHelp = (Button)GetNode("menumain/btnhelp");
    }

    public void _on_btnstart_pressed() {
        GetTree().ChangeScene("res://0 Scenes/root.tscn");
    }

    public void _on_btnhelp_pressed() {
        OS.ShellOpen("https://github.com/mattdbarnett/ai-robots-project");
    }



//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
