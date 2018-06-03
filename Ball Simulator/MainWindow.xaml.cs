using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.ComponentModel;
using System.Threading;


namespace Ball_Simulator
{
    public partial class MainWindow : Window
    {
        Ellipse myEllipse = new Ellipse();
        Ball ball;
        double time;
        double timeIncrement = .05;
        double[] centerCoord = new double[2];

        double ballAreaWidth = 700;
        double ballAreaHeight = 300;

        public MainWindow()
        {
            InitializeComponent();

            centerCoord[0] = ballAreaWidth / 2;
            centerCoord[1] = ballAreaHeight / 2;

            ball = new Ball(1, 10, centerCoord[0], centerCoord[1], 0, 0, .85, 0.65, -9.81, 0.00);

            myEllipse.Stroke = Brushes.Black;
            myEllipse.Fill = Brushes.DarkBlue;
            myEllipse.HorizontalAlignment = HorizontalAlignment.Center;
            myEllipse.VerticalAlignment = VerticalAlignment.Center;
            myEllipse.Width = ball.radius;
            myEllipse.Height = ball.radius;

            mainCanvas.Children.Add(myEllipse);

            Canvas.SetLeft(myEllipse, ball.x);
            Canvas.SetTop(myEllipse, ball.y);

            txt_x.Text = "x:\t" + RoundDecimals(ball.x, 2);
            txt_y.Text = "y:\t" + RoundDecimals(ball.y, 2);
        }

        #region Pointers

        private async void btn_beginSimulation_Click(object sender, RoutedEventArgs e)
        {
            await BeginSimulation();
        }

        private void btn_restartSimulation_Click(object sender, RoutedEventArgs e)
        {
            RestartSimulation();
        }

        #endregion

        /// <summary>
        /// Asynchronously starts the simulation.
        /// </summary>
        /// <returns></returns>
        async Task BeginSimulation()
        {
            btn_beginSimulation.IsEnabled = false;
            ball = new Ball(
                1,
                Double.Parse(txt_radius.Text),
                centerCoord[0],
                centerCoord[1],
                Double.Parse(txt_vX.Text),
                Double.Parse(txt_vY.Text),
                0.85,
                Double.Parse(txt_restitution.Text),
                Double.Parse(txt_gravity.Text),
                Double.Parse(txt_friction.Text));

            await Task.Run(async () =>
            {
                while (!ball.isAtRest)
                {
                    await Task.Delay(1);
                    await Dispatcher.BeginInvoke(new Action(() =>
                    {
                        UpdateUI();
                        DrawTrail();
                    }), null);
                }
            });
        }

        /// <summary>
        /// Restarts the simulation and pauses the movement of the ball.
        /// </summary>
        void RestartSimulation()
        {
            btn_beginSimulation.IsEnabled = true;
            ball.isAtRest = true;

            ball = new Ball(1, 10, centerCoord[0], centerCoord[1], 0, 0, .85, 0.65, 0, 0.00);
            Canvas.SetLeft(myEllipse, ball.x);
            Canvas.SetTop(myEllipse, ball.y);
        }

        void UpdateUI()
        {
            //Update the time and ball position
            time += timeIncrement;
            ball.Move(timeIncrement, ballAreaWidth, ballAreaHeight);
            
            //Draw the ball
            Canvas.SetLeft(myEllipse, ball.x);
            Canvas.SetTop(myEllipse, ballAreaHeight - ball.y);
            
            //Update the text in the corner of the screen that displays the coordinates of the ball
            txt_x.Text = "x:\t" + RoundDecimals(ball.x, 2);
            txt_y.Text = "y:\t" + RoundDecimals(ball.y, 2);
        }

        /// <summary>
        /// Draws te breadcrumb trail of the ball as it moves.
        /// </summary>
        void DrawTrail()
        {
            Ellipse p = new Ellipse();
            p.Stroke = Brushes.Black;
            p.Fill = Brushes.DarkBlue;
            p.HorizontalAlignment = HorizontalAlignment.Center;
            p.VerticalAlignment = VerticalAlignment.Center;
            p.Width = ball.radius * 0.1;
            p.Height = ball.radius * 0.1;

            mainCanvas.Children.Add(p);

            Canvas.SetLeft(p, ball.x + ball.radius / 2);
            Canvas.SetTop(p, ballAreaHeight - ball.y + ball.radius / 2);
        }

        /// <summary>
        /// Rounds lengthy decimal numbers that are outputted to the window.
        /// </summary>
        /// <param name="num">The number to round.</param>
        /// <param name="rnd">The number decimal places to round to.</param>
        /// <returns></returns>
        string RoundDecimals(double num, int rnd)
        {
            string newNum = ""; 
            if ((num + "").Contains("."))
            {
                int lengthOfNonDecimalString = 0;
                for (int i = 0; i < (num + "").IndexOf('.'); i++)
                {
                    newNum += (num + "").ElementAt(i);
                    lengthOfNonDecimalString++;
                }

                if (rnd > 0) newNum += ".";
                lengthOfNonDecimalString++;

                for (int i = (num + "").IndexOf('.') + 1; i < Math.Min(rnd + lengthOfNonDecimalString, (num + "").Length - (num + "").IndexOf('.')); i++)
                {
                    newNum += (num + "").ElementAt(i);
                }

                return newNum;
            }
            else return num + "";
        }
    }
}
