using Godot;
using System;

public class root : Node2D
{
	Random rnd = new Random();
	Globals globals = null;

	// Instanced elements
	PackedScene food;
	PackedScene robot;

	// List of all robot elements
	public Godot.Collections.Array<robot> robotList = new Godot.Collections.Array<robot>();

	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_EnterTree();
		globals = (Globals)GetNode("/root/GM");
		
		createFood();
		createRobots();
	}
	
	private void createFood()
	{
		food = (PackedScene)ResourceLoader.Load("res://0 Scenes/food.tscn");

		for (int i = 0; i < 10; i++) {

			// Create random coordinates for where to spawn current food
			Random rnd = new Random();
			int x = rnd.Next(0, 1920);
			int y = rnd.Next(0, 1080);

			food newInstance = (food)food.Instance();
			globals.foodList.Add(newInstance);
			AddChild(newInstance);
			newInstance.Position = new Vector2(x, y);
		}

	}

	private void createRobots()
	{
		robot = (PackedScene)ResourceLoader.Load("res://0 Scenes/robot.tscn");

		for (int i = 0; i < 25; i++) {

			// Create random coordinates for where to spawn current robot
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

	public void controlRobots()
	{	
		// Find closest food, look at it and move towards it
		for(int i = 0; i < robotList.Count; i++) {
			var currentRobot = robotList[i];
			float closestDistance = 999999;
			Vector2 closestFood = new Vector2(0, 0);
			for(int y = 0; y < globals.foodList.Count; y++) {
				var currentFood = globals.foodList[y];
				float currentDistance = currentRobot.Position.DistanceTo(currentFood.Position);
				if(currentDistance < closestDistance) {
					closestDistance = currentDistance;
					closestFood = currentFood.Position;
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
		if(globals.foodList.Count > 0) {
			controlRobots();
		}
		if(Input.IsActionJustPressed("ui_right")) {
			createFood();
		}
	}
}
