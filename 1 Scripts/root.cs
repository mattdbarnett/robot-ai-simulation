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
	PackedScene home;
	public Godot.Collections.Array<Area2D> homeList = new Godot.Collections.Array<Area2D>();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_EnterTree();
		globals = (Globals)GetNode("/root/GM");
		roundLabel = (Label)GetNode("UI/statsPanel/roundLabel");
		seasonLabel = (Label)GetNode("UI/statsPanel/seasonLabel");
		robotsLabel = (Label)GetNode("UI/statsPanel/robotsLabel");
		
		createFood(10);
		createHomes();
		createRobots();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(float delta)
	{

		controlRobots();
		updateUI();

		if((globals.foodList.Count == 0) && (globals.currentMode != "winter")) {
			roundOver = true;
		}

		if(roundOver == true) {
			deathCheck();
			drainHunger();
			globals.iterateRound();
			if(globals.currentMode == "winter") {
				createFood(3);
				roundOver = false;
			} else {
				createFood(10);
				roundOver = false;
			}
		}

		if(Input.IsActionJustPressed("ui_right")) {
			createFood(10);
		}
	}
	
	private void createFood(int sum)
	{
		food = (PackedScene)ResourceLoader.Load("res://0 Scenes/food.tscn");

		for (int i = 0; i < sum; i++) {

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
			int home = rnd.Next(0, homeList.Count);

			robot newInstance = (robot)robot.Instance();
			newInstance.setSpeed(Convert.ToSingle(rnd.NextDouble() * 500));
			newInstance.setHome(home);
			globals.robotList.Add(newInstance);
			AddChild(newInstance);
			newInstance.Position = new Vector2(x, y);
		}

	}

	private void createHomes()
	{
		home = (PackedScene)ResourceLoader.Load("res://0 Scenes/home.tscn");

		for (int i = 0; i < 3; i++) {

			// Create random coordinates for where to spawn current robot
			Random rnd = new Random();
			int x = rnd.Next(0, 1920);
			int y = rnd.Next(0, 1080);

			Area2D newInstance = (Area2D)home.Instance();
			homeList.Add(newInstance);
			AddChild(newInstance);
			newInstance.Position = new Vector2(x, y);
		}
	}

	public void controlRobots()
	{	
		// Find closest food, look at it and move towards it
		if((globals.foodList.Count > 0) | (globals.currentMode != "winter")) {
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
		} else {
			int robotsAtHome = 0;
			for(int i = 0; i < globals.robotList.Count; i++) {
				var currentRobot = globals.robotList[i];
				if(currentRobot.getAtHome() == false) {
					Vector2 housePos = homeList[currentRobot.getHome()].Position;
					Vector2 direction = currentRobot.Position.DirectionTo(housePos);
					currentRobot.LookAt(housePos);
					Vector2 velocity = direction * currentRobot.getSpeed();
					currentRobot.MoveAndSlide(velocity);
				} else {
					robotsAtHome += 1;
				}
			}
			if(robotsAtHome == globals.robotList.Count) {
				roundOver = true;
			}
		}
	}

	public void updateUI() {
		roundLabel.Text = globals.currentRound.ToString();
		seasonLabel.Text = globals.currentMode.Capitalize();
		robotsLabel.Text = globals.robotList.Count.ToString();
	}

	public void drainHunger() {
		for(int i = 0; i < globals.robotList.Count; i++) {
			var currentRobot = globals.robotList[i];
			int drain = 1;
			if(globals.currentMode == "winter") {
				drain = 2;
			}
			currentRobot.setHunger(currentRobot.getHunger() - drain);
		}
	}

	public void deathCheck() {
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
	}

	public bool endOfYearCheck() {
		if((globals.foodList.Count == 0) && (globals.currentMode == "winter")) {
			return true; //It is the end of the year
		} else {
			return false;
		}
	}
}
