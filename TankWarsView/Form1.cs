using GameController;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameModel;
using System.Windows.Forms.DataVisualization.Charting;

/// <summary>
/// Authors: Anish Narayanswamy and Ben Kempers
/// 
/// This class contains the View of the MVC, responsible for displaying what has been drawn from the DrawingPanel.
/// </summary>
namespace GameView
{
    public partial class Form1 : Form
    {
        private gameController controller;
        private World world;
        private DrawingPanel drawingPanel;

        public Form1()
        {
            InitializeComponent();

            controller = new gameController();
            world = controller.GetWorld();
            controller.UpdateArrived += OnFrame;

            ClientSize = new Size(900, 925);
            
            //Drawing panel instanciation into the Form
            drawingPanel = new DrawingPanel(world);
            drawingPanel.Location = new Point(0, 25);
            drawingPanel.Size = new Size(900, 925);
            drawingPanel.BackColor = Color.Black;
                
            //Key and mouse event handlers
            this.KeyDown += HandleKeyPressDown;
            this.KeyUp += HandleKeyPressUp;
            drawingPanel.MouseDown += HandleMousePressDown;
            drawingPanel.MouseUp += CancelMousePressDown;
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            // Disable the form controls
            ConnectButton.Enabled = false;
            NameTextBox.Enabled = false;
            IPAddressTextBox.Enabled = false;

            // Enable the global form to capture key presses
            KeyPreview = true;

            controller.ConnectButton(IPAddressTextBox.Text, NameTextBox.Text);
            this.Controls.Add(drawingPanel);
            drawingPanel.MouseMove += HandleMouseMovements;
        }

        /// <summary>
        /// Handler for the controller's UpdateArrived event
        /// </summary>
        private void OnFrame()
        {
            // Invalidate this form and all its children
            // This will cause the form to redraw as soon as it can
            MethodInvoker invalidator = new MethodInvoker(() => this.Invalidate(true));
            controller.SetPlayerID(drawingPanel);
            this.Invoke(invalidator);
        }

        /// <summary>
        /// Key down handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleKeyPressDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Application.Exit();
            if (e.KeyCode == Keys.W || e.KeyCode == Keys.A || e.KeyCode == Keys.S || e.KeyCode == Keys.D)
                controller.HandleMoveRequest(e);

            // Prevent other key handlers from running
            e.SuppressKeyPress = true;
            e.Handled = true;
        }

        /// <summary>
        /// Key down handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleKeyPressUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Application.Exit();
            if (e.KeyCode == Keys.W || e.KeyCode == Keys.A || e.KeyCode == Keys.S || e.KeyCode == Keys.D)
                controller.CancelMoveRequest(e);
        }

        /// <summary>
        /// Handle mouse down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMousePressDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                controller.HandleMouseRequest(e);
            if (e.Button == MouseButtons.Right)
                controller.HandleMouseRequest(e);
        }

        /// <summary>
        /// Handle mouse down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelMousePressDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                controller.CancelMouseRequest(e);
            if (e.Button == MouseButtons.Right)
                controller.CancelMouseRequest(e);
        }

        /// <summary>
        /// Handles mouse movements
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMouseMovements(object sender, MouseEventArgs e)
        {
            controller.MouseMoveRequest(e);
        }
    }
}
