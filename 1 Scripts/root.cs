/*
- - - - - - - - - - - - - - -
 Title: Root Script
 Author: Matt Barnett
 Created: 6/06/2022
 Last Modified: 22/06/2022
- - - - - - - - - - - - - - -
*/

using Godot;
using System;

public class root : Node2D
{

    // Simulation Vars
    Random rnd = new Random();
    Globals globals = null;
    bool roundOver = false;
    int countdownValue = 25;
    Godot.Collections.Array<Godot.Vector2> homePosList =
    new Godot.Collections.Array<Godot.Vector2>();
    int robotStartRate = 25;
    int foodRate = 10;
    int simuWidth = 1920;
    int simuHeight = 1080;

    // Simulation UI Nodes
    Label roundLabel = null;
    Label seasonLabel = null;
    Label robotsLabel = null;
    Label fastestLabel = null;
    Label mostHLabel = null;
    Label leastHLabel = null;
    Label countdownLabel = null;
    Timer countdownTimer = null;
    Button roundStatsBtn = null;
    Panel roundStatsPanel = null;
    Button robotStatsBtn = null;
    Panel robotStatsPanel = null;

    // Sitmulation UI Vars
    bool roundStatsOpen = true;
    Vector2 roundStatsOpenPos = new Vector2(1856, 130);
    Vector2 roundStatsClosedPos = new Vector2(1856, 0);
    bool robotStatsOpen = true;
    Vector2 robotStatsOpenPos = new Vector2(1600, 130);
    Vector2 robotStatsClosedPos = new Vector2(1600, 0);
    StyleBox openStyle = (StyleBox)GD.Load("res://3 Styling/panelOpenBtnTheme.tres");
    StyleBox openInvertStyle = (StyleBox)GD.Load("res://3 Styling/panelOpenInvertBtnTheme.tres");
    StyleBox closeStyle = (StyleBox)GD.Load("res://3 Styling/panelCloseBtnTheme.tres");
    StyleBox closeInvertStyle = (StyleBox)GD.Load("res://3 Styling/panelCloseInvertBtnTheme.tres");
    Color colourWhite = Color.Color8(255, 255, 255);
    Color colourYellow = Color.Color8(255, 255, 94);
    Color colourRed = Color.Color8(255, 38, 38);

    // Instanced simulation scenes
    PackedScene food;
    PackedScene robot;
    PackedScene home;

    // -- Ready Func
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _EnterTree();
        globals = (Globals)GetNode("/root/GM");

        roundLabel = (Label)GetNode("UI/roundStats/infoPanel/roundValue");
        seasonLabel = (Label)GetNode("UI/roundStats/infoPanel/seasonValue");
        robotsLabel = (Label)GetNode("UI/roundStats/infoPanel/robotsValue");

        fastestLabel = (Label)GetNode("UI/robotStats/infoPanel/fastestValue");
        mostHLabel = (Label)GetNode("UI/robotStats/infoPanel/mostHValue");
        leastHLabel = (Label)GetNode("UI/robotStats/infoPanel/leastHValue");

        countdownLabel = (Label)GetNode("UI/countdownStats/countdownPanel/countdownLabel");
        countdownTimer = (Timer)GetNode("UI/countdownStats/countdownTimer");

        roundStatsBtn = (Button)GetNode("UI/roundStats/closeRoundButton");
        roundStatsPanel = (Panel)GetNode("UI/roundStats/infoPanel");
        robotStatsBtn = (Button)GetNode("UI/robotStats/closeRobotButton");
        robotStatsPanel = (Panel)GetNode("UI/robotStats/infoPanel");

        createFood(10);
        createHomes();
        createRobots();
        activateHomes(false);
    }

    // -- Input Func
    // Called whenever user input is made.
    public override void _Input(InputEvent inputEvent)
    {

        if (Input.IsActionJustReleased("exit"))
        {
            this.QueueFree();
            GetTree().ChangeScene("res://0 Scenes/menu.tscn");
        }

		if(inputEvent.IsActionPressed("fullscreen")) 
        {
            OS.WindowFullscreen = !OS.WindowFullscreen;
            OS.WindowMaximized = OS.WindowFullscreen;
        }

        if (Input.IsActionJustReleased("restart"))
        {
            globals.initGlobals();
			globals.currentRound = 0;
			globals.currentMode = "winter";
            GetTree().ReloadCurrentScene();
            roundOver = false;
        }


    }

    // -- Process Func
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {

        controlRobots();
        updateUI();

        if ((globals.foodList.Count == 0) && (globals.currentMode != "winter"))
        {
            roundOver = true;
        }

        if (countdownValue <= 0)
        {
            for (int id = 0; id < globals.robotList.Count; id++)
            {
                if (!globals.robotList[id].getAtHome())
                {
                    globals.robotList[id].setHunger(-10);
                }
            }
            roundOver = true;
        }

        if (roundOver == true)
        {

            // End of season functions
            switch (globals.currentMode)
            {
                case "winter":
                    createChildren();
                    globals.homeRobots.Clear();
                    globals.initHomeResidents();
                    break;
            }

            countdownValue = 25;
            deathCheck();
            drainHunger();
            globals.iterateRound();

            // Start of season functions
            switch (globals.currentMode)
            {
                case "winter":
                    createFood((int)(foodRate * 0.3));
                    roundOver = false;
                    break;
                case "spring":
                    createFood(foodRate);
                    roundOver = false;
                    break;
                case "summer":
                    activateHomes(false);
                    createFood(foodRate);
                    roundOver = false;
                    break;
                case "autumn":
                    createFood(foodRate);
                    roundOver = false;
                    break;
            }
        }
    }

    // -- Simulation Init Funcs
    // Populates simulation with randomly placed food.
    private void createFood(int sum)
    {
        food = (PackedScene)ResourceLoader.Load("res://0 Scenes/food.tscn");
        globals.foodList.Clear();

        for (int i = 0; i < sum; i++)
        {

            // Create random coordinates for where to spawn current food
            Random rnd = new Random();
            int x = rnd.Next(0, simuWidth);
            int y = rnd.Next(0, simuHeight);

            Area2D newInstance = (Area2D)food.Instance();
            globals.foodList.Add(newInstance);
            AddChild(newInstance);
            newInstance.Position = new Vector2(x, y);
        }

    }

    // Creates the first inital set of simulation robots.
    private void createRobots()
    {
        robot = (PackedScene)ResourceLoader.Load("res://0 Scenes/robot.tscn");

        for (int i = 0; i < robotStartRate; i++)
        {

            // Create random coordinates for where to spawn current robot
            Random rnd = new Random();
            int x = rnd.Next(0, simuWidth);
            int y = rnd.Next(0, simuHeight);
            int home = rnd.Next(0, 2);

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

    // Creates the homes that robots enter during winter
    private void createHomes()
    {
        home = (PackedScene)ResourceLoader.Load("res://0 Scenes/home.tscn");

        for (int i = 0; i < globals.homeSum; i++)
        {
            // Create random coordinates for where to spawn current robot
            Random rnd = new Random();
            int x = rnd.Next(0, simuWidth);
            int y = rnd.Next(0, simuHeight);

            home newInstance = (home)home.Instance();
            globals.homeList.Add(newInstance);
            AddChild(newInstance);
            newInstance.Position = new Vector2(x, y);
            homePosList.Add(newInstance.Position);
        }
    }


    // -- Simulation Maintainence Funcs
    // Controls all robot activity and movement.
    public void controlRobots()
    {
        // Find closest food, look at it and move towards it
        if ((globals.foodList.Count > 0) | (globals.currentMode != "winter"))
        {
            for (int i = 0; i < globals.robotList.Count; i++)
            {
                var currentRobot = globals.robotList[i];
                if (currentRobot != null)
                {
                    float closestDistance = 999999;
                    Vector2 closestFood = new Vector2(0, 0);
                    for (int y = 0; y < globals.foodList.Count; y++)
                    {
                        var currentFood = globals.foodList[y];
                        if (currentFood == null)
                        {
                            globals.foodList.Remove(currentFood);
                        }
                        else
                        {
                            float currentDistance = currentRobot.Position.DistanceTo(currentFood.Position);
                            if (currentDistance < closestDistance)
                            {
                                closestDistance = currentDistance;
                                closestFood = currentFood.Position;
                            }
                        }

                    }
                    Vector2 direction = currentRobot.Position.DirectionTo(closestFood);
                    currentRobot.LookAt(closestFood);
                    Vector2 velocity = direction * currentRobot.getSpeed();
                    currentRobot.MoveAndSlide(velocity);
                }
            }
        }
        else
        {
            activateHomes(true);
            for (int i = 0; i < globals.robotList.Count; i++)
            {
                var currentRobot = globals.robotList[i];
                if (!currentRobot.getAtHome())
                {
                    Vector2 housePos = homePosList[currentRobot.getHome()];
                    Vector2 direction = currentRobot.Position.DirectionTo(housePos);
                    currentRobot.LookAt(housePos);
                    Vector2 velocity = direction * currentRobot.getSpeed();
                    currentRobot.MoveAndSlide(velocity);
                }
            }
            if (globals.homeRobots.Count == globals.robotList.Count)
            {
                roundOver = true;
            }
        }
    }

    // Creates the children from the first two robot residents of every existing home.
    private void createChildren()
    {
        for (int i = 0; i < globals.homeSum; i++)
        {
            var currentHomeResidents = globals.homeResidents[i];
            if (currentHomeResidents.Count >= 2)
            {

                robot newChild = (robot)robot.Instance();

                float newSpeed;
                newSpeed = ((currentHomeResidents[0].getSpeed() +
                currentHomeResidents[1].getSpeed()) / 2) +
                (float)(rnd.Next(-15, 30));

                byte[] newColour = new byte[3];
                for (int col = 0; col < 3; col++)
                {
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

    // Updates the user interface.
    public void updateUI()
    {

        //Round Stats
        roundLabel.Text = globals.currentRound.ToString();
        seasonLabel.Text = globals.currentMode.Capitalize();
        robotsLabel.Text = globals.robotList.Count.ToString();
        countdownLabel.Text = countdownValue.ToString();

        //Robot Stats
        float fastestSpeed = 0;
        int mostHealth = 0;
        int leastHealth = 9999;
        for (int id = 0; id < globals.robotList.Count; id++)
        {
            robot currentRobot = globals.robotList[id];
            if (currentRobot != null)
            {
                if (currentRobot.getSpeed() > fastestSpeed)
                {
                    fastestSpeed = globals.robotList[id].getSpeed();
                }
                if (currentRobot.getHunger() > mostHealth)
                {
                    mostHealth = currentRobot.getHunger();
                }
                else if (currentRobot.getHunger() < leastHealth)
                {
                    leastHealth = currentRobot.getHunger();
                }
            }
        }
        fastestLabel.Text = fastestSpeed.ToString();
        mostHLabel.Text = mostHealth.ToString();
        leastHLabel.Text = leastHealth.ToString();

        //Countdown
        if (countdownValue > 15)
        {
            countdownLabel.Modulate = colourWhite;
        }
        else if (countdownValue > 5)
        {
            countdownLabel.Modulate = colourYellow;
        }
        else
        {
            countdownLabel.Modulate = colourRed;
        }
    }

    // Drains the hunger of every robot at the end of every season.
    public void drainHunger()
    {
        for (int i = 0; i < globals.robotList.Count; i++)
        {
            var currentRobot = globals.robotList[i];
            int drain = 1;
            if (globals.currentMode == "winter")
            {
                drain = 2;
            }
            currentRobot.setHunger(currentRobot.getHunger() - drain);
        }
    }

    // Kills starving robots at the end of every season.
    public void deathCheck()
    {
        var deadRobotList = new Godot.Collections.Array<robot>();

        for (int i = 0; i < globals.robotList.Count; i++)
        {
            var currentRobot = globals.robotList[i];
            if (currentRobot.getHunger() <= 0)
            {
                deadRobotList.Add(currentRobot);
            }
        }

        for (int x = 0; x < deadRobotList.Count; x++)
        {
            deadRobotList[x].killSelf();
        }
    }

    // Deactivates homes when not needed, reactives them when needed.
    public void activateHomes(bool activate)
    {
        for (int id = 0; id < globals.homeList.Count; id++)
        {
            if (globals.homeList[id] != null)
            {
                globals.homeList[id].setMonitoring(activate);
            }
        }
    }

    // -- Signal Funcs
    // Reduces the timer by 1 every second.
    public void _on_countdownTimer_timeout()
    {
        countdownValue -= 1;
    }

    // Changes relevant styling and position of UI elements when the close button is pressed.
    public void _on_closeRoundButton_pressed()
    {
        roundStatsOpen = !roundStatsOpen;
        if (roundStatsOpen)
        {
            roundStatsPanel.Visible = true;
            roundStatsBtn.SetPosition(roundStatsOpenPos);
            roundStatsBtn.AddStyleboxOverride("normal", closeStyle);
            roundStatsBtn.AddStyleboxOverride("hover", closeStyle);
            roundStatsBtn.AddStyleboxOverride("pressed", closeInvertStyle);
        }
        else
        {
            roundStatsPanel.Visible = false;
            roundStatsBtn.AddStyleboxOverride("normal", openStyle);
            roundStatsBtn.AddStyleboxOverride("hover", openStyle);
            roundStatsBtn.AddStyleboxOverride("pressed", openInvertStyle);
            roundStatsBtn.SetPosition(roundStatsClosedPos);
        }
    }

    // Changes relevant styling and position of UI elements when the close button is pressed.
    public void _on_closeRobotButton_pressed()
    {
        robotStatsOpen = !robotStatsOpen;
        if (robotStatsOpen)
        {
            robotStatsPanel.Visible = true;
            robotStatsBtn.SetPosition(robotStatsOpenPos);
            robotStatsBtn.AddStyleboxOverride("normal", closeStyle);
            robotStatsBtn.AddStyleboxOverride("hover", closeStyle);
            robotStatsBtn.AddStyleboxOverride("pressed", closeInvertStyle);
        }
        else
        {
            robotStatsPanel.Visible = false;
            robotStatsBtn.SetPosition(robotStatsClosedPos);
            robotStatsBtn.AddStyleboxOverride("normal", openStyle);
            robotStatsBtn.AddStyleboxOverride("hover", openStyle);
            robotStatsBtn.AddStyleboxOverride("pressed", openInvertStyle);
        }
    }

}
