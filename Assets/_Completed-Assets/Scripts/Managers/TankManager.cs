using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Complete
{
    [Serializable]
    public class TankManager : MonoBehaviour
    {
        // This class is to manage various settings on a tank
        // It works with the GameManager class to control how the tanks behave
        // and whether or not players have control of their tank in the 
        // different phases of the game

        public Color m_PlayerColor;                             // This is the color this tank will be tinted
        public Transform m_SpawnPoint;                          // The position and direction the tank will have when it spawns
        [HideInInspector] public int m_PlayerNumber;            // This specifies which player this the manager for
        [HideInInspector] public string m_ColoredPlayerText;    // A string that represents the player with their number colored to match their tank
        [HideInInspector] public GameObject m_Instance;         // A reference to the instance of the tank when it is created
        [HideInInspector] public int m_Wins;                    // The number of wins this player has so far
        [HideInInspector] public CinemachineVirtualCamera m_Camera;

        private TankMovement m_Movement;                        // Reference to tank's movement script, used to disable and enable control
        private TankShooting m_Shooting;                        // Reference to tank's shooting script, used to disable and enable control
        private GameObject m_CanvasGameObject;                  // Used to disable the world space UI during the Starting and Ending phases of each round
        private Rigidbody m_rigidBody;
        private BoxCollider m_boxCollider;

        private TankHealth m_tankHealth;

        [SerializeField] private GameObject m_TankRenders;

        [HideInInspector] public GameManager m_GameManager;

        [HideInInspector] public PlayerInput m_PlayerInput;

        public bool isActive;

        public void Setup ()
        {
            // Get references to the components
            m_Movement = m_Instance.GetComponent<TankMovement>();
            m_Shooting = m_Instance.GetComponent<TankShooting>();
            m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;
            m_PlayerInput = m_Instance.GetComponent<PlayerInput>();
            m_rigidBody = m_Instance.GetComponent<Rigidbody>();
            m_boxCollider = m_Instance.GetComponent<BoxCollider>();

            m_tankHealth = m_Instance.GetComponent<TankHealth>();

            // Set the player numbers to be consistent across the scripts
            m_Movement.m_PlayerNumber = m_PlayerNumber;
            m_Shooting.m_PlayerNumber = m_PlayerNumber;

            // Create a string using the correct color that says 'PLAYER 1' etc based on the tank's color and the player's number
            m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";

            // Get all of the renderers of the tank
            MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();

            // Go through all the renderers...
            for (int i = 0; i < renderers.Length; i++)
            {
                // ... set their material color to the color specific to this tank
                renderers[i].material.color = m_PlayerColor;
            }

            isActive = true;
        }


        // Used during the phases of the game where the player shouldn't be able to control their tank
        public void DisableControl()
        {
            m_Movement.enabled = false;
            m_Shooting.enabled = false;

            m_CanvasGameObject.SetActive (false);
        }


        // Used during the phases of the game where the player should be able to control their tank
        public void EnableControl()
        {
            m_Movement.enabled = true;
            m_Shooting.enabled = true;

            m_CanvasGameObject.SetActive (true);
        }


        // Used at the start of each round to put the tank into it's default state
        public void Reset()
        {
            m_Instance.transform.position = m_SpawnPoint.position;
            m_Instance.transform.rotation = m_SpawnPoint.rotation;

            EnableTank();

            m_tankHealth.ResetHealth();
        }

        // Method to disable components that interacts with game
        public void DisableTank()
        {
            m_Movement.enabled = false;     // Disable movement script
            m_Shooting.enabled = false;       // Disable shoting script
            m_rigidBody.isKinematic = true; // Set rigidbody to kinematic to disable physics update
            m_boxCollider.enabled = false; // set box collider off
            m_TankRenders.SetActive(false); // set tank renders off

            m_CanvasGameObject.SetActive(false);

            isActive = false;
        }

        // Method to enable components that interacts with game
        private void EnableTank()
        {
            m_Movement.enabled = true;     // Enable movement script
            m_Shooting.enabled = true;       // Enable shoting script
            m_rigidBody.isKinematic = false; // Set rigidbody to no kinematic to enable physics update
            m_boxCollider.enabled = true; // set box collider on
            m_TankRenders.SetActive(true); // set tank renders on

            m_CanvasGameObject.SetActive(true);

            isActive = true;
        }
    }
}