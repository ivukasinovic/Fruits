// -----------------------------------------------------------------------
// <file>World.cs</file>
// <copyright>Grupa za Grafiku, Interakciju i Multimediju 2013.</copyright>
// <author>Srđan Mihić</author>
// <author>Aleksandar Josić</author>
// <summary>Klasa koja enkapsulira OpenGL programski kod.</summary>
// -----------------------------------------------------------------------
using System;
using Assimp;
using System.IO;
using System.Reflection;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Core;
using SharpGL;

namespace AssimpSample
{


    /// <summary>
    ///  Klasa enkapsulira OpenGL kod i omogucava njegovo iscrtavanje i azuriranje.
    /// </summary>
    public class World : IDisposable
    {
        #region Atributi

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        private AssimpScene m_scene;
        private AssimpScene m_scene2;

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        private float m_xRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        private float m_yRotation = 0.0f;

        private float m_zRotation = 0.0f;

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        private float m_sceneDistance = 7000.0f;

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width;

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height;

        #endregion Atributi

        #region Properties

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        public AssimpScene Scene
        {
            get { return m_scene; }
            set { m_scene = value; }
        }
        public AssimpScene Scene2
        {
            get { return m_scene2; }
            set { m_scene2 = value; }
        }
        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        public float RotationX
        {
            get { return m_xRotation; }
            set { m_xRotation = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        public float RotationY
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        public float RotationZ
        {
            get { return m_zRotation; }
            set { m_zRotation = value; }
        }

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        public float SceneDistance
        {
            get { return m_sceneDistance; }
            set { m_sceneDistance = value; }
        }

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        public int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        #endregion Properties

        #region Konstruktori

        /// <summary>
        ///  Konstruktor klase World.
        /// </summary>
        public World(String scenePath, String sceneFileName, String sceneFileName2, int width, int height, OpenGL gl)
        {
            this.m_scene = new AssimpScene(scenePath, sceneFileName, gl);
            this.m_scene2 = new AssimpScene(scenePath, sceneFileName2, gl);
            this.m_width = width;
            this.m_height = height;
        }

        /// <summary>
        ///  Destruktor klase World.
        /// </summary>
        ~World()
        {
            this.Dispose(false);
        }

        #endregion Konstruktori

        #region Metode

        /// <summary>
        ///  Korisnicka inicijalizacija i podesavanje OpenGL parametara.
        /// </summary>
        public void Initialize(OpenGL gl)
        {
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Color(1f, 0f, 0f);
            // Model sencenja na flat (konstantno)
            gl.ShadeModel(OpenGL.GL_FLAT);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);
            m_scene.LoadScene();
            m_scene.Initialize();
            m_scene2.LoadScene();
            m_scene2.Initialize();
        }

        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);
            //gl.LoadIdentity();
            gl.Viewport(0, 0, m_width, m_height);

            WriteText(gl);         

            gl.PushMatrix();

            gl.Translate(0.0f, 0.0f, -m_sceneDistance);
            gl.Scale(10f, 10f, 10f);
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f); 
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);
            gl.Rotate(m_zRotation, 0.0f, 0.0f, 1.0f);
            gl.Translate(40f, 200f, 0.0f);
           



            //iscrtavanje 1/2jabuke
            gl.PushMatrix();
            gl.Translate(-180f,-30f, -70f);
            gl.Rotate(90f, 0f, 0f);
            gl.Scale(20f, 20f, 20f);
            m_scene.Draw();
            gl.PopMatrix();
            //iscrtavanje 2/2jabuke
            gl.PushMatrix();
            gl.Rotate(90f, 180f, 0f);
            gl.Translate(-172f, -70f, -173f);
            
            gl.Scale(20f, 20f, 20f);
            m_scene.Draw();
            gl.PopMatrix();



            //iscrtavanje narandza
            gl.PushMatrix();
            gl.Rotate(180f, 0f, 0f);
            gl.Translate(-125f, 137f,-50f);
            gl.Scale(150f, 150f, 150f);
            m_scene2.Draw();
            gl.PopMatrix();
            //iscrtavanje narandza
            gl.PushMatrix();
            gl.Rotate(0f, 0f, 0f);
            gl.Translate(-125f, -255f, 12f);
            gl.Scale(150f, 150f, 150f);
            m_scene2.Draw();
            gl.PopMatrix();

            //jabuka postolje
            gl.PushMatrix();
            gl.Translate(-5.0f, -100.0f, 0.0f);
            gl.Color(0.0f, 0.7f, 0.0f);
            Cylinder cyl = new Cylinder
            {
                TopRadius = 20f,
                BaseRadius = 20f,
                Height = 20f
            };
            
            cyl.CreateInContext(gl);
            cyl.Render(gl, RenderMode.Render);
            Disk disk = new Disk();
            gl.Translate(0.0f, 0.0f, 20f);
            disk.InnerRadius = 0f;
            disk.OuterRadius = 20f;
            disk.CreateInContext(gl);
            disk.Render(gl, RenderMode.Render);

            gl.Translate(-150f, -100f, -20f);
            cyl.Render(gl, RenderMode.Render);
            gl.Translate(0.0f, 0.0f, 20f);
            disk.Render(gl, RenderMode.Render);
            gl.PopMatrix();

            
            //iscrtavanje podloge
            gl.Color(0.5f, 0.5f, 0.5f);
            gl.Translate(0.0f, -480f, -1f);
            gl.Begin(OpenGL.GL_QUADS);
            gl.Vertex(200f, 100f);
            gl.Vertex(200f, 480f);
            gl.Vertex(-300f, 480f);
            gl.Vertex(-300f, 100f);
            gl.End();

            gl.Color(0.7f, 0.0f, 0.0f);

            //dole zid
            gl.PushMatrix();
            Cube wall = new Cube();
            gl.Translate(-45.0f, 150.0f, 10.0f);
            gl.Scale(200f, 5f, 10f);
            wall.Render(gl, RenderMode.Render);
            gl.PopMatrix();
            //gore zid
            gl.PushMatrix();
            gl.Translate(-45.0f, 430.0f, 10.0f);
            gl.Scale(200f, 5f, 10f);
            wall.Render(gl, RenderMode.Render);
            gl.PopMatrix();
            //levo zid
            gl.PushMatrix();
            gl.Rotate(0.0f, 0.0f, 90.0f);
            gl.Translate(290.0f, 240.0f, 10.0f);
            gl.Scale(135f, 5f, 10f);
            wall.Render(gl, RenderMode.Render);
            gl.PopMatrix();
            //desni zid
            gl.PushMatrix();
            gl.Rotate(0.0f, 0.0f, 90.0f);
            gl.Translate(290.0f, -150.0f, 10.0f);
            gl.Scale(135f, 5f, 10f);
            wall.Render(gl, RenderMode.Render);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(-50f, 250f, 0);
            gl.Scale(40f, 30f, 0f);
            Grid grid = new Grid();
            grid.Render(gl, RenderMode.Design);
            gl.PopMatrix();

            gl.PopMatrix();
            

            // Oznaci kraj iscrtavanja
            gl.Flush();
        }

        private void WriteText(OpenGL gl)
        {
            var yTranslate = 0.0f;
            
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PushMatrix();
            gl.LoadIdentity();
            gl.Ortho2D(-100f, 0f, -20f, 20f); //definise projekciju 2D

            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();
           
            gl.Translate(-27.0f, 20.0f, 0.0f);
            gl.Color(1f, 0.0f, 0.0f);

            gl.PushMatrix();
            gl.Translate(0f, yTranslate -= 1f, 0f);
            gl.DrawText3D("Helvetica", 12f, 0.0f, 0.0f, "Predmet: Racunarska Grafika");
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(0f, yTranslate -= 1f, 0f);
            gl.DrawText3D("Helvetica", 12f, 0.0f, 0.0f, "Sk. god: 2017/18");
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(0f, yTranslate -= 1f, 0f);
            gl.DrawText3D("Helvetica", 12f, 0.0f, 0.0f, "Ime: Ivan");
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(0f, yTranslate -= 1f, 0f);
            gl.DrawText3D("Helvetica", 12f, 0.0f, 0.0f, "Prezime: Vukasinovic");
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(0f, yTranslate -= 1f, 0f);
            gl.DrawText3D("Arial", 12f, 0.0f, 0.0f, "Sifra zad: 15.2");
            gl.PopMatrix();
            gl.Viewport(m_width / 2, 0, m_width / 2, m_height / 2);
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }



        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;
            gl.Viewport(0, 0, width, height);
            gl.MatrixMode(OpenGL.GL_PROJECTION);      // selektuj Projection Matrix
            gl.LoadIdentity();
            gl.Perspective(45f, (double)width / height, 1f, 20000f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();                // resetuj ModelView Matrix
        }

        /// <summary>
        ///  Implementacija IDisposable interfejsa.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_scene.Dispose();
            }
        }

        #endregion Metode

        #region IDisposable metode

        /// <summary>
        ///  Dispose metoda.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable metode
    }
}
