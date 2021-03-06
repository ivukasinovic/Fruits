﻿// -----------------------------------------------------------------------
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
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Threading;

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
        ///	 Identifikatori OpenGL tekstura
        /// </summary>
        private uint[] m_textures = null;
        /// <summary>
        ///	 Putanje do slika koje se koriste za teksture
        /// </summary>
        private string[] m_textureFiles = { "..//..//images//brick.jpg", "..//..//images//parquet.jpg" };

        private float ambient0;
        private float ambient1;
        private float ambient2;

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        private float m_xRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        private float m_yRotation = 0.0f;

        private float m_zRotation = 90.0f;
        

        //private float ambientRG
        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        private float m_sceneDistance = 7000.0f;

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width;

        public float fruitHeight;
        public float leftFruitRotation;
        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height;

        public float Ambient0
        {
            get { return ambient0; }
            set { ambient0 = value; }
        }
        public float Ambient1
        {
            get { return ambient1; }
            set { ambient1 = value; }
        }
        public float Ambient2
        {
            get { return ambient2; }
            set { ambient2 = value; }
        }

        //animacija
        private float fruitRotation = 0.0f;
        public DispatcherTimer timer;

        #endregion Atributi

        #region Properties

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        /// 
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
            m_textures = new uint[2];

            ambient0 = 0.8f;
            ambient1 = 0.8f;
            ambient2 = 0.0f;

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

            SetupLighting0(gl);

            FormTexture(gl);

            m_scene.LoadScene();
            m_scene.Initialize();
            m_scene2.LoadScene();
            m_scene2.Initialize();
        }

        public void SetTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Tick += new EventHandler(RotateFruit);
            timer.Start();
        }

        private void RotateFruit(object sender, EventArgs e)
        {
            fruitRotation += 0.2f;
            if (fruitRotation > 10)
            {
                timer.Stop();
                fruitRotation -= 10;
            }
        }

        private void FormTexture(OpenGL gl)
        {
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            //kreira N tekstura odjednom (mipmap)
            gl.GenTextures(2, m_textures);
            for (int i = 0; i < 2; ++i)
            {
                // Pridruzi teksturu odgovarajucem identifikatoru
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[i]);

                // Ucitaj sliku i podesi parametre teksture
                Bitmap image = new Bitmap(m_textureFiles[i]);
                // rotiramo sliku zbog koordinantog sistema opengl-a
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                // RGBA format (dozvoljena providnost slike tj. alfa kanal)
                BitmapData imageData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                                      PixelFormat.Format32bppArgb);

                gl.Build2DMipmaps(OpenGL.GL_TEXTURE_2D, (int)OpenGL.GL_RGBA8, image.Width, image.Height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);
                //filtriranje teksture koje se primenjuje u slucaju da se tekstura mora smanjiti  da bi bila pridruzena obj je LINEAR
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);		// Linear Filtering
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);		// Linear Filtering

                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);

                image.UnlockBits(imageData);
                image.Dispose();
            }

        }

        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Viewport(0, 0, m_width, m_height);
            gl.ClearColor(0.2f, 0.2f, 0.2f, 1.0f);

            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            WriteText2(gl);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.Enable(OpenGL.GL_LIGHTING);
            
            gl.PushMatrix();
            
            gl.LookAt(0.0f, 0f, 100.0f, 0.0f, -1.0f, 0.0f, 0.0f, 1.0f, 0.0f);
            gl.Translate(0.0f, 0.0f, -m_sceneDistance);
          
            gl.Scale(10f, 10f, 10f);
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f); 
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);
            gl.Rotate(m_zRotation, 0.0f, 0.0f, 1.0f);
            
            gl.Translate(40f, 200f, 0.0f);
            //reflektor
             SetupLighting1(gl);


            //iscrtavanje 1/2jabuke
            //nacin stapanja teksture sa modelom ( sabira teksel sa bojom materijala)
            gl.Color(0.0f, 0.1f, 0.0f, 1.0f);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_ADD);
            gl.PushMatrix();
            gl.Rotate(0.0f + leftFruitRotation, 0.0f , 0.0f);
            gl.PushMatrix();
            gl.Translate(35, -80f, -40f + fruitHeight);
            gl.Rotate(90f - fruitRotation, 0.0f, 0.0f);
            gl.Scale(200f, 200f, 200f);
            m_scene.Draw();
            gl.PopMatrix();
           

            //iscrtavanje 2/2jabuke
            gl.PushMatrix();
            gl.Rotate(90f, 180f - fruitRotation, 0f);
            gl.Translate(50f, -40f + fruitHeight, -130f);
            gl.Scale(200f, 200f, 200f);
            m_scene.Draw();
            gl.PopMatrix();
            gl.PopMatrix();

            gl.PushMatrix();
           // gl.Rotate(0.0f, 0.0f + leftFruitRotation, 0.0f);
            //iscrtavanje narandza gore
            gl.PushMatrix();
            
            gl.Rotate(180f, 0f, 0f);
            gl.Translate(-125f, 137f,-50f - fruitHeight);
            gl.Scale(150f, 150f, 150f);
            m_scene2.Draw();
            gl.PopMatrix();
            //iscrtavanje narandza dole
            gl.PushMatrix();
            gl.Rotate(0f, 0f, 0f);
            gl.Translate(-125f, -258f, 12f + fruitHeight);
            gl.Scale(150f, 150f, 150f);
            m_scene2.Draw();
            gl.PopMatrix();
            gl.PopMatrix();
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            //postolja
            gl.PushMatrix();
            gl.Translate(-5.0f, -100.0f , 0.0f );
            gl.Color(0.0f, 0.7f, 0.0f);
            Cylinder cyl = new Cylinder
            {
                
                TopRadius = 20f,
                BaseRadius = 20f,
                Height = 20f+ fruitHeight
            };
            cyl.CreateInContext(gl);
            gl.Normal(0.0f, 1.0f, 0.0f);
            cyl.Render(gl, RenderMode.Render);
            Disk disk = new Disk();
            
            disk.InnerRadius = 0f;
            disk.OuterRadius = 20f;
            disk.CreateInContext(gl);
            gl.Normal(0.0f, 1.0f, 0.0f);
            gl.Translate(0.0f, 0.0f, 20f +fruitHeight);
            disk.Render(gl, RenderMode.Render);
            gl.Normal(0.0f, 1.0f, 0.0f);
            gl.Translate(-150f, -100f, -20f -fruitHeight);
            cyl.Render(gl, RenderMode.Render);
            gl.Translate(0.0f, 0.0f, 20f + fruitHeight );
            gl.Normal(0.0f, 1.0f, 0.0f);
            disk.Render(gl, RenderMode.Render);
            gl.PopMatrix();


            //iscrtavanje podloge

            drawFloor(gl);
           


            //gl.Color(0.0f, 0.0f, 0.7f);

            //dole zid
            gl.PushMatrix();
            Cube wall = new Cube();
            gl.Translate(-45.0f, 150.0f, 10.0f);
            gl.Scale(200f, 5f, 10f);
            gl.Normal(0f, 1f, 0f);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[0]);
            wall.Render(gl, RenderMode.Render);
            //skidanje tekstura
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

            //crtanje grid mreze
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

        private void drawFloor(OpenGL gl)
        {
            gl.Color(0.5f, 0.5f, 0.5f);
            gl.Translate(0.0f, -480f, -1f);

            //gl.Scale(2f,2f,2f);



            /*    for (float x = -300.0f; x < 100.0f; x += 250.0f)
            {*/

            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.PushMatrix();
            gl.Scale(4f, 4f, 4f);
            
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[1]);
            gl.Begin(OpenGL.GL_QUADS);
                gl.Normal(0.0f, 1.0f, 0.0f);
                gl.TexCoord(1.0f, 0.0f);
                gl.Vertex(200f , 100f);      

                gl.TexCoord(1.0f, 1.0f);
                gl.Vertex(200f, 480f);  

                gl.TexCoord(0.0f, 1.0f);
                gl.Vertex(-300f, 480f);

                gl.TexCoord(0.0f, 0.0f);
                gl.Vertex(-300f, 100f);

            gl.End();
            //}
            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
                
            }

        private void WriteText2(OpenGL gl)
        {
            gl.Viewport(0, m_width / 2, m_width / 2, m_height / 2);
            gl.PushMatrix();
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Ortho2D(-13.0f, 13.0f, -10.0f, 10.0f);

            gl.MatrixMode(OpenGL.GL_MODELVIEW);

            //poruka

            gl.PushMatrix();
            gl.Color(1.0f, 0f, 0.0f);
            gl.Translate(1.5f, -4.0f, 0.0f);
            gl.PushMatrix();
            gl.DrawText3D("Helvetica Bold", 14f, 0.0f, 0.0f, "Predmet: Racunarska grafika");
            gl.PopMatrix();
            gl.PushMatrix();
            gl.Translate(0f, -1.0f, 0.0f);
            gl.DrawText3D("Helvetica Bold", 14f, 0.0f, 0.0f, "Sk.god : 2017/18");
            gl.PopMatrix();
            gl.PushMatrix();
            gl.Translate(0f, -2.0f, 0.0f);
            gl.DrawText3D("Helvetica Bold", 14f, 0.0f, 0.0f, "Ime: Ivan");
            gl.PopMatrix();
            gl.PushMatrix();
            gl.Translate(0f, -3.0f, 0.0f);
            gl.DrawText3D("Helvetica Bold", 14f, 0.0f, 0.0f, "Prezime: Vukasinovic");
            gl.PopMatrix();
            gl.PushMatrix();
            gl.Translate(0f, -4.0f, 0.0f);
            gl.DrawText3D("Helvetica Bold", 14f, 0.0f, 0.0f, "Sifra zad: 15.2");
            gl.PopMatrix();


            gl.PopMatrix();

            gl.Viewport(0, 0, m_width, m_height);
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
           
            gl.Perspective(45f, 1.0f, 1.0f, 20000f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);

            gl.PopMatrix();
        }

        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(OpenGL gl, int width, int height)
        {

            
            gl.MatrixMode(OpenGL.GL_PROJECTION);      // selektuj Projection Matrix
            gl.LoadIdentity();
            gl.Perspective(45f, (double)width / height, 1f, 20000f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.Viewport(0, 0, m_width, m_height);
            gl.LoadIdentity();                // resetuj ModelView Matrix
        }
        public void SetupLighting0(OpenGL gl)
        {
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.ColorMaterial(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_AMBIENT_AND_DIFFUSE);
            
            
            float[] globalAmbient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            gl.Light(OpenGL.GL_LIGHT0,OpenGL.GL_AMBIENT, globalAmbient);

            float[] ac = new float[] { 0.7f, 0.7f, 0.0f, 1.0f };
            float[] dc = new float[] { 0.7f, 0.7f, 0.0f, 1.0f };
            float[] sc = new float[] { 0.2f, 0.2f, 0.0f, 1.0f };
            float[] position = { -300.0f, 300.0f, m_sceneDistance+100, 1.0f };
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, position);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, ac);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, dc);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, sc);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 180.0f);

            


            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);
            gl.Enable(OpenGL.GL_NORMALIZE);
           // gl.Rotate(0.0f, 90.0f, 0.0f);

        }
        public void SetupLighting1(OpenGL gl)
        {
            //zuto refleksiono
            float[] yellowAc = { ambient0, ambient1, ambient2, 1.0f };
            float[] reflectDiff = { 0.5f, 0.5f, 0.0f, 1.0f };
            float[] direction = { 0.0f, 0.0f, -1.0f };
            float[] light0specular_yellow = new float[] { 0.5f, 0.5f, 0.0f, 1.0f };

            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, yellowAc);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, reflectDiff);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPECULAR, light0specular_yellow);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_DIRECTION, direction);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_CUTOFF, 40.0f);
            float[] positionReflector = {-40.0f, -200.0f, 7000.0f, 1.0f };
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, positionReflector);
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT1);
            gl.Enable(OpenGL.GL_NORMALIZE);
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
