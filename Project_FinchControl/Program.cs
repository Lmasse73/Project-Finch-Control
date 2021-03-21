using System;
using System.Collections.Generic;
using System.IO;
using FinchAPI;

namespace Project_FinchControl
{

    public enum Command
    {
        NONE,
        MOVEFORWARD,
        MOVEBACKWARD,
        STOPMOTORS,
        WAIT,
        TURNRIGHT,
        TURNLEFT,
        LEDON,
        LEDOFF,
        GETTEMPERATURE,
        DONE
    }

    /************************************************
     * Title: Finch Control
     * Application Type: Console
     * Description:First use of Finch Robot
     * Author:Laurent Mase
     * Date:2/12/2021
     * Last Modified: 2/28/2021
     * ***********************************************/

    class Program

    {
        /// <summary>
        /// first method run when the app starts up
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            SetTheme();

            DisplayWelcomeScreen();
            DisplayMenuScreen();
            DisplayClosingScreen();
        }

        /// <summary>
        /// setup the console theme
        /// </summary>
        static void SetTheme()
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.BackgroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// *****************************************************************
        /// *                     Main Menu                                 *
        /// *****************************************************************
        /// </summary>
        static void DisplayMenuScreen()
        {
            Console.CursorVisible = true;

            bool quitApplication = false;
            string menuChoice;

            Finch finchRobot = new Finch();


            do
            {
                DisplayScreenHeader("Main Menu");

                //
                // get user menu choice
                //
                Console.WriteLine("\ta) Connect Finch Robot");
                Console.WriteLine("\tb) Talent Show");
                Console.WriteLine("\tc) Data Recorder");
                Console.WriteLine("\td) Alarm System");
                Console.WriteLine("\te) User Programming");
                Console.WriteLine("\tf) Disconnect Finch Robot");
                Console.WriteLine("\tq) Quit");
                Console.Write("\t\tEnter Choice:");
                menuChoice = Console.ReadLine().ToLower();
          

                if (menuChoice == "a" || menuChoice == "b" || menuChoice == "c" || menuChoice == "d" || menuChoice == "e" || menuChoice == "f" || menuChoice == "q")    //created if statement to validate string
                {

                    switch (menuChoice)
                    {
                        case "a":
                            bool connectSuccess = DisplayConnectFinchRobot(finchRobot);
                            break;

                        case "b":
                            TalentShowDisplayMenuScreen(finchRobot);
                            break;

                        case "c":
                            DataRecorderDisplayMenuScreen(finchRobot);

                            break;

                        case "d":
                            LightAlarmDisplayMenuScreen(finchRobot);

                            break;

                        case "e":
                            UserProgrammingDisplayMenuScreen(finchRobot);
                            break;

                        case "f":
                            DisplayDisconnectFinchRobot(finchRobot);
                            break;

                        case "q":
                            DisplayDisconnectFinchRobot(finchRobot);
                            quitApplication = true;
                            break;

                        default:

                            DisplayContinuePrompt();
                            break;
                    }
                }                                               //removed the default message and return to menu in order to validate string
                else
                {
                    Console.WriteLine("\tYou entered {0}, which is not valid.  Please select one of the choices from the menu.", menuChoice);
                    DisplayContinuePrompt();
                }

            } while (!quitApplication);
        }
        #region USER PROGRAMMING
        /// <summary>
        /// User Prammer Display Menu, declaring the tuple, 
        /// using the List and Enum, do loop and switch for the menu options
        /// </summary>
        /// <param name="finchRobot"></param>
        private static void UserProgrammingDisplayMenuScreen(Finch finchRobot)
        {
            string menuChoice;
            bool quitMenu = false;

            (int motorspeed, int ledBrightness, double waitSeconds) commandParameters;    //declaring the tuple with the cammandParameters name
            commandParameters.motorspeed = 0;
            commandParameters.ledBrightness = 0;
            commandParameters.waitSeconds = 0;

            List<Command> commands = new List<Command>();    //List getting the Enum values

            do
            {
                DisplayScreenHeader("User Programming Menu");
                Console.WriteLine("\ta) Set Command Parameters");
                Console.WriteLine("\tb) Add Commands");
                Console.WriteLine("\tc) View Commands");
                Console.WriteLine("\td) Execute Commands");
                Console.WriteLine("\tq) Return to Main Menu");
                Console.Write("\t\tEnter Your Choice: ");
                menuChoice = Console.ReadLine().ToLower();

                switch (menuChoice)
                {
                    case "a":
                        commandParameters = UserProgrammingDisplayGetCommandParameter();
                        EchoCommandParameters(commandParameters);
                        break; ;

                    case "b":
                        commands = UserProgrammingDisplayGetFinchCommands();
                        break;

                    case "c":
                        UserProgrammingDisplayFinchCommands(commands);
                        break;

                    case "d":
                        UserProgrammerDisplayExecuteFinchCommands(commandParameters, commands, finchRobot);
                        break;

                    case "q":
                        quitMenu = true;
                        break;

                    default:
                        Console.WriteLine();
                        Console.Write("\tPlease enter a letter from the menu choice.");
                        DisplayContinuePrompt();
                        break;
                }

            } while (!quitMenu);
        }

        /// <summary>
        /// Getting the command parameters from the user with the tuple
        /// getting user input and storingit in comandP
        /// </summary>
        /// <returns></returns>
        private static (int motorspeed, int ledBrightness, double waitSeconds) UserProgrammingDisplayGetCommandParameter()
        {
            (int motorspeed, int ledBrightness, double waitSeconds) commandParameters;
            DisplayScreenHeader("Command Parameters");
            Console.Write("\tEnter Motor Speed [1-250]: ");
            commandParameters.motorspeed = getIntNumber(1, 255);
            Console.Write("\tEnter LED Brightness [1-255]: ");
            commandParameters.ledBrightness = getIntNumber(1, 255);
            Console.Write("\tEnter Wait Elapse Time in Seconds: ");
            commandParameters.waitSeconds = getDoubleNumber(0.01, 100);


            return commandParameters;
        }

        /// <summary>
        /// Displaying the command list from the Enum, and asking the user for their choices with another method called
        /// UserProgrammingGetUserCommands to validate.
        /// </summary>
        /// <returns>the user command choices</returns>
        private static List<Command> UserProgrammingDisplayGetFinchCommands()
        {
            List<Command> commands = new List<Command>();

            DisplayScreenHeader("\n\n\t Finch Robot Command List");
            //use a count to separate the commands evenly on each line - 4 per line
            int wordCount = 1;
            Console.Write("\t");

            foreach (string command in Enum.GetNames(typeof(Command)))
            {
                if (wordCount == 4)
                {
                    Console.WriteLine();
                    Console.Write("\t");
                    wordCount = 1;
                }
                if (command != "DONE" && command != "NONE")
                {
                    Console.Write("--{0,-15}", command);
                    wordCount++;
                }
            }
            Console.WriteLine();
            Console.WriteLine("\tChoose the commands you would like to use from above, one at a time. Enter \"done\" when you are finished.");
            commands = UserProgrammingGetUserCommands();
            DisplayContinuePrompt();
            return commands;
        }

        /// <summary>
        /// Displaying the commands entered by the user
        /// Used foreach to cycle through the commands entered by user.  
        /// Getting the List of commands, and usercommands
        /// </summary>
        /// <param name="commands"></param>
        private static void UserProgrammingDisplayFinchCommands(List<Command> commands)
        {
            DisplayScreenHeader("User Command Choices");
            foreach (Command userCommands in commands)
            {
                Console.WriteLine("\t\t{0}", userCommands);
            }
            DisplayContinuePrompt();
            
        }


        /// <summary>
        /// Getting the user command choices, validating them with TryParse.
        /// </summary>
        /// <returns></returns>
        private static List<Command> UserProgrammingGetUserCommands()
        {
            List<Command> commands = new List<Command>();
            string userResponse;
            bool validCommand = false;
            Command userCommand;

            do
            {
                Console.Write("\tEnter command: ");
                userResponse = Console.ReadLine().ToUpper();
                validCommand = Enum.TryParse(userResponse, out userCommand);

                if (validCommand)
                {
                    if (userResponse !="DONE")
                    {
                        commands.Add(userCommand);
                    }
                }
                else
                {
                    Console.WriteLine("\tInvalid entry, please try again.");
                }

            } while (userResponse != "DONE");

            return commands;
        }


        /// <summary>
        /// Executing the Finch Commands
        ///  Uisng foreach to cycyle through the commands for the user
        ///  and executes them. 
        /// </summary>
        /// <param name="commandParameters"></param>
        /// <param name="commands"></param>
        /// <param name="finchRobot"></param>
        private static void UserProgrammerDisplayExecuteFinchCommands((int motorspeed, int ledBrightness, double waitSeconds) commandParameters,
                                                                      List<Command> commands, Finch finchRobot)
        {
            DisplayScreenHeader("The Finch robot Will Now Execute Your Commands.");
            DisplayContinuePrompt();


            foreach (Command userCommand in commands)
            {
                string command = userCommand.ToString();

                switch (command)
                {
                    case "MOVEFORWARD":
                        Console.WriteLine("\t " + command);
                        finchRobot.setMotors(commandParameters.motorspeed, commandParameters.motorspeed);
                        break;

                    case "MOVEBACKWARD":
                        Console.WriteLine("\t " + command);
                        finchRobot.setMotors(commandParameters.motorspeed * -1, commandParameters.motorspeed * -1);
                        break;

                    case "STOPMOTORS":
                        Console.WriteLine("\t " + command);
                        finchRobot.setMotors(0, 0);
                        break;

                    case "WAIT":
                        Console.WriteLine("\t " + command);
                        finchRobot.wait(Convert.ToInt32(commandParameters.waitSeconds) * 1000);
                        break;

                    case "TURNRIGHT":
                        Console.WriteLine("\t " + command);
                        finchRobot.setMotors(commandParameters.motorspeed, commandParameters.motorspeed * -1);
                        break;

                    case "TURNLEFT":
                        Console.WriteLine("\t " + command);
                        finchRobot.setMotors(commandParameters.motorspeed * -1, commandParameters.motorspeed);
                        break;

                    case "LEDON":
                        Console.WriteLine("\t " + command);
                        finchRobot.setLED(commandParameters.ledBrightness, commandParameters.ledBrightness, commandParameters.ledBrightness);
                        break;

                    case "LEDOFF":
                        Console.WriteLine("\t " + command);
                        finchRobot.setLED(0, 0, 0);
                        break;

                    case "GETTEMPERATURE":
                        Console.WriteLine("\t " + command);
                        double temp = finchRobot.getTemperature();
                        Console.WriteLine($"\t The temperature is {temp}.");
                        break;

                    default:
                        break;
                }

            }
            DisplayContinuePrompt();
        }

        #endregion USER PROGRAMMING

        #region DATA RECORDER MENU

        /// <summary>
        /// **************************************************
        ///      DATE RECORDER MENU SCREEN
        /// **************************************************
        /// </summary>
        /// <param name="finchRobot"></param>
        static void DataRecorderDisplayMenuScreen(Finch finchRobot)
        {
            int numberOfDataPoints = 0;
            double dataPointFrequency = 0;
            double[] temperatures = null;

            Console.CursorVisible = true;
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkBlue;

            bool quitMenu = false;
            string menuChoice;

            do
            { 
                DisplayScreenHeader("Data Recorder Menu");
                Console.WriteLine("\ta) Number of Temperature Data Points");
                Console.WriteLine("\tb) Frequency of Temperature Data Points");
                Console.WriteLine("\tc) Get Data");
                Console.WriteLine("\td) Show Data");
                Console.WriteLine("\tq) Return to Main Menu");
                Console.Write("\t\tEnter Your Choice: ");
                menuChoice = Console.ReadLine().ToLower();

                switch (menuChoice)
                {
                    case "a":
                        numberOfDataPoints = TempDataPoints();
                        break;

                    case "b":
                        dataPointFrequency= DataPointsFrequency();
                        break;

                    case "c":
                        temperatures= GetData(numberOfDataPoints, dataPointFrequency, finchRobot);
                        break;

                    case "d":
                        ShowData(temperatures);
                        break;

                    case "q":
                        quitMenu = true;
                        break;

                    default:
                        Console.WriteLine();
                        Console.Write("\tPlease enter a letter from the menu choice.");
                        DisplayContinuePrompt();
                        break;
                }

            } while (!quitMenu);
        }
        #endregion   //end of Data Menu region

        #region DATA RECORDER METHODS
        /********************************************************************
         *                  DATA RECORDER METHODS
         *******************************************************************/
        /// <summary>
        /// Get Number of Datat Points from user
        /// </summary>
        /// <param name="finchRobot"></param>
        /// <returns>number of data points</returns>
        static int TempDataPoints()
        {
            int numberOfDataPoints = 0;
            string userResponse;
            bool quit = false;
            Console.CursorVisible = false;
            DisplayScreenHeader("Number of Data Points");
            Console.Write("\tPlease enter how many temperature readings you would like to record: ");

            while (!quit)
            {
                userResponse = Console.ReadLine();
                bool success = int.TryParse(userResponse, out numberOfDataPoints);
                if (success && numberOfDataPoints > 0)
                {                 
                    quit = true;
                }
                else
                {
                    Console.Write("You have entered an invalid number, please enter another number of temperature readings: ");
                }
            }
            DisplayContinuePrompt();
            return numberOfDataPoints;
        }
        /// <summary>
        /// Get Frequency of Data points from the user
        /// </summary>
        /// <param name="finchRobot"></param>
        /// <returns>Returns Frequency of data Points</returns>
        static double DataPointsFrequency()
        {
            double dataPointFrequency=0;
            bool quit = false;
            Console.CursorVisible = false;
            DisplayScreenHeader("Data Point Frequency");
            Console.Write("\tHow much time would you like between the data points?  Please enter a number in seconds: ");
            while (!quit)
            {
              bool success =  double.TryParse(Console.ReadLine(), out dataPointFrequency);

                if (success && dataPointFrequency > 0)
                {
                    quit = true;
                }
                else
                {
                    Console.Write("You have entered an invalid number, please enter a number for the frequency of data points to be collected: ");
                }
            }
          
            DisplayContinuePrompt();
            return dataPointFrequency;
        }
        /// <summary>
        /// Finch Collecting the Data
        /// </summary>
        /// <param name="numberOfDataPoints"></param>
        /// <param name="dataPointFrequency"></param>
        /// <param name="finchRobot"></param>
        /// <returns>The collected Temperatures in an array</returns>
       static double [] GetData(int numberOfDataPoints, double dataPointFrequency, Finch finchRobot)
        {
            double[] temperatures = new double[numberOfDataPoints];
            DisplayScreenHeader("Get Data");
            Console.WriteLine($"\tNumber of Data Points: {numberOfDataPoints}");
            Console.WriteLine($"\tFrequency of Collected Data Points: {dataPointFrequency}");
            Console.WriteLine();
            Console.WriteLine("\tThe Finch Robot is ready to record the temperature data.");
            DisplayContinuePrompt();

            for (int index = 0; index < numberOfDataPoints; index++)
            {
                temperatures[index] = finchRobot.getTemperature();
                Console.WriteLine($"\tReading {index +1}: {temperatures[index].ToString("n2")}");
                int waitInSeconds = (int)(dataPointFrequency * 1000);
                finchRobot.wait(waitInSeconds);
            }

            Console.WriteLine();
            Console.WriteLine("Temperature Table");
            Console.WriteLine();
            DataRecorderDisplayTable(temperatures);
            DisplayContinuePrompt();
            return temperatures;
        }
        /// <summary>
        /// Calling Methods to display the table, and Sum, Sort, Averages
        /// </summary>
        /// <param name="temperatures"></param>
        static void ShowData(double[] temperatures)
        {
            DisplayScreenHeader("Show Data");
            DataRecorderDisplayTable(temperatures);
            DisplayContinuePrompt();
            DisplayDataStats(temperatures);
        }
        /// <summary>
        /// Method to display Sum, Average and Sort
        /// </summary>
        /// <param name="temperatures"></param>
        private static void DisplayDataStats(double[] temperatures)
        {
            int x;
            Array.Sort(temperatures);
            Console.WriteLine("\tThe temperatures sorted in ascending order: ");
            double sum = 0;
            double average;
            for (x = 0; x < temperatures.Length; x++)
            {
                sum = sum + temperatures[x];
                Console.WriteLine($"\t{temperatures[x].ToString("n2")}");
            }
            Console.WriteLine($"\tThe sum of the temperatures is {sum.ToString("n2")}");
            average = sum / temperatures.Length;
            Console.WriteLine($"\tThe average temperature is {average.ToString("n2")}");
            DisplayContinuePrompt();
        }
        /// <summary>
        /// Method to Display Table
        /// </summary>
        /// <param name="temperatures"></param>
        static void DataRecorderDisplayTable(double[] temperatures)
        {
            //
            //display tableheadder
            //
            Console.WriteLine(
                "Recording #".PadLeft(15) +
                "Temp (Celcius)".PadLeft(15) +
                "Temp (Fahreneit)".PadLeft(18));

            Console.WriteLine(
                "_____________".PadLeft(15) +
                "_____________".PadLeft(15) +
                "________________".PadLeft(18));
        
            for (int index = 0; index < temperatures.Length; index++)
            {
                double fahrenheit = (temperatures[index] * 9) / 5 + 32;
                Console.WriteLine(
                (index + 1).ToString().PadLeft(15) +
                temperatures[index].ToString("n2").PadLeft(15) + fahrenheit.ToString("n2").PadLeft(18));
            }
        }

        #endregion  //End Data Methods Region

        #region ALARM SYSTEM

        /// <summary>
        /// **************************************************
        ///      LIGHT ALARM MENU SCREEN
        /// **************************************************
        /// </summary>
        /// <param name="finchRobot"></param>
        static void LightAlarmDisplayMenuScreen(Finch finchRobot)
        {
            
            Console.CursorVisible = true;
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkBlue;

            bool quitMenu = false;
            string menuChoice;
            string sensorsToMonitor = "";
            string rangeType = "";
            int minMaxThresholdValue = 0;
            int timeToMonitor = 0;


            do
            {
                DisplayScreenHeader("Light Alarm System");
                Console.WriteLine("\ta) Set Sensors to Monitor");
                Console.WriteLine("\tb) Set Range Type");
                Console.WriteLine("\tc) Set Minimum/Maximum Threshold Values");
                Console.WriteLine("\td) Set Time to Monitor");
                Console.WriteLine("\te) Set Alarm");
                Console.WriteLine("\tq) Return to Main Menu");
                Console.Write("\t\tEnter Your Choice: ");
                menuChoice = Console.ReadLine().ToLower();

                switch (menuChoice)
                {
                    case "a":
                        sensorsToMonitor = LightAlarmDisplaySetSensorsToMonitor();
                        break;

                    case "b":
                        rangeType = LightAlarmDisplaySetRangeType();
                        break;

                    case "c":
                        minMaxThresholdValue = LightAlarmSetMinMaxThresholdValue(rangeType, finchRobot);
                        break;

                    case "d":
                        timeToMonitor = LightAlarmSetTimeToMonitor();
                        break;

                    case "e":
                        LightAlarmSetAlarm(finchRobot, sensorsToMonitor, rangeType, minMaxThresholdValue, timeToMonitor);
                        break;
                    case "q":
                        quitMenu = true;
                        break;

                    default:
                        Console.WriteLine();
                        Console.Write("\tPlease enter a letter from the menu choice.");
                        DisplayContinuePrompt();
                        break;
                }

            } while (!quitMenu);
        }




        /// <summary>
        /// *****************************
        /// LIGHT ALARM METHODS
        /// *****************************
        /// </summary>
        /// <returns></returns>
        /// 


        /// SETTING THE ALARM METHOD///
        private static void LightAlarmSetAlarm(
                   Finch finchRobot,
                   string sensorsToMonitor,
                   string rangeType,
                   int minMaxThresholdValue,
                   int timeToMonitor)
        {

            int secondsElapsed = 0;
            bool threshholdExceeded = false;
            int currentLightSensorValue=0;
            DisplayScreenHeader("Set Alarm");

            Console.WriteLine($"\tSensors to monitor: {sensorsToMonitor}");
            Console.WriteLine($"\tRange Type: {rangeType}");
            Console.WriteLine("\tMin/Max threshold value: " + minMaxThresholdValue);
            Console.WriteLine($"\tTime to monitor: {timeToMonitor}");
            Console.WriteLine();

            Console.WriteLine("\tPress any key to begin monitoring.");
            Console.ReadKey();
            Console.WriteLine();

            while ((secondsElapsed < timeToMonitor) && !threshholdExceeded)
            {
                switch (sensorsToMonitor)
                {
                    case "left":
                        currentLightSensorValue = finchRobot.getLeftLightSensor();
                        break;

                    case "right":
                        currentLightSensorValue = finchRobot.getRightLightSensor();
                        break;

                    case "both":
                        currentLightSensorValue = (finchRobot.getLeftLightSensor() + finchRobot.getRightLightSensor()) / 2;
                        break;
                }
                switch (rangeType)
                {
                    case "minimum":
                        if (currentLightSensorValue < minMaxThresholdValue)
                        {
                            threshholdExceeded = true;
                        }
                        break;

                    case "maximum":
                        if (currentLightSensorValue > minMaxThresholdValue)
                        {
                            threshholdExceeded = true;
                        }
                        break;
                }
                finchRobot.wait(1000);
                secondsElapsed++;
                Console.WriteLine("\tSeconds Elapsed: {0}",secondsElapsed);
                Console.WriteLine("\tCurrent Light Sensor Value: {0}", currentLightSensorValue);
                Console.WriteLine();
            }

            if (threshholdExceeded)
            {
                Console.WriteLine($"\t***** The {rangeType} threshold value of {minMaxThresholdValue} was exceeded by the current light sensor value of {currentLightSensorValue}. *****");
                finchRobot.setLED(255, 0, 255);
                finchRobot.noteOn(261);
                finchRobot.wait(1000);
                finchRobot.setLED(0, 0, 0);
                finchRobot.noteOff();
            }
            else 
            {
                Console.WriteLine($"\t***** The {rangeType} threshold value of {minMaxThresholdValue} was not exceeded.*****");
            }
            DisplayMenuPrompt("Light Alarm");
        }



        ///SENSORS TO MONITOR///
        static string LightAlarmDisplaySetSensorsToMonitor()
        {
            string sensorsToMonitor;
            string[] validSensorValues = new string[] { "left", "right", "both" };
            string message = ("\tSelect the sensors to monitor with [left, right, both]");
            DisplayScreenHeader("Sensors To Monitor");

            sensorsToMonitor = ValidateUserValuesWithArray(validSensorValues, message);
            Console.WriteLine("\tYou entered {0}.", sensorsToMonitor);
            DisplayMenuPrompt("Light Alarm");

            return sensorsToMonitor;
        }

        /// <summary>
        /// RANGE TYPE////
        /// </summary>
        /// <returns>returns the range type</returns>
        static string LightAlarmDisplaySetRangeType()
        {
            string rangeType;
            string[] rangeTypeValues = new string[] { "minimum", "maximum" };
            string message = ("\tSelect the Range Type [minumum, maximum]");

            DisplayScreenHeader("Range Type");
            rangeType = ValidateUserValuesWithArray(rangeTypeValues, message);
            Console.WriteLine("\tYou entered {0}.", rangeType);
            DisplayMenuPrompt("Light Alarm");

            return rangeType;
        }

        /// <summary>
        /// ***********************************
        /// SETTING MIN/MAX THRESHOLD VALUE
        /// ***********************************
        /// Used getValidLightSensor method to prompt user and to validate.
        /// 
        /// </summary>
        /// <param name="rangeType"></param>
        /// <param name="finchRobot"></param>
        /// <returns></returns>
           static int LightAlarmSetMinMaxThresholdValue(string rangeType, Finch finchRobot)
        {

            DisplayScreenHeader("Minimum/Maximum Threshold Value");
            Console.WriteLine($"\tLeft light sensor ambient value: {finchRobot.getLeftLightSensor()}");
            Console.WriteLine($"\tRight light sensor ambient value: {finchRobot.getRightLightSensor()}");
            Console.WriteLine();
             int minMaxThresholdValue = GetValidLightSensor(rangeType);
            DisplayMenuPrompt("Light Alarm");
            return minMaxThresholdValue;
        }


        /// <summary>
        /// SET TIME TO MONITOR  
        /// getting the user to enter a time value to 
        /// monitor the light.  With Validation
        /// </summary>
        /// <returns></returns>
        static int LightAlarmSetTimeToMonitor()
        {
            int timeToMonitor;
            bool validNumber = false;

            DisplayScreenHeader("Time to Monitor");
            
            Console.Write("\tPlease enter the time to monitor: ");

            do
            {
                validNumber = int.TryParse(Console.ReadLine(), out timeToMonitor);

                if (validNumber && timeToMonitor > 0)
                {
                    break;
                }
                else
                {
                    validNumber = false;
                    Console.Write("\tInvalid Entry, please enter again");
                }
            } while (!validNumber);
            Console.WriteLine("\tYou have chosen to monitor for {0} seconds.", timeToMonitor);
            DisplayMenuPrompt("Light Alarm");
            return timeToMonitor;
        }

        /// <summary>
        /// GEt
        /// </summary>
        /// <param name="rangeType"></param>
        /// <returns></returns>
        static int GetValidLightSensor(string rangeType)
        {
            bool success = false;
            int minMaxThresholdValue;

            Console.Write($"\tPlease enter the {rangeType} light sensor value from 0-255: ");
            do
            {
                success = int.TryParse(Console.ReadLine(), out minMaxThresholdValue);
                if (success)
                {
                    if (minMaxThresholdValue >= 0 && minMaxThresholdValue <= 255)
                    {
                        Console.WriteLine("\tYou have entered {0}.", minMaxThresholdValue);
                    }
                    else
                    {
                        success = false;
                    }
                     
                }
                else
                { 
                    Console.Write("\tInvalid value, please try again: ");
                }
            } while (!success);
            return minMaxThresholdValue;
        }

        #endregion
        #region COMMON METHODS   
        /// <summary>
        /// VALIDATOR WITH ARRAY     
        /// 
        /// Reads the userinput, loops through the array to find a match
        /// If it is found it will set the bool to true and break out of the loop
        /// If it is not it will resend the message to the user.
        /// </summary>
        /// <param name="validValueSet"></param>
        /// <param name="message"></param>
        /// <returns>valid uservalue</returns>
        static string ValidateUserValuesWithArray(string [] validValueSet, string message)
        {
            bool matchFound = false;
            string userValue;
            Console.Write(message + " : ");
            do
            {
                userValue = Console.ReadLine().ToLower();

                for (int i = 0; i < validValueSet.Length; i++)
                {
                    if (userValue == validValueSet[i])
                        matchFound = true;
                }
                if (!matchFound)
                {
                    Console.Write("\tInvalid value, please try again.  " + message + " : ");
                }
                else
                    break;
            } while (!matchFound);
            return userValue;
        }
        /// <summary>
        /// Used to validate an int
        /// </summary>
        /// <param name="x"></param>
        /// <returns>bool tru or false</returns>
        static bool CheckValidNumber(string x)   //validating number
        {
            bool success = Int32.TryParse(x, out int number);
            return success;
        }

        /// <summary>
        /// READ int number from user, validate the number
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private static int getIntNumber(int min, int max)
        {
            bool success = false;
            bool goodRange = false;
            int number = 0;

            do
            {
                string userResponse = Console.ReadLine();
                success = Int32.TryParse(userResponse, out number);

                if (success)
                {
                    goodRange = ValidateIntRange(number, min, max);
                }
                if ((!success) || (!goodRange))
                {
                    Console.Write("\tInvalid entry, please enter again: ");
                }

            } while (!success || !goodRange);
            return number;
        }

        /// <summary>
        /// VALIDATING an INT RANGE
        /// </summary>
        /// <param name="number"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private static bool ValidateIntRange(int number, int min, int max)
        {
            bool inRange = false;

            if (number >= min && number <= max)
            {
                inRange = true;
            }

            return inRange;
        }


        /// <summary>
        /// GETTING The user input and then validating with other methods
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private static double getDoubleNumber(double min, int max)
        {
            bool success = false;
            bool goodRange = false;
            double number = 0;

            do
            {
                string userResponse = Console.ReadLine();
                success = Double.TryParse(userResponse, out number);

                if (success)
                {
                    goodRange = ValidateDoubleRange(number, min, max);
                }
                if (!success || !goodRange)
                {
                    Console.Write("\tInvalid entry, please enter again: ");
                }

            } while (!success || !goodRange);
            return number;
        }


        /// <summary>
        /// Validate DOUBLE Range
        /// </summary>
        /// <param name="number"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private static bool ValidateDoubleRange(double number, double min, int max)
        {
            bool inRange = false;

            if (number >= min && number <= max)
            {
                inRange = true;
            }
            return inRange;
        }

        /// <summary>
        /// Echoing the command parameters
        /// </summary>
        /// <param name="commandParameters"></param>
        private static void EchoCommandParameters((int motorspeed, int ledBrightness, double waitSeconds) commandParameters)
        {
            Console.WriteLine();
            Console.WriteLine("\tYou entered:");
            Console.WriteLine();
            Console.WriteLine("\tMotorspeed: {0}", commandParameters.motorspeed);
            Console.WriteLine("\tLED Brightness: {0}", commandParameters.ledBrightness);
            Console.WriteLine("\tWait Seconds: {0}", commandParameters.waitSeconds);
            DisplayContinuePrompt();
        }


        #endregion  Common Methods
        #region TALENT SHOW

        /// <summary>
        /// *****************************************************************
        /// *                     Talent Show Menu                          *
        /// *****************************************************************
        /// </summary>
        static void TalentShowDisplayMenuScreen(Finch finchRobot)
        {
            Console.CursorVisible = true;
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkBlue;

            bool quitTalentShowMenu = false;
            string menuChoice;

            do
            {
                DisplayScreenHeader("Talent Show Menu");

                //
                // get user menu choice
                //
                Console.WriteLine("\ta) Light and Sound");
                Console.WriteLine("\tb) Dance");
                Console.WriteLine("\tc) Mixing It Up");
                Console.WriteLine("\td) Under Development");
                Console.WriteLine("\tq) Main Menu");
                Console.Write("\t\tEnter Choice:");
                menuChoice = Console.ReadLine().ToLower();

                //
                // process user menu choice
                //
                switch (menuChoice)
                {
                    case "a":
                        TalentShowDisplayLightAndSound(finchRobot);
                        break;

                    case "b":
                        TalentShowDance(finchRobot);

                        break;

                    case "c":
                        TalentShowMixItUp(finchRobot);
                        
                        break;

                    case "d":
                        Console.WriteLine("\tUnder Construction, see you soon!");
                        Console.ReadLine();
                        break;

                    case "q":
                        quitTalentShowMenu = true;
                        break;

                    default:
                        Console.WriteLine();
                        Console.WriteLine("\tPlease enter a letter from the menu choice.");
                        DisplayContinuePrompt();
                        break;
                }

            } while (!quitTalentShowMenu);
        }

        /// <summary>
        /// *****************************************************************
        /// *               Talent Show > Light and Sound                   *
        /// *****************************************************************
        /// </summary>
        /// <param name="finchRobot">finch robot object</param>
        static void TalentShowDisplayLightAndSound(Finch finchRobot)
        {
            Console.CursorVisible = false;
            DisplayScreenHeader("Light and Sound");
            Console.WriteLine("\t\tThe Finch robot will now show off its glowing talent, ");
            Console.WriteLine("\twhile playing \"Whisky Before Breakfast\", a traditional bluegrass song.");
            DisplayContinuePrompt();

            finchRobot.setLED(0, 255, 255);
            finchRobot.wait(1000);
            finchRobot.setLED(0, 0, 255);
            finchRobot.wait(1000);
            finchRobot.setLED(255, 255, 0);
            finchRobot.wait(1000);          
            Song(finchRobot);
            finchRobot.setLED(0, 0, 0);

            DisplayMenuPrompt("Talent Show Menu");
        }

        /******************************************************************
         *                  Talent Show Dance
         * ***************************************************************/
        static void TalentShowDance(Finch finchRobot)
        {
            Console.CursorVisible = false;
            DisplayScreenHeader("Dance");
            Console.WriteLine("\tThe Finch robot will now do a little dance.");
            DisplayContinuePrompt();
            Dancer(finchRobot);
        }

        /*******************************************************************
         *              Talent ShowMix it UP                                *
         ******************************************************************/
        static void TalentShowMixItUp(Finch finchRobot)
        {
            DisplayScreenHeader("Mix it up");
            Console.WriteLine("The Finch Robot will now mix it up and show a variety of its talents.");
            Console.WriteLine("How many times do you want Finch to cyle the mix it up combo?  ");
            string userRunTime = Console.ReadLine();
            bool valid = CheckValidNumber(userRunTime);                     //calling methof to validate number with feeback message, also single responsiblity
            int runTime=0;
            if (valid)
            {
                Console.WriteLine("Finch will run {0} times", userRunTime);
                runTime = Int32.Parse(userRunTime);
            }
            else
            {
                Console.WriteLine("You did not enter a number, please try again.");
                
            }
            //looping and generating random lights, need to sort this out some more, but tried and it does work on longer sequences.
            for(int x = 1; x <=runTime; x++)                    
            {
                int value1 = GenerateRandomValue(10, 150);
                int value2 = GenerateRandomValue(0, 10);
                int value3 = GenerateRandomValue(100, 255);
                finchRobot.setLED(value1, value2, value3);
                ShortSong(finchRobot);
                ShortDance(finchRobot);                              
            }           
            finchRobot.setLED(0, 0, 0);
            DisplayContinuePrompt();

        }
        //use GenerateRandomValue for the mix it up lights.//
        static int GenerateRandomValue(int x, int y)
        {
            Random randomValue = new Random();
            int generatedValue = randomValue.Next(x, y);
            return generatedValue;
        }
        //method used for song only
        static void Song(Finch finchRobot)
        {
            finchRobot.noteOn(220);
            finchRobot.wait(250);
            finchRobot.noteOn(246);
            finchRobot.wait(250);
            finchRobot.noteOn(293);
            finchRobot.wait(250);
            finchRobot.noteOn(329);
            finchRobot.wait(250);
            finchRobot.noteOn(293);
            finchRobot.wait(500);
            finchRobot.noteOn(293);
            finchRobot.wait(250);
            finchRobot.noteOn(293);
            finchRobot.wait(250);
            finchRobot.noteOn(329);
            finchRobot.wait(250);
            finchRobot.noteOn(370);
            finchRobot.wait(250);
            finchRobot.noteOn(440);
            finchRobot.wait(250);
            finchRobot.noteOn(494);
            finchRobot.wait(250);
            finchRobot.noteOn(440);
            finchRobot.wait(250);
            finchRobot.noteOn(370);
            finchRobot.wait(250);
            finchRobot.noteOn(329);
            finchRobot.wait(250);
            finchRobot.noteOn(370);
            finchRobot.wait(250);
            finchRobot.noteOn(392);
            finchRobot.wait(250);
            finchRobot.noteOn(494);
            finchRobot.wait(250);
            finchRobot.noteOn(392);
            finchRobot.wait(250);
            finchRobot.noteOn(370);
            finchRobot.wait(500);
            finchRobot.noteOn(440);
            finchRobot.wait(250);
            finchRobot.noteOn(370);
            finchRobot.wait(250);
            finchRobot.noteOn(329);
            finchRobot.wait(250);
            finchRobot.noteOn(293);
            finchRobot.wait(250);
            finchRobot.noteOn(329);
            finchRobot.wait(250);
            finchRobot.noteOn(370);
            finchRobot.wait(250);
            finchRobot.noteOn(329);
            finchRobot.wait(250);
            finchRobot.noteOn(293);
            finchRobot.wait(250);
            finchRobot.noteOn(246);
            finchRobot.wait(500);
            finchRobot.noteOn(220);
            finchRobot.wait(250);
            finchRobot.noteOn(246);
            finchRobot.wait(250);
            finchRobot.noteOn(293);
            finchRobot.wait(250);
            finchRobot.noteOn(329);
            finchRobot.wait(250);
            finchRobot.noteOn(293);
            finchRobot.wait(500);
            finchRobot.noteOn(293);
            finchRobot.wait(250);
            finchRobot.noteOn(293);
            finchRobot.wait(250);
            finchRobot.noteOn(329);
            finchRobot.wait(250);
            finchRobot.noteOn(370);
            finchRobot.wait(250);
            finchRobot.noteOn(440);
            finchRobot.wait(250);
            finchRobot.noteOn(494);
            finchRobot.wait(250);
            finchRobot.noteOn(440);
            finchRobot.wait(250);
            finchRobot.noteOn(370);
            finchRobot.wait(250);
            finchRobot.noteOn(329);
            finchRobot.wait(250);
            finchRobot.noteOn(370);
            finchRobot.wait(250);
            finchRobot.noteOn(392);
            finchRobot.wait(250);
            finchRobot.noteOn(494);
            finchRobot.wait(250);
            finchRobot.noteOn(392);
            finchRobot.wait(250);
            finchRobot.noteOn(370);
            finchRobot.wait(500);
            finchRobot.noteOn(440);
            finchRobot.wait(250);
            finchRobot.noteOn(370);
            finchRobot.wait(250);
            finchRobot.noteOn(329);
            finchRobot.wait(250);
            finchRobot.noteOn(293);
            finchRobot.wait(250);
            finchRobot.noteOn(277);
            finchRobot.wait(250);
            finchRobot.noteOn(329);
            finchRobot.wait(250);
            finchRobot.noteOn(293);
            finchRobot.wait(1000);
            finchRobot.noteOn(293);
            finchRobot.wait(250);
            finchRobot.noteOn(329);
            finchRobot.wait(250);
            finchRobot.noteOn(370);
            finchRobot.wait(250);
            finchRobot.noteOn(392);
            finchRobot.wait(250);
            finchRobot.noteOn(440);
            finchRobot.wait(1000);
            finchRobot.noteOn(147);
            finchRobot.wait(250);
            finchRobot.noteOn(494);
            finchRobot.wait(250);
            finchRobot.noteOn(440);
            finchRobot.wait(250);
            finchRobot.noteOn(392);
            finchRobot.wait(250);
            finchRobot.noteOn(370);
            finchRobot.wait(250);
            finchRobot.noteOn(293);
            finchRobot.wait(750);
            finchRobot.noteOn(147);
            finchRobot.wait(250);
            finchRobot.noteOn(165);
            finchRobot.wait(250);
            finchRobot.noteOn(165);
            finchRobot.wait(250);
            finchRobot.noteOn(165);
            finchRobot.wait(250);
            finchRobot.noteOn(165);
            finchRobot.wait(500);
            finchRobot.noteOn(165);
            finchRobot.wait(250);
            finchRobot.noteOn(185);
            finchRobot.wait(250);
            finchRobot.noteOn(196);
            finchRobot.wait(250);
            finchRobot.noteOn(185);
            finchRobot.wait(250);
            finchRobot.noteOn(165);
            finchRobot.wait(250);
            finchRobot.noteOn(147);
            finchRobot.wait(250);
            finchRobot.noteOn(139);
            finchRobot.wait(250);
            finchRobot.noteOn(440);
            finchRobot.wait(250);
            finchRobot.noteOn(494);
            finchRobot.wait(250);
            finchRobot.noteOn(139);
            finchRobot.wait(250);
            finchRobot.noteOn(147);
            finchRobot.wait(500);
            finchRobot.noteOn(185);
            finchRobot.wait(250);
            finchRobot.noteOn(147);
            finchRobot.wait(250);
            finchRobot.noteOn(139);
            finchRobot.wait(500);
            finchRobot.noteOn(165);
            finchRobot.wait(250);
            finchRobot.noteOn(139);
            finchRobot.wait(250);
            finchRobot.noteOn(494);
            finchRobot.wait(250);
            finchRobot.noteOn(440);
            finchRobot.wait(250);
            finchRobot.noteOn(494);
            finchRobot.wait(250);
            finchRobot.noteOn(139);
            finchRobot.wait(250);
            finchRobot.noteOn(494);
            finchRobot.wait(250);
            finchRobot.noteOn(440);
            finchRobot.wait(250);
            finchRobot.noteOn(370);
            finchRobot.wait(250);
            finchRobot.noteOn(293);
            finchRobot.wait(250);
            finchRobot.noteOn(392);
            finchRobot.wait(500);
            finchRobot.noteOn(494);
            finchRobot.wait(250);
            finchRobot.noteOn(392);
            finchRobot.wait(250);
            finchRobot.noteOn(370);
            finchRobot.wait(500);
            finchRobot.noteOn(440);
            finchRobot.wait(250);
            finchRobot.noteOn(370);
            finchRobot.wait(250);
            finchRobot.noteOn(329);
            finchRobot.wait(250);
            finchRobot.noteOn(293);
            finchRobot.wait(250);
            finchRobot.noteOn(277);
            finchRobot.wait(250);
            finchRobot.noteOn(329);
            finchRobot.wait(250);
            finchRobot.noteOn(293);
            finchRobot.wait(1000);
            finchRobot.noteOff();
        }
        static void ShortSong(Finch finchRobot)
        {
            finchRobot.noteOn(220);
            finchRobot.wait(250);
            finchRobot.noteOn(246);
            finchRobot.wait(250);
            finchRobot.noteOn(293);
            finchRobot.wait(250);
            finchRobot.noteOn(329);
            finchRobot.wait(250);
            finchRobot.noteOn(293);
            finchRobot.wait(500);
            finchRobot.noteOn(293);
            finchRobot.noteOff();
        }
        static void Dancer(Finch finchRobot)
        {
            finchRobot.setMotors(255, 255);
            finchRobot.wait(1000);
            finchRobot.setMotors(-100, 100);
            finchRobot.wait(4000);
            finchRobot.setMotors(0, 0);
            finchRobot.setMotors(100, -100);
            finchRobot.wait(1000);
            finchRobot.setMotors(100, 100);
            finchRobot.wait(3500);
            finchRobot.setMotors(-100, -100);
            finchRobot.wait(2000);
            finchRobot.setMotors(-255, 255);
            finchRobot.wait(2500);
            finchRobot.setMotors(255, -255);
            finchRobot.wait(2500);
            finchRobot.setMotors(-100, -100);
            finchRobot.wait(500);
            finchRobot.setMotors(100, 100);
            finchRobot.wait(500);
            finchRobot.setMotors(-100, 100);
            finchRobot.wait(3000);
            finchRobot.setMotors(0, 0);
        }
        static void ShortDance(Finch finchRobot)
        {
            finchRobot.setMotors(100, 100);
            finchRobot.wait(300);
            finchRobot.setMotors(-100, -100);
            finchRobot.wait(300);
            finchRobot.setMotors(100, -100);
            finchRobot.wait(300);
            finchRobot.setMotors(100, 100);
            finchRobot.wait(300);
            finchRobot.setMotors(-100, -100);
            finchRobot.wait(300);
            finchRobot.setMotors(-100, 100);
            finchRobot.wait(300);
            finchRobot.setMotors(0, 0);
        }

        #endregion

        #region FINCH ROBOT MANAGEMENT

        /// <summary>
        /// *****************************************************************
        /// *               Disconnect the Finch Robot                      *
        /// *****************************************************************
        /// </summary>
        /// <param name="finchRobot">finch robot object</param>
        static void DisplayDisconnectFinchRobot(Finch finchRobot)
        {
            Console.CursorVisible = false;

            DisplayScreenHeader("Disconnect Finch Robot");

            Console.WriteLine("\tAbout to disconnect from the Finch robot.");
            DisplayContinuePrompt();

            finchRobot.disConnect();

            Console.WriteLine("\tThe Finch robot is now disconnect.");

            DisplayMenuPrompt("Main Menu");
        }

        /// <summary>
        /// *****************************************************************
        /// *                  Connect the Finch Robot                      *
        /// *****************************************************************
        /// </summary>
        /// <param name="finchRobot">finch robot object</param>
        /// <returns>notify if the robot is connected</returns>
        static bool DisplayConnectFinchRobot(Finch finchRobot)
        {
            Console.CursorVisible = false;

            bool robotConnected;

            DisplayScreenHeader("Connect Finch Robot");

            Console.WriteLine("\tAbout to connect to Finch robot. Please be sure the USB cable is connected to the robot and computer now.");
            DisplayContinuePrompt();

            robotConnected = finchRobot.connect();

            if (robotConnected)
            {
                Console.WriteLine("\tRobot Connected");
                finchRobot.setLED(255, 0, 255);
                finchRobot.noteOn(261);
                finchRobot.wait(1000);
                finchRobot.setLED(0, 0, 0);
                finchRobot.noteOff();
                DisplayMenuPrompt("Main Menu");
            }
            else
            {
                Console.WriteLine("\t\tRobot not connected, please try again.");    //connection checked and sends back to menu
                DisplayContinuePrompt();
            }
                

            return robotConnected;
        }

        #endregion

        #region USER INTERFACE

        /// <summary>
        /// *****************************************************************
        /// *                     Welcome Screen                            *
        /// *****************************************************************
        /// </summary>
        static void DisplayWelcomeScreen()
        {
            Console.CursorVisible = false;

            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("\t\tFinch Control");
            Console.WriteLine();

            DisplayContinuePrompt();
        }

        /// <summary>
        /// *****************************************************************
        /// *                     Closing Screen                            *
        /// *****************************************************************
        /// </summary>
        static void DisplayClosingScreen()
        {
            Console.CursorVisible = false;

            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("\t\tThank you for using Finch Control!");
            Console.WriteLine();

            DisplayContinuePrompt();
        }

        /// <summary>
        /// display continue prompt
        /// </summary>
        static void DisplayContinuePrompt()
        {
            Console.WriteLine();
            Console.WriteLine("\tPress any key to continue.");
            Console.ReadKey();
        }

        /// <summary>
        /// display menu prompt
        /// </summary>
        static void DisplayMenuPrompt(string menuName)
        {
            Console.WriteLine();
            Console.WriteLine($"\tPress any key to return to the {menuName} Menu.");
            Console.ReadKey();
        }

        /// <summary>
        /// display screen header
        /// </summary>
        static void DisplayScreenHeader(string headerText)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("\t\t" + headerText);
            Console.WriteLine();
        }

        #endregion
    }
}



