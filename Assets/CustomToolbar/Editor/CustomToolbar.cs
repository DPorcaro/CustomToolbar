using UnityEngine;
using UnityEditor;
using static UnityEditor.EditorApplication;

namespace CustomEditor.Windows
{
    /// <summary>
    /// A custom toolbar with Play, Pause, Framestep and Slowmotion Buttons.
    /// </summary>
    public class CustomToolbar : EditorWindow
    {
        #region private attributes

        private const string path = "Assets/CustomToolbar/Resources/";

        private Texture2D _playTex;
        private Texture2D _pauseTex;
        private Texture2D _framestepTex;
        private Texture2D _slowmotionTex;

        private GUIContent _playBtn;
        private GUIContent _pauseBtn;
        private GUIContent _framestepBtn;
        private GUIContent _slowmotionBtn;

        #endregion

        #region public static functions
        /// <summary>
        /// Adds a menu entry in the windows menu.
        /// </summary>
        /// <returns></returns>
        [MenuItem("Window/Plug and Boom/Custom Toolbar")]
        public static CustomToolbar OpenWindow()
        {
            CustomToolbar customToolbar = GetWindow<CustomToolbar>("Custom Toolbar");
            customToolbar.minSize = new Vector2(250, 60);
            return customToolbar;
        }
        #endregion

        #region EditorWindow implementation
        private void OnEnable()
        {
            _playTex = AssetDatabase.LoadAssetAtPath<Texture2D>($"{path}Play.png");
            _pauseTex = AssetDatabase.LoadAssetAtPath<Texture2D>($"{path}Pause.png");
            _framestepTex = AssetDatabase.LoadAssetAtPath<Texture2D>($"{path}Framestep.png");
            _slowmotionTex = AssetDatabase.LoadAssetAtPath<Texture2D>($"{path}Slowmotion.png");
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }

        private void OnGUI()
        {
            if (_playBtn == null)
            {
                AssembleButtons();
            }

            // Create one Group to contain all the buttons
            GUI.BeginGroup(new Rect((this.position.width / 2) - 81, 10, 180, 50));

            // Create a group to contain the first 4 buttons
            GUILayout.BeginHorizontal(GUILayout.Width(120));

            // Makes the Play button grayed out if the Game Tab is on Play mode
            if (isPlaying)
            {
                if (GUILayout.Button(_playBtn, CreateDefaultButton(true)))
                { 
                    ExecuteMenuItem("Edit/Play");
                }
            } // Make the Play button
            else if (GUILayout.Button(_playBtn, CreateDefaultButton(false)))
            {
                ExecuteMenuItem("Edit/Play");
            }

            // Makes the Pause button grayed out if the Game Tab is Paused
            if (isPaused)
            {
                if (GUILayout.Button(_pauseBtn, CreateDefaultButton(true)))
                {
                    ExecuteMenuItem("Edit/Pause");
                    Time.timeScale = 1;
                }
            }// Make the Pause button
            else if (GUILayout.Button(_pauseBtn, CreateDefaultButton(false)))
            {
                ExecuteMenuItem("Edit/Pause");
            }

            // Make the Framestep button
            if (GUILayout.Button(_framestepBtn, CreateDefaultButton(false)))
            {
                ExecuteMenuItem("Edit/Step");
            }

            // Makes the Slowmotion button grayed out if the Time.timeScale is less than 1x
            if (isPlaying && Time.timeScale < 1)
            {
                if (GUILayout.Button(_slowmotionBtn, CreateDefaultButton(true)))
                {
                    Time.timeScale = 1;
                }
            } // Make the Slowmotion button
            else if (GUILayout.Button(_slowmotionBtn, CreateDefaultButton(false)))
            {
                if (isPlaying && Time.timeScale >= 1f)
                {
                    Time.timeScale = 0.75f;
                }
            }
            GUILayout.BeginVertical(); // Creates a new group for the Slowmotion speed controls

            // Make the "plus" microbutton grayed out if Time.timeScale is between 0.75 and 1
            if (isPlaying && Time.timeScale >= 0.75f && Time.timeScale < 1)
            {
                if (GUILayout.Button("+", CreateMicroButton(true)))
                {
                    IncPlaySpeed();
                }
            }// Make the "plus" microbutton, which increases Time.timeScale in 0.25 increments (up to 1x speed)
            else if (GUILayout.Button("+", CreateMicroButton(false)))
            {
                IncPlaySpeed();
            }
            if (isPlaying && Time.timeScale <= 0.25f)
            {
                if (GUILayout.Button("-", CreateMicroButton(true)))
                {
                    DecPlaySpeed();
                }
            }// Make the "minus" microbutton, which decreases Time.timeScale in 0.25 increments (up to 1x speed)
            else if (GUILayout.Button("-", CreateMicroButton(false)))
            {
                DecPlaySpeed();
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(GUILayout.Width(120)); // Creates a new group for a label

            if (isPlaying) // When the Game Tab is on Play Mode, shows the current Time.timeScale
            {
                GUILayout.Label($"Play Speed: {Time.timeScale} x", GUI.skin.label);
            }

            GUILayout.EndHorizontal();
            GUI.EndGroup();
        }
        #endregion

        #region private functions
        /// <summary>
        /// Creates a separate GUIContent for Play, Pause, Slow motion and Next Frame buttons, assigning their tooltips and icons
        /// </summary>
        /// <returns></returns>
        private void AssembleButtons()
        {
            _playBtn = new GUIContent(_playTex, "Play");
            _pauseBtn = new GUIContent(_pauseTex, "Pause");
            _slowmotionBtn = new GUIContent(_slowmotionTex, "Slow Motion");
            _framestepBtn = new GUIContent(_framestepTex, "Next Frame");
        }

        /// <summary>
        /// Creates a GUIStyle for bigger buttons, with custom Width, Height, Margin and Background Texture
        /// </summary>
        /// <returns></returns>
        private GUIStyle CreateDefaultButton(bool pressed)
        {
            GUIStyle button = new GUIStyle(GUI.skin.button)
            {
                fixedWidth = 36,
                fixedHeight = 26,
                margin = new RectOffset(0, 0, 0, 0)
            };

            if (pressed)
            {
                button.normal.background = MakeTex(1, 1, new Color(0f, 0f, 0f, 0.5f));
            }

            return button;
        }

        /// <summary>
        /// Creates a GUIStyle for smaller buttons, with custom Width, Height, Margin and Background Texture
        /// </summary>
        /// <returns></returns>
        private GUIStyle CreateMicroButton(bool pressed)
        {
            GUIStyle button = new GUIStyle(GUI.skin.button)
            {
                fixedWidth = 17,
                fixedHeight = 13,
                margin = new RectOffset(0, 0, 0, 0)
            };

            if (pressed)
            {
                button.normal.background = MakeTex(1, 1, new Color(0f, 0f, 0f, 0.5f));
            }

            return button;
        }

        /// <summary>
        /// Creates a custom Texture2D (actually I just copied from the Unity Forum)
        /// </summary>
        /// <returns></returns>
        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        /// <summary>
        /// Increases the play speed from 0.1 to 1, in 0.25 increments
        /// </summary>
        /// <returns></returns>
        private void IncPlaySpeed()
        {
            if (Time.timeScale < 1f)
                if (Time.timeScale < 0.75f)
                {
                    if (Time.timeScale < 0.25f)
                    {
                        Time.timeScale = 0f;
                    }
                    Time.timeScale += 0.25f;
                }
                else
                {
                    Time.timeScale = 1f;
                }
        }

        /// <summary>
        /// Decreases the play speed from 0.75 to 0.1, in 0.25 increments
        /// </summary>
        /// <returns></returns>
        private void DecPlaySpeed()
        {
            if (Time.timeScale < 1f)
            {
                if (Time.timeScale > 0.25f)
                {
                    Time.timeScale -= 0.25f;
                }
                else
                {
                    Time.timeScale = 0.1f;
                }
            }
        }
        #endregion
    }
}