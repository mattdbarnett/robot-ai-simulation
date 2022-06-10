using Godot;
using System;

public class root : Node2D
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";
	Random rnd = new Random();
	PackedScene food;
	PackedScene robot;

	float robotSpeed = 0.5f;
	Vector2 velocity = new Vector2(1,1);
	public Godot.Collections.Array<Godot.Area2D> foodList = new Godot.Collections.Array<Godot.Area2D>();
	public Godot.Collections.Array<Vector2> foodPosList = new Godot.Collections.Array<Vector2>();
	public Godot.Collections.Array<robot> robotList = new Godot.Collections.Array<robot>();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		createFood();
		createRobots();
	}
	
	private void createFood()
	{
		food = (PackedScene)ResourceLoader.Load("res://0 Scenes/food.tscn");

		for (int i = 0; i < 10; i++) 
			{
			Random rnd = new Random();
			int x = rnd.Next(0, 1920);
			int y = rnd.Next(0, 1080);
			Area2D newInstance = (Area2D)food.Instance();
			
			foodList.Add(newInstance);
			AddChild(newInstance);
			newInstance.Position = new Vector2(x, y);
			foodPosList.Add(newInstance.Position);
			}

	}

	private void createRobots()
	{
		robot = (PackedScene)ResourceLoader.Load("res://0 Scenes/robot.tscn");

		for (int i = 0; i < 25; i++) 
			{
			Random rnd = new Random();
			int x = rnd.Next(0, 1920);
			int y = rnd.Next(0, 1080);
			robot newInstance = (robot)robot.Instance();
			
			newInstance.setSpeed(Convert.ToSingle(rnd.NextDouble()));
			robotList.Add(newInstance);
			AddChild(newInstance);
			newInstance.Position = new Vector2(x, y);
			}

	}

	public void controlRobots(float delta)
	{	
		robotList[0].LookAt(foodPosList[0]);
		for(int i = 0; i < robotList.Count; i++)
		{
			var currentRobot = robotList[i];
			float closestDistance = 999999;
			Vector2 closestFood = new Vector2(0, 0);
			for(int y = 0; y < foodList.Count; y++) {
				Vector2 currentFood = foodPosList[y];
				float currentDistance = currentRobot.Position.DistanceTo(currentFood);
				if(currentDistance < closestDistance) {
					closestDistance = currentDistance;
					closestFood = currentFood;
				}
			}
			Vector2 distance = closestFood - currentRobot.Position;
			currentRobot.LookAt(closestFood);
			currentRobot.MoveAndSlide(distance * currentRobot.getSpeed());
		}
	}

// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		controlRobots(delta);
		if(Input.IsActionJustPressed("ui_right")) {
			createFood();
		}
	}
}
