/*
- - - - - - - - - - - - - - -
 Title: Menu Script
 Author: Matt Barnett
 Created: 21/06/2022
 Last Modified: 22/06/2022
- - - - - - - - - - - - - - -
*/

using Godot;
using System;

public class menu : Control
{
    Button btnStart = null;
    Button btnHelp = null;

    public override void _Ready()
    {
        btnStart = (Button)GetNode("menumain/btnstart");
        btnHelp = (Button)GetNode("menumain/btnhelp");
    }

    public void _on_btnstart_pressed()
    {
        GetTree().ChangeScene("res://0 Scenes/root.tscn");
    }

    public void _on_btnhelp_pressed()
    {
        OS.ShellOpen("https://github.com/mattdbarnett/ai-robots-project");
    }



    public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent.IsActionPressed("exit"))
        {
            GetTree().Quit();
        }

        if(inputEvent.IsActionPressed("fullscreen")) 
        {
            OS.WindowFullscreen = !OS.WindowFullscreen;
            OS.WindowMaximized = OS.WindowFullscreen;
        }
    }

}
