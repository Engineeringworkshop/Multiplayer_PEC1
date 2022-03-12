using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Complete
{
    public class PlayerManager : MonoBehaviour
    {
        [Header("Components to interact")]
        [SerializeField] private CameraControlCinemachine camControl;// Reference to the CameraControlCinemachine script for control during different phases
        [SerializeField] private GameManager gameManager;
        [SerializeField] private PlayerInputManager playerInputManager;

        public int m_MaxPlayers = 4;

        public Color[] m_Colors; // Array with the colours for the tanks
        public Transform[] m_SpawnPoints; // Array with the spawnpoint of the game
        public CinemachineVirtualCamera[] m_VirtualCameras; // Array of the scene cameras

        public List<GameObject> m_TankPlaying; // List of tanks currently playing

        public int numberOfInitialPlayers;

        private void OnValidate()
        {
            if (camControl == null)
            {
                camControl = FindObjectOfType<CameraControlCinemachine>();
            }

            if (gameManager == null)
            {
                gameManager = FindObjectOfType<GameManager>();
            }
        }

        private void Start()
        {
            numberOfInitialPlayers = 0;

            playerInputManager = GetComponent<PlayerInputManager>();
        }

        public void StartGameWithNumberOfPlayerGame(int num)
        {
            numberOfInitialPlayers = num;

            camControl.ChangeCameraLayout(num);

            SpawnInitialTanks();

            Debug.Log("Juego iniciado con " + numberOfInitialPlayers + " jugadores.");
        }

        // Method to create all initial tanks 
        public void SpawnInitialTanks()
        {
            m_TankPlaying.Clear();

            // For all the tanks...
            for (int i = 0; i < numberOfInitialPlayers; i++)
            {
                // ... create them, set their player number and references needed for control
                var newTank = Instantiate(playerInputManager.playerPrefab, m_SpawnPoints[i].position, m_SpawnPoints[i].rotation);
                // The tanks will be configurated on the method OnPlayerJoined when PlayerInputManager detected the new game object with PlayerInput componen
            }

            // Remap controlls on next frame
            StartCoroutine(RemapControlls());
        }

        // Method to configure a player
        private void ConfigurePlayer(int i, GameObject gameObject)
        {
            // SetPosition and orientation
            gameObject.transform.position = m_SpawnPoints[i].position;
            gameObject.transform.rotation = m_SpawnPoints[i].rotation;

            TankManager tankManager = gameObject.GetComponent<TankManager>();

            tankManager.m_Instance = gameObject;
            tankManager.m_PlayerNumber = i + 1;
            tankManager.m_PlayerColor = m_Colors[i];
            tankManager.m_SpawnPoint = m_SpawnPoints[i];
            tankManager.m_GameManager = gameManager;
            tankManager.m_Camera = m_VirtualCameras[i];
            tankManager.m_Camera.Follow = gameObject.transform;
            tankManager.m_Camera.LookAt = gameObject.transform;
            tankManager.Setup();

            m_TankPlaying.Add(gameObject);
        }

        public void OnPlayerJoined(PlayerInput playerInput)
        {
            //Debug.Log("PlayerInput ID: " + playerInput.playerIndex);

            // If the tank is not in the list => configure it and include it
            if (!m_TankPlaying.Contains(playerInput.gameObject))
            {
                // New tank configuration
                ConfigurePlayer(playerInput.playerIndex, playerInput.gameObject);

                // Update camera layout
                camControl.ChangeCameraLayout(m_TankPlaying.Count);
            }            
        }

        // method to asign all controllers
        public void SetControllers()
        {
            int i = 0;

            foreach (var tank in m_TankPlaying)
            {
                TankManager tankManager = tank.GetComponent<TankManager>();
                SetControlScheme(i, tankManager.m_PlayerInput);
                i++;
            }
        }

        // Method to asign controllers
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

        // Coroutine to assign controlls of initial player the next frame. (To be able to use 2 action map on the keyboard)
        private IEnumerator RemapControlls()
        {
            yield return null;

            SetControllers();
        }

    }
}
