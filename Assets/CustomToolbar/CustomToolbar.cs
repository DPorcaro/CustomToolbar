using UnityEngine;
using UnityEditor;

namespace TomeSupreme.Levels.Pieces
{
    /// <summary>
    /// A custom toolbar with Play, Pause, Framestep and Slowmotion Buttons.
    /// </summary>
    public class CustomToolbar : EditorWindow
    {
        #region private attributes

        [SerializeField]
        private Texture2D _playTex = Resources.Load("Assets/CustomToolbar/Resources/Play.png") as Texture2D;
        [SerializeField]
        private Texture2D _pauseTex = Resources.Load("Assets/CustomToolbar/Resources/Pause.png") as Texture2D;
        [SerializeField]
        private Texture2D _framestepTex = Resources.Load("Assets/CustomToolbar/Resources/Framestep.png") as Texture2D;
        [SerializeField]
        private Texture2D _slowmotionTex = Resources.Load("Assets/CustomToolbar/Resources/Slowmotion.png") as Texture2D;

        private Rect tabSize;
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
        [MenuItem("Window/Custom Toolbar")]
        public static CustomToolbar OpenWindow()
        {
            return GetWindow<CustomToolbar>("Play Menu");
        }
        #endregion

        #region EditorWindow implementation
        private void OnInspectorUpdate()
        {
            Repaint();
        }

        private void OnGUI()
        {
            if (_playBtn == null) {
                AssembleButtons();
            }

            // Create one Group to contain all the buttons
            GUI.BeginGroup(new Rect((Screen.width / 2) - 81, 10, 180, 50));
            // Create a group to contain the first 4 buttons
            GUILayout.BeginHorizontal(GUILayout.Width(120));

            // Makes the Play button grayed out if the Game Tab is on Play mode
            if (EditorApplication.isPlaying) {
                if (GUILayout.Button(_playBtn, CreateDefaultButton(true)))
                { 
                    EditorApplication.ExecuteMenuItem("Edit/Play");
                }
            } // Make the Play button
            else if (GUILayout.Button(_playBtn, CreateDefaultButton(false)))
            {
                EditorApplication.ExecuteMenuItem("Edit/Play");
            }

            // Makes the Pause button grayed out if the Game Tab is Paused
            if (EditorApplication.isPaused)
            {
                if (GUILayout.Button(_pauseBtn, CreateDefaultButton(true)))
                {
                    EditorApplication.ExecuteMenuItem("Edit/Pause");
                    Time.timeScale = 1;
                }
            }// Make the Pause button
            else if (GUILayout.Button(_pauseBtn, CreateDefaultButton(false)))
            {
                EditorApplication.ExecuteMenuItem("Edit/Pause");
            }

            // Make the Framestep button
            if (GUILayout.Button(_framestepBtn, CreateDefaultButton(false)))
            {
                EditorApplication.ExecuteMenuItem("Edit/Step");
            }

            // Makes the Slowmotion button grayed out if the Time.timeScale is less than 1x
            if ((EditorApplication.isPlaying) && (Time.timeScale < 1))
            {
                if (GUILayout.Button(_slowmotionBtn, CreateDefaultButton(true)))
                {
                    Time.timeScale = 1;
                }
            } // Make the Slowmotion button
            else if (GUILayout.Button(_slowmotionBtn, CreateDefaultButton(false)))
            {
                if (EditorApplication.isPlaying && Time.timeScale >= 1f)
                {
                    Time.timeScale = 0.75f;
                }
            }
            GUILayout.BeginVertical(); // Creates a new group for the Slowmotion speed controls

            // Make the "plus" microbutton grayed out if Time.timeScale is between 0.75 and 1
            if ((EditorApplication.isPlaying) && (Time.timeScale >= 0.75f) && (Time.timeScale < 1))
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
            if ((EditorApplication.isPlaying) && (Time.timeScale <= 0.25f))
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

            if ((EditorApplication.isPlaying)) // When the Game Tab is on Play Mode, shows the current Time.timeScale
            {
                GUILayout.Label("Play Speed: " + Time.timeScale.ToString() + "x", GUI.skin.label);
            }

            GUILayout.EndHorizontal();
            GUI.EndGroup();
        }
        #endregion

        #region private functions
        // Creates a separate GUIContent for each of the 4 first buttons, assigning their tooltips and icons
        private void AssembleButtons()
        {
            _playBtn = new GUIContent(_playTex, "Play");
            _pauseBtn = new GUIContent(_pauseTex, "Pause");
            _slowmotionBtn = new GUIContent(_slowmotionTex, "Slow Motion");
            _framestepBtn = new GUIContent(_framestepTex, "Next Frame");
        }

        // Creates a GUIStyle for bigger buttons, with custom Width, Height, Margin and Background Texture
        private GUIStyle CreateDefaultButton(bool pressed)
        {
            GUIStyle botao = new GUIStyle(GUI.skin.button);
            botao.fixedWidth = 36;
            botao.fixedHeight = 26;
            botao.margin = new RectOffset(0, 0, 0, 0);

            if (pressed)
            {
                botao.normal.background = MakeTex(1, 1, new Color(0f, 0f, 0f, 0.5f));
            }

            return botao;
        }

        // Creates a GUIStyle for smaller buttons, with custom Width, Height, Margin and Background Texture
        private GUIStyle CreateMicroButton(bool pressed)
        {
            GUIStyle botao = new GUIStyle(GUI.skin.button);
            botao.fixedWidth = 17;
            botao.fixedHeight = 13;
            botao.margin = new RectOffset(0, 0, 0, 0);

            if (pressed) {
                botao.normal.background = MakeTex(1, 1, new Color(0f, 0f, 0f, 0.5f));
            }

            return botao;
        }

        // Creates a custom Texture2D (actually I just copied from the Unity Forum)
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

        // Increases the play speed from 0.1 to 1, in 0.25 increments
        private void IncPlaySpeed()
        {
            if (Time.timeScale < 1f)
                if (Time.timeScale < 0.75f)
                {
                    if (Time.timeScale < 0.25f)
                    {
                        Time.timeScale = 0f;
                    }
                    Time.timeScale = Time.timeScale + 0.25f;
                }else
                {
                    Time.timeScale = 1f;
                }
        }

        // Decreases the play speed from 0.75 to 0.1, in 0.25 increments
        private void DecPlaySpeed()
        {
            if (Time.timeScale < 1f)
            {
                if (Time.timeScale > 0.25f)
                {
                    Time.timeScale = Time.timeScale - 0.25f;
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