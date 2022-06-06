using Godot;
using System;

public class root : Node2D
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";
	Random rnd = new Random();
	PackedScene food;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		createFood();
	}
	
	private void createFood()
	{
		food = (PackedScene)ResourceLoader.Load("res://0 Scenes/food.tscn");

		Random rnd = new Random();
		int x = rnd.Next(0, 1920);
		int y = rnd.Next(0, 1080);
		Area2D newInstance = (Area2D)food.Instance();
		
		AddChild(newInstance);
		newInstance.Position = new Vector2(x, y);

	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
