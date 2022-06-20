using Godot;
using System;

public class root : Node2D
{
	Random rnd = new Random();
	Globals globals = null;
	bool roundOver = false;
	Godot.Collections.Array<Godot.Vector2> homePosList = 
	new Godot.Collections.Array<Godot.Vector2>();

	Label roundLabel = null;
	Label seasonLabel = null;
	Label robotsLabel = null;
	Label fastestLabel = null;
	Label mostHLabel = null;
	Label leastHLabel = null;

	// Instanced elements
	PackedScene food;
	PackedScene robot;
	PackedScene home;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_EnterTree();
		globals = (Globals)GetNode("/root/GM");

		roundLabel = (Label)GetNode("UI/infoPanel/roundLabel");
		seasonLabel = (Label)GetNode("UI/infoPanel/seasonLabel");
		robotsLabel = (Label)GetNode("UI/infoPanel/robotsLabel");

		fastestLabel = (Label)GetNode("UI/statsPanel/fastestLabel");
		mostHLabel = (Label)GetNode("UI/statsPanel/mostHLabel");
		leastHLabel = (Label)GetNode("UI/statsPanel/leastHLabel");
		
		createFood(10);
		createHomes();
		createRobots();
		activateHomes(false);
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
			switch(globals.currentMode) {
				case "winter":
					createFood(3);
					roundOver = false;
					break;
				case "spring":
					createFood(10);
					roundOver = false;
					break;
				case "summer":
					activateHomes(false);
					createFood(10);
					roundOver = false;
					break;
				case "autumn":
					createFood(10);
					roundOver = false;
					break;
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

			Area2D newInstance = (Area2D)food.Instance();
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
			int home = rnd.Next(0, globals.homeList.Count);

			robot newInstance = (robot)robot.Instance();

			newInstance.setSpeed(Convert.ToSingle(rnd.NextDouble() * 500));
			newInstance.setHome(home);
			newInstance.randomColour();

			newInstance.AddToGroup("ROBOTS");
			globals.robotList.Add(newInstance);
			AddChild(newInstance);
			newInstance.Position = new Vector2(x, y);
		}

	}

	private void createHomes()
	{
		home = (PackedScene)ResourceLoader.Load("res://0 Scenes/home.tscn");

		for (int i = 0; i < globals.homeSum; i++) {
			// Create random coordinates for where to spawn current robot
			Random rnd = new Random();
			int x = rnd.Next(0, 1920);
			int y = rnd.Next(0, 1080);

			home newInstance = (home)home.Instance();
			globals.homeList.Add(newInstance);
			AddChild(newInstance);
			newInstance.Position = new Vector2(x, y);
			homePosList.Add(newInstance.Position);
		}
	}

	private void createChildren() 
	{
		for(int i = 0; i < globals.homeSum; i++) {
			var currentHomeResidents = globals.homeResidents[i];
			if(currentHomeResidents.Count >= 2) {

				robot newChild = (robot)robot.Instance();

				float newSpeed;
				newSpeed = ((currentHomeResidents[0].getSpeed() + 
				currentHomeResidents[1].getSpeed()) / 2) + 
				(float)(rnd.Next(-15, 30));

				byte[] newColour = new byte[3];
				for(int col = 0; col < 3; col++) {
					newColour[col] = (byte)
					(((currentHomeResidents[0].robotColour[col] + 
					currentHomeResidents[1].robotColour[col]) 
					/ 2) + rnd.Next(15, 15));
				}

				newChild.setHome(i);
				newChild.setSpeed(newSpeed);
				newChild.setColour(newColour);

				newChild.AddToGroup("ROBOTS");
				globals.robotList.Add(newChild);
				AddChild(newChild);
				newChild.Position = currentHomeResidents[0].Position;
			}
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
			activateHomes(true);
			for(int i = 0; i < globals.robotList.Count; i++) {
				var currentRobot = globals.robotList[i];
				if(!currentRobot.getAtHome()) {
					Vector2 housePos = homePosList[currentRobot.getHome()];
					Vector2 direction = currentRobot.Position.DirectionTo(housePos);
					currentRobot.LookAt(housePos);
					Vector2 velocity = direction * currentRobot.getSpeed();
					currentRobot.MoveAndSlide(velocity);
				}
			}
			if(globals.homeRobots.Count == globals.robotList.Count) {
				createChildren();
				globals.resetHomeResidents();
				globals.homeRobots.Clear();
				roundOver = true;
			}
		}
	}

	public void updateUI() {
		roundLabel.Text = globals.currentRound.ToString();
		seasonLabel.Text = globals.currentMode.Capitalize();
		robotsLabel.Text = globals.robotList.Count.ToString();

		if(roundOver == true) {
			float fastestSpeed = 0;
			int mostHealth = 0;
			int leastHealth = 9999;
			for(int id = 0; id < globals.robotList.Count; id++) {
				robot currentRobot = globals.robotList[id];
				if(currentRobot.getSpeed() > fastestSpeed) {
					fastestSpeed = globals.robotList[id].getSpeed();
				}
				if(currentRobot.getHunger() > mostHealth) {
					mostHealth = currentRobot.getHunger();
				} else if(currentRobot.getHunger() < leastHealth) {
					leastHealth = currentRobot.getHunger();
				}
			}
			fastestLabel.Text = fastestSpeed.ToString();
			mostHLabel.Text = mostHealth.ToString();
			leastHLabel.Text = leastHealth.ToString();
		}
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

	public void activateHomes(bool activate) {
		for(int id = 0; id < globals.homeList.Count; id++) {
			globals.homeList[id].setMonitoring(activate);
		}
	}

}
