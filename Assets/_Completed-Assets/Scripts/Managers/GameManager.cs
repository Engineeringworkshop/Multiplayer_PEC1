using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Complete
{
    public class GameManager : MonoBehaviour
    {
        [Header("Gameplay configuration")]
        public int m_NumRoundsToWin = 5;            // The number of rounds a single player has to win to win the game
        public float m_StartDelay = 3f;             // The delay between the start of RoundStarting and RoundPlaying phases
        public float m_EndDelay = 3f;               // The delay between the end of RoundPlaying and RoundEnding phases   
        public Text m_MessageText;                  // Reference to the overlay Text to display winning text, etc.
        public GameObject m_TankPrefab;             // Reference to the prefab the players will control

        public int m_MaxPlayers = 4;

        public Color[] m_Colors; // Array with the colours for the tanks
        public Transform[] m_SpawnPoints; // Array with the spawnpoint of the game
        public CinemachineVirtualCamera[] m_VirtualCameras; // Array of the scene cameras

        public List<GameObject> m_TankPlaying; // List of tanks currently playing

        [Header("Start Menu configuration")]
        public GameObject startMenuObject;           

        private int m_RoundNumber;                  // Which round the game is currently on
        private WaitForSeconds m_StartWait;         // Used to have a delay whilst the round starts
        private WaitForSeconds m_EndWait;           // Used to have a delay whilst the round or game ends
        private TankManager m_RoundWinner;          // Reference to the winner of the current round.  Used to make an announcement of who won
        private TankManager m_GameWinner;           // Reference to the winner of the game.  Used to make an announcement of who won

        private CameraControlCinemachine camControl;// Reference to the CameraControlCinemachine script for control during different phases

        private int numberOfPlayers;

        private void OnValidate()
        {
            if (camControl == null)
            {
                camControl = FindObjectOfType<CameraControlCinemachine>();
            }
        }

        private void Start()
        {
            // Create the delays so they only have to be made once
            m_StartWait = new WaitForSeconds (m_StartDelay);
            m_EndWait = new WaitForSeconds (m_EndDelay);

            // Activate start menu
            startMenuObject.SetActive(true);
        }

        public void StartGameWithNumberOfPlayerGame(int num)
        {
            numberOfPlayers = num;

            startMenuObject.SetActive(false);

            camControl.ChangeCameraLayout(num);

            Debug.Log("Juego iniciado con " + numberOfPlayers + " jugadores.");

            StartGame();
        }

        // Method to manage start game a fter selectin number of players
        private void StartGame()
        {

            SpawnAllTanks();

            // Once the tanks have been created and the camera is using them as targets, start the game
            StartCoroutine(GameLoop());
        }
		
		private void SpawnAllTanks()
		{
			Camera mainCam = GameObject.Find ("Main Camera").GetComponent<Camera>();

            m_TankPlaying.Clear();

            // For all the tanks...
            for (int i = 0; i < numberOfPlayers; i++)
			{
                SpawnTank(i);
            }

            mainCam.gameObject.SetActive (false);
		}

        public void SpawnNewTank()
        {
            if (numberOfPlayers < m_MaxPlayers)
            {
                Debug.Log("New tank joined");

                //SpawnTank(numberOfPlayers);

                numberOfPlayers++;

                camControl.ChangeCameraLayout(numberOfPlayers);
            }
        }

        public void SpawnTank(int i)
        {
            // ... create them, set their player number and references needed for control
            var newTank = Instantiate(m_TankPrefab, m_SpawnPoints[i].position, m_SpawnPoints[i].rotation);

            TankManager tankManager = newTank.GetComponent<TankManager>();

            tankManager.m_Instance = newTank;
            tankManager.m_PlayerNumber = i + 1;
            tankManager.m_PlayerColor = m_Colors[i];
            tankManager.m_SpawnPoint = m_SpawnPoints[i];
            tankManager.m_GameManager = this;
            tankManager.m_Camera = m_VirtualCameras[i];
            tankManager.m_Camera.Follow = newTank.transform;
            tankManager.m_Camera.LookAt = newTank.transform;
            tankManager.Setup();

            // Set controll scheme
            //SetControlScheme(i, tankManager.m_PlayerInput);

            m_TankPlaying.Add(newTank);
        }

        private void SetControllers()
        {
            int i = 0;

            foreach (var tank in m_TankPlaying)
            {
                TankManager tankManager = tank.GetComponent<TankManager>();
                SetControlScheme(i, tankManager.m_PlayerInput);
                i++;
            }
        }

        private void SetControlScheme(int i, PlayerInput playerInput)
        {
            playerInput.neverAutoSwitchControlSchemes = true;

            if (i == 0)
            {
                playerInput.SwitchCurrentControlScheme("Keyboard_1", Keyboard.current);
            }
            else if (i == 1)
            {
                playerInput.SwitchCurrentControlScheme("Keyboard_2", Keyboard.current);
            }
            else if (i == 2)
            {
                if (InputSystem.devices.Count >= 3)
                {
                    playerInput.SwitchCurrentControlScheme("Controller", Gamepad.all[0]);
                }
                else
                {
                    Debug.LogError("Controller is not connected");
                }
                
            }
            else if (i == 3)
            {
                if (InputSystem.devices.Count >= 4)
                {
                    playerInput.SwitchCurrentControlScheme("Controller", Gamepad.all[1]);
                }
                else
                {
                    Debug.LogError("Controller is not connected");
                }
            }
        }

        // This is called from start and will run each phase of the game one after another
        private IEnumerator GameLoop()
        {
            // Start off by running the 'RoundStarting' coroutine but don't return until it's finished
            yield return StartCoroutine (RoundStarting());

            // Once the 'RoundStarting' coroutine is finished, run the 'RoundPlaying' coroutine but don't return until it's finished
            yield return StartCoroutine (RoundPlaying());

            // Once execution has returned here, run the 'RoundEnding' coroutine, again don't return until it's finished
            yield return StartCoroutine (RoundEnding());

            // This code is not run until 'RoundEnding' has finished.  At which point, check if a game winner has been found
            if (m_GameWinner != null)
            {
                // If there is a game winner, restart the level
                SceneManager.LoadScene (0);
            }
            else
            {
                // If there isn't a winner yet, restart this coroutine so the loop continues
                // Note that this coroutine doesn't yield.  This means that the current version of the GameLoop will end
                StartCoroutine (GameLoop());
            }
        }


        private IEnumerator RoundStarting()
        {
            // As soon as the round starts reset the tanks and make sure they can't move
            ResetAllTanks();
            DisableTankControl();

            // Set controllers
            SetControllers();

            // Reset all cameras to the players number
            SetCameraOnActiveTanks();

            // Increment the round number and display text showing the players what round it is
            m_RoundNumber++;
            m_MessageText.text = "ROUND " + m_RoundNumber;

            // Wait for the specified length of time until yielding control back to the game loop
            yield return m_StartWait;
        }


        private IEnumerator RoundPlaying()
        {
            // As soon as the round begins playing let the players control the tanks
            EnableTankControl();

            // Clear the text from the screen
            m_MessageText.text = string.Empty;

            // While there is not one tank left...
            while (!OneTankLeft())
            {
                // ... return on the next frame
                yield return null;
            }
        }


        private IEnumerator RoundEnding()
        {
            // Stop tanks from moving
            DisableTankControl();

            // Clear the winner from the previous round
            m_RoundWinner = null;

            // See if there is a winner now the round is over
            m_RoundWinner = GetRoundWinner();

            // If there is a winner, increment their score
            if (m_RoundWinner != null)
                m_RoundWinner.m_Wins++;

            // Now the winner's score has been incremented, see if someone has one the game
            m_GameWinner = GetGameWinner();

            // Get a message based on the scores and whether or not there is a game winner and display it
            string message = EndMessage();
            m_MessageText.text = message;

            // Wait for the specified length of time until yielding control back to the game loop
            yield return m_EndWait;
        }


        // This is used to check if there is one or fewer tanks remaining and thus the round should end
        private bool OneTankLeft()
        {
            // Start the count of tanks left at zero
            int numTanksLeft = 0;

            // Go through all the tanks...
            foreach (var tank in m_TankPlaying)
            {
                // ... and if they are active, increment the counter
                if (tank.activeSelf)
                {
                    numTanksLeft++;
                }
            }

            // If there are one or fewer tanks remaining return true, otherwise return false
            return numTanksLeft <= 1;
        }

        // Method to set cameras on active tanks
        private void SetCameraOnActiveTanks()
        {
            int i = 0;
            // Asign cam to active tank
            foreach (var tank in m_TankPlaying)
            {
                if (tank.activeSelf)
                {
                    SetCamera(tank.GetComponent<TankManager>(), i);
                    i++;
                }
            }

            // Set cam layout to correct number
            camControl.ChangeCameraLayout(i);
        }

        private void SetCamera(TankManager tank, int cameraNumber)
        {
            tank.m_Camera = m_VirtualCameras[cameraNumber];
            tank.m_Camera.Follow = tank.transform;
            tank.m_Camera.LookAt = tank.transform;
        }

        // This function is to find out if there is a winner of the round
        // This function is called with the assumption that 1 or fewer tanks are currently active
        private TankManager GetRoundWinner()
        {
            // Go through all the tanks...
            foreach (var tank in m_TankPlaying)
            {
                // ... and if one of them is active, it is the winner so return it
                if (tank.activeSelf)
                {
                    return tank.GetComponent<TankManager>();
                }
            }
            // If none of the tanks are active it is a draw so return null
            return null;
        }


        // This function is to find out if there is a winner of the game
        private TankManager GetGameWinner()
        {
            // Go through all the tanks...
            foreach (var tank in m_TankPlaying)
            {
                TankManager tantManager = tank.GetComponent<TankManager>();
                // ... and if one of them has enough rounds to win the game, return it
                if (tantManager.m_Wins == m_NumRoundsToWin)
                {
                    return tank.GetComponent<TankManager>();
                }
            }
            // If no tanks have enough rounds to win, return null
            return null;
        }


        // Returns a string message to display at the end of each round.
        private string EndMessage()
        {
            // By default when a round ends there are no winners so the default end message is a draw
            string message = "DRAW!";

            // If there is a winner then change the message to reflect that
            if (m_RoundWinner != null)
            {
                message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";
            }

            // Add some line breaks after the initial message
            message += "\n\n\n\n";

            // Go through all the tanks and add each of their scores to the message
            foreach (var tank in m_TankPlaying)
            {
                TankManager tantManager = tank.GetComponent<TankManager>();
                message += tantManager.m_ColoredPlayerText + ": " + tantManager.m_Wins + " WINS\n";
            }

            // If there is a game winner, change the entire message to reflect that
            if (m_GameWinner != null)
            {
                message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";
            }

            return message;
        }

        // This function is used to turn all the tanks back on and reset their positions and properties
        private void ResetAllTanks()
        {
            foreach (var tank in m_TankPlaying)
            {
                tank.GetComponent<TankManager>().Reset();
            }
        }


        private void EnableTankControl()
        {
            foreach (var tank in m_TankPlaying)
            {
                tank.GetComponent<TankManager>().EnableControl();
            }
        }


        private void DisableTankControl()
        {
            foreach (var tank in m_TankPlaying)
            {
                tank.GetComponent<TankManager>().DisableControl();
            }
        }

        public void TankDefeated()
        {
            SetCameraOnActiveTanks();
        }
    }
}