using Godot;
using System;

public class root : Node2D
{
	Random rnd = new Random();
	Globals globals = null;
	bool roundOver = false;

	Label roundLabel = null;
	Label seasonLabel = null;
	Label robotsLabel = null;

	// Instanced elements
	PackedScene food;
	PackedScene robot;

	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_EnterTree();
		globals = (Globals)GetNode("/root/GM");
		roundLabel = (Label)GetNode("UI/statsPanel/roundLabel");
		seasonLabel = (Label)GetNode("UI/statsPanel/seasonLabel");
		robotsLabel = (Label)GetNode("UI/statsPanel/robotsLabel");
		
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
			newInstance.setSpeed(Convert.ToSingle(rnd.NextDouble() * 250));
			globals.robotList.Add(newInstance);
			AddChild(newInstance);
			newInstance.Position = new Vector2(x, y);
		}

	}

	public void controlRobots()
	{	
		if(globals.currentMode == "winter") {

			var deadRobotList = new Godot.Collections.Array<robot>();

			for(int i = 0; i < globals.robotList.Count; i++) {
				var currentRobot = globals.robotList[i];
				if(currentRobot.getHunger() <= 0) {
					deadRobotList.Add(currentRobot);
				}
			}

			for(int x = 0; x < deadRobotList.Count; x++)  {
				deadRobotList[x].killSelf();
			}

			roundOver = true;
		} else {
			// Find closest food, look at it and move towards it
			for(int i = 0; i < globals.robotList.Count; i++) {
				var currentRobot = globals.robotList[i];
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
				Vector2 direction = currentRobot.Position.DirectionTo(closestFood);
				currentRobot.LookAt(closestFood);
				Vector2 velocity = direction * currentRobot.getSpeed();
				currentRobot.MoveAndSlide(velocity);
			}
		}
	}

// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{

		controlRobots();
		updateUI();

		if((globals.currentMode != "winter") && (globals.foodList.Count == 0)) {
			roundOver = true;
		}

		if(roundOver == true) {
			globals.iterateRound();
			if(globals.currentMode == "winter") {
				roundOver = false;
			} else {
				createFood();
				roundOver = false;
			}
		}

		if(Input.IsActionJustPressed("ui_right")) {
			createFood();
		}
	}

	public void updateUI() {
		roundLabel.Text = globals.currentRound.ToString();
		seasonLabel.Text = globals.currentMode.Capitalize();
		robotsLabel.Text = globals.robotList.Count.ToString();
	}
}
