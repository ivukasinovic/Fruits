using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using SharpGL.SceneGraph;
using SharpGL;
using Microsoft.Win32;


namespace AssimpSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Atributi

        /// <summary>
        ///	 Instanca OpenGL "sveta" - klase koja je zaduzena za iscrtavanje koriscenjem OpenGL-a.
        /// </summary>
        World m_world = null;

        #endregion Atributi

        #region Konstruktori

        public MainWindow()
        {
            // Inicijalizacija komponenti
            InitializeComponent();

            // Kreiranje OpenGL sveta
            try
            {
                m_world = new World(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\Fruits"), "pumpkin.3DS","orangeHalf2.3ds", (int)openGLControl.Width, (int)openGLControl.Height, openGLControl.OpenGL);
            }
            catch (Exception e)
            {
                MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta. Poruka greške: " + e.Message, "Poruka", MessageBoxButton.OK);
                this.Close();
            }
        }

        #endregion Konstruktori

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            m_world.Draw(args.OpenGL);
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            m_world.Initialize(args.OpenGL);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            m_world.Resize(args.OpenGL, (int)openGLControl.Width, (int)openGLControl.Height);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F10: this.Close(); break;
                //case Key.W: m_world.RotationX -= 5.0f; break;
                //case Key.S: m_world.RotationX += 5.0f; break;
                //case Key.A: m_world.RotationY -= 5.0f; break;
                //case Key.D: m_world.RotationY += 5.0f; break;
                case Key.F4: this.Close(); break;
                case Key.I: if(m_world.RotationX > -85) m_world.RotationX  -= 5.0f;  break;
                case Key.K: if (m_world.RotationX < 85) m_world.RotationX += 5.0f; Console.WriteLine(m_world.RotationX); break;
                case Key.J: if (m_world.RotationY > -85) m_world.RotationY -= 5.0f; Console.WriteLine(m_world.RotationY); break;
                case Key.L: if (m_world.RotationY < 85) m_world.RotationY += 5.0f; Console.WriteLine(m_world.RotationY); break;
                case Key.U: m_world.RotationZ -= 5.0f; break;
                case Key.O: m_world.RotationZ += 5.0f; break;
                case Key.V: m_world.SetTimer(); break;
                case Key.Add: m_world.SceneDistance -= 700.0f; break;
                case Key.Subtract: m_world.SceneDistance += 700.0f; break;
                case Key.F2:
                    OpenFileDialog opfModel = new OpenFileDialog();
                    bool result = (bool) opfModel.ShowDialog();
                    if (result)
                    {

                        try
                        {
                            World newWorld = new World(Directory.GetParent(opfModel.FileName).ToString(), Path.GetFileName(opfModel.FileName), Path.GetFileName(opfModel.FileName),(int)openGLControl.Width, (int)openGLControl.Height, openGLControl.OpenGL);
                            m_world.Dispose();
                            m_world = newWorld;
                            m_world.Initialize(openGLControl.OpenGL);
                        }
                        catch (Exception exp)
                        {
                            MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta:\n" + exp.Message, "GRESKA", MessageBoxButton.OK );
                        }
                    }
                    break;
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            m_world.fruitHeight = (float)e.NewValue*3;
        }

        private void rotation_value_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                m_world.leftFruitRotation = (float)Convert.ToDouble(rotation_value.Text);
            }
            catch(Exception)
            {
                rotation_value.Text = "0";
            }

        }

        private void ambientR_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            m_world.ambientC[0] = (float)e.NewValue;
            Console.WriteLine("Pomerio 1 za " + m_world.ambientC[0]);
           

           
        }
        private void ambientB_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            m_world.ambientC[1] = (float)e.NewValue;
            Console.WriteLine("Pomerio 2 za " + m_world.ambientC[0]);
        }
    }
}
