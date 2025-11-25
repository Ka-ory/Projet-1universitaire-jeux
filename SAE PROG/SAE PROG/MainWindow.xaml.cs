using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Shapes;
using System.Media;
using System.Reflection;
using System.Windows.Media;

namespace SAE_PROG
{
    public partial class MainWindow : Window
    {
        private MediaPlayer musique;
        private bool isLeftKeyDown = false;
        private bool isRightKeyDown = false;
        private bool isUpKeyDown = false;
        private bool isDownKeyDown = false;
        public double canvaHauteur = 1900;
        public double canvaLargeur = 900;
        public int tailleBanane = 100;
        private SoundPlayer sonPorte;
        private SoundPlayer sonAie;
        private SoundPlayer sonBoss;
        private SoundPlayer sonPas;
        private SoundPlayer sonPiou;
        static double PAS_BANANE = 20;
        static int PAS_OBJET = 20;
        private BitmapImage[] imgD;
        private BitmapImage[] imgG;
        private int Animation = 0;
        private bool droite = false;
        private bool gauche = false;
        private bool monter = false;
        private bool descendre = false;
        private bool block = false;
        private static DispatcherTimer minuterie;
        private static DateTime lastTick;
        int[,] map;
        private int direction;
        private Random random = new Random();
        public static int introo { get; set; }
        public int Nbimage = 0;
        public int ingredientfruit1 = 0;
        public int ingredientfruit = 0;
        private Image fruit1;
        private Image fruit;
        private Image boss;
        public int tailleBoss = 400;
        public int tailleObj = 50;
        public int tailleEcriture = 24;
        private int directionFeu;
        private int directionFruit;
        private DispatcherTimer changementDirectionTimerFeu;
        private DispatcherTimer changementDirectionTimerFruit;
        private DispatcherTimer mouvementTimerFeu;
        private DispatcherTimer mouvementTimerFruit;
        private DispatcherTimer updateTimer;
        private DispatcherTimer projectileTimer;
        private int nbProjectiles = 0;
        private bool toucher = false;
        public int VITESSE_PROJECTILE_BASE = 10;
        public static Image[] lesProjectiles;
        public int degatProjectiles = 10;
        public int tailleProjectiles = 35;
        public int degatsBalle = 10;
        public int tailleBalle = 50;
        public int hauteurBarre = 20;
        private int[] positionsProjectiles;
        private DispatcherTimer projectileSpawnTimer;
        private DispatcherTimer projectileMoveTimer;
        private bool[] projectilesActive;
        private DispatcherTimer bossAnimationStarterTimer;
        private DispatcherTimer animationTimer;
        private int frameActu = 0;
        private BitmapImage[] imgP;
        private DispatcherTimer mouvementTimerBanane;
        private DispatcherTimer bossMoveTimer;
        private int VieBoss = 0;
        private List<Image> BananeTire = new List<Image>();
        private int bananeVie = 100;
        private Rectangle bossBarreDeVie;
        private Rectangle bananeBarreDeVie;
        public static int Niveau { get; set; }
        private Label labelObjet;
        private int compteurObjet = 0;
        private Label labelBoss;
        private static SoundPlayer son;

        public MainWindow()
        {
            InitializeComponent();
            InitBitimage();
            InitIntroWindow();
            InitTimer();
            map = GenereMap(1);
            InitialiserTimersBanane();
            Level();
            InitSon();
            InitMusique("pack://application:,,,/sons/musiqueChill.mp3");
        }
        private void InitialiserTimersBanane()
        {
            mouvementTimerBanane = new DispatcherTimer();
            mouvementTimerBanane.Interval = TimeSpan.FromMilliseconds(10);
            mouvementTimerBanane.Tick += Deplacement;
            mouvementTimerBanane.Start();
        }

        private void InitIntroWindow()
        {
            IntroWindow intro = new IntroWindow();
            bool? result = intro.ShowDialog();
            if (introo == 1)
            {
                Window1 levell = new Window1();
                bool? result3 = levell.ShowDialog();
            }

            if (introo == 2)
            {
                SettingsWindow setting = new SettingsWindow();
                bool? resultt = setting.ShowDialog();
            }

        }

        private void Level()
        {
            if (Niveau == 1) 
            {
                nbProjectiles = 10;
                VieBoss = 100;
                bananeVie = 100;
            }
            else if (Niveau == 2)
            {
                nbProjectiles = 30;
                VieBoss = 250;
                bananeVie = 80;
            }
            else if (Niveau == 3)
            {
                nbProjectiles = 50;
                VieBoss = 500;
                bananeVie = 60;
            }
        }


        private void InitTimer()
        {
            minuterie = new DispatcherTimer();
            minuterie.Interval = TimeSpan.FromMilliseconds(30);
            minuterie.Tick += Deplacement;
            minuterie.Start();
        }

        private void InitSon()
        {
            sonPorte = new SoundPlayer(Application.GetResourceStream(new Uri("pack://application:,,,/sons/Porte.wav")).Stream);
            sonAie = new SoundPlayer(Application.GetResourceStream(new Uri("pack://application:,,,/sons/Aie.wav")).Stream);
            sonBoss = new SoundPlayer(Application.GetResourceStream(new Uri("pack://application:,,,/sons/Boss.wav")).Stream);
            sonPas = new SoundPlayer(Application.GetResourceStream(new Uri("pack://application:,,,/sons/BruitPas.wav")).Stream);
            sonPiou = new SoundPlayer(Application.GetResourceStream(new Uri("pack://application:,,,/sons/Piou.wav")).Stream);
        }

        private void InitMusique(string uriMusique)
        {
            musique = new MediaPlayer();
            musique.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + uriMusique));
            musique.MediaEnded += RelanceMusique;
            musique.Volume = 0.5;
            musique.Play();
        }
        private void RelanceMusique(object? sender, EventArgs e)
        {
            musique.Position = TimeSpan.Zero;
            musique.Play();
        }
        private void ChangerMusique()
        {
            InitMusique("pack://application:,,,/sons/musique boss.mp3");
        }

        public int[,] GenereMap(int mapNb)
        {
            //choix differentes maps 
            if (mapNb == 1)
            {
                return new int[,]
                {
            //1  2  3  4  5  6  7  8  9 10 11 12 13 14 15 16 17 18 19 20
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 3, 3, 1, 1, 1, 1, 1, 1, 1, 1,1},
            { 1, 1, 1, 1, 1, 1, 1, 1, 0, 3, 3, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0,1},
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0,1},
                };
            }
            else if (mapNb == 2)
            {
                return new int[,]
                {
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,1},
            { 1, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0,1},
            { 1, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0,1},
            { 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0,1},
            { 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0,1},
            { 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0,1},
            { 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0,1},
            { 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0,1},
            { 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1,1},
            { 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 5, 1, 1, 1,1},
            { 1, 1, 1, 6, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0,1},
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 1, 1, 1,1},
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 1, 1, 1,1},
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0,1},
                };
            }
            if (mapNb == 3)
            {
                return new int[,]
                {
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 3, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 3, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0,1},
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0,1},
                };
            }
            if (mapNb == 4)
            {
                return new int[,]
                {
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 3,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 3,1},
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0,1},
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0,1},
                };
            }
            else if (mapNb == 5)
            {
                return new int[,]
                {
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,1},
            { 1, 0, 0, 0, 1, 1, 1, 1, 7, 7, 1, 1, 0, 0, 1, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 1, 1, 1, 1, 7, 7, 1, 1, 0, 0, 1, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0,1},
            { 1, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0,1},
            { 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0,1},
            { 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0,1},
            { 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0,1},
            { 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0,1},
            { 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0,1},
            { 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1,1},
            { 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1,1},
            { 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0,1},
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 1, 1, 1,1},
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 1, 1, 1,1},
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0,1},
                };
            }
            else if (mapNb == 6)
            {
                return new int[,]
                {
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 8, 8, 8, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 8, 8, 8, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 8, 8, 8, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,1},
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,1},
            { 1, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0,1},
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0,1},
                };
            }
            //verif debug
            throw new ArgumentException("map inexistante.");
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Left) 
            {
                gauche = true;
                sonPas.Play();
                isLeftKeyDown = true;
            }
               
            if (e.Key == Key.Right)
            {
                sonPas.Play();
                isRightKeyDown = true;
                droite = true;
            }
                
            if (block == false)
            {
                if (e.Key == Key.Up)
                {
                    sonPas.Play();
                    isUpKeyDown = true;
                    monter = true;
                }
                   
                if (e.Key == Key.Down)
                {
                    sonPas.Play();
                    isDownKeyDown = true;
                    descendre = true;
                }
                    
            }
            else
                monter = false;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && block == true)
            {
                sonPiou.Play();
                balle();
            }

            if (e.Key == Key.Left) 
            {
                sonPas.Stop();
                isLeftKeyDown = false;
                gauche = false;
            }
                
            if (e.Key == Key.Right) 
            {
                sonPas.Stop();
                isRightKeyDown = false;
                droite = false;
            }
                

            if (block == false)
            {
                if (e.Key == Key.Up)
                {
                    sonPas.Stop();
                    isUpKeyDown = false;
                    monter = false;
                }
                    
                if (e.Key == Key.Down) 
                {
                    sonPas.Stop();
                    isDownKeyDown = false;
                    descendre = false;
                }
                    
            }
            else
                monter = false;
        }
        private void Deplacement(object? sender, EventArgs e)
        {
            //coordonnées du perso
            double x = Canvas.GetLeft(banane);
            double y = Canvas.GetTop(banane);

            double caseWidth = jeuCanvas.Width / 20;
            double caseHeight = jeuCanvas.Height / 20;


            int caseX = (int)(x / caseWidth);
            int caseY = (int)(y / caseHeight);


            bool peutSeDeplacer = false;

            //prise en charge collison
            if (gauche)
            {
                if (caseX > 0)
                {
                    peutSeDeplacer = map[caseY, caseX - 1] != 1;
                }
                if (peutSeDeplacer)
                {
                    sonPas.Play();
                    x -= PAS_BANANE;
                    Canvas.SetLeft(banane, x);
                    Animation++;
                    if (Animation >= imgG.Length) Animation = 0;
                    banane.Source = imgG[Animation];
                }
            }
            else if (droite)
            {
                if (caseX < map.GetLength(1) - 1)
                {
                    peutSeDeplacer = map[caseY, caseX + 1] != 1;
                }
                if (peutSeDeplacer)
                {
                    sonPas.Play();
                    x += PAS_BANANE;
                    Canvas.SetLeft(banane, x);
                    Animation++;
                    if (Animation >= imgD.Length) Animation = 0;
                    banane.Source = imgD[Animation];
                }
            }
            else if (monter)
            {
                if (caseY > 0)
                {
                    peutSeDeplacer = map[caseY - 1, caseX] != 1;
                }
                if (peutSeDeplacer)
                {
                    sonPas.Play();
                    y -= PAS_BANANE;
                    Canvas.SetTop(banane, y);
                    Animation++;
                    if (Animation >= imgD.Length) Animation = 0;
                    banane.Source = imgD[Animation];
                }
            }
            else if (descendre)
            {
                if (caseY < map.GetLength(0) - 1)
                {
                    peutSeDeplacer = map[caseY + 1, caseX] != 1;
                }
                if (peutSeDeplacer)
                {
                    y += PAS_BANANE;
                    Canvas.SetTop(banane, y);
                    Animation++;
                    if (Animation >= imgG.Length) Animation = 0;
                    banane.Source = imgG[Animation];
                }
            }



            // si on atteint la porte 
            if (caseY >= 0 && caseY < map.GetLength(0) && caseX >= 0 && caseX < map.GetLength(1))
            {
                mouvementTimerBanane.Stop();

                DispatcherTimer pauseTimer = new DispatcherTimer();
                pauseTimer.Interval = TimeSpan.FromSeconds(1);
                pauseTimer.Tick += (s, e) =>
                {
                    pauseTimer.Stop();
                    mouvementTimerBanane.Start();
                };

                pauseTimer.Start();

                //changement maps

                if (map[caseY, caseX] == 3)
                {

                    if (fruit1 != null && ingredientfruit1 == 0)
                    {
                        VerifierCollisions();
                    }

                    if (fruit1 != null)
                    {
                        jeuCanvas.Children.Remove(fruit1);
                        mouvementTimerFeu.Stop();
                        changementDirectionTimerFeu.Stop();
                        updateTimer.Stop();
                        fruit1 = null;
                    }

                    if (fruit != null && ingredientfruit == 0)
                    {
                        VerifierCollisions();
                    }

                    if (fruit != null)
                    {
                        jeuCanvas.Children.Remove(fruit);
                        mouvementTimerFruit.Stop();
                        changementDirectionTimerFruit.Stop();
                        updateTimer.Stop();
                    }

                    ChangerCarte();
                    if (ingredientfruit1 < 1 || ingredientfruit < 1)
                    {
                        map = GenereMap(2);
                        Nbimage = 2;
                    }
                    if (ingredientfruit1 == 1 && ingredientfruit == 1)
                    {
                        map = GenereMap(5);
                        Nbimage = 5;
                    }

                    if (labelObjet == null)
                    {
                        labelObjet = new Label
                        {
                            Content = "Objet = 0",
                            FontSize = tailleEcriture,
                            Foreground = new SolidColorBrush(Colors.White),
                            Background = new SolidColorBrush(Colors.Black)
                        };
                        Canvas.SetLeft(labelObjet, 10);
                        Canvas.SetTop(labelObjet, 10);
                        jeuCanvas.Children.Add(labelObjet);
                    }
                    if (map[caseY, caseX] == 4)
                    {

                        ChangerCarte();
                        map = GenereMap(1);
                        Nbimage = 1;

                    }
                }
                if (map[caseY, caseX] == 5)
                {
                    ChangerCarte();
                    map = GenereMap(3);
                    Nbimage = 3;
                    EntrerDansCarte3();


                }
                if (map[caseY, caseX] == 6)
                {

                    ChangerCarte();
                    map = GenereMap(4);
                    Nbimage = 4;
                    EntrerDansCarte4();

                }
                if (map[caseY, caseX] == 7)
                {
                    ChangerMusique();
                    if (labelObjet != null)
                    {
                        jeuCanvas.Children.Remove(labelObjet);

                    }

                    if (labelBoss != null)
                    {
                        jeuCanvas.Children.Remove(labelBoss);

                    }
                    ChangerCarte();
                    map = GenereMap(6);
                    Nbimage = 6;
                    InitJeu();
                    block = true;
                }
            }
        }

        private void EntrerDansCarte3()
        {
            //initialisation de l'objet
            fruit1 = new Image
            {
                Width = tailleObj,
                Height = tailleObj,
                Source = new BitmapImage(new Uri("pack://application:,,,/img/Fruit1.png"))
            };

            int X = random.Next(0, (int)jeuCanvas.ActualWidth - 50);
            int Y = random.Next(0, (int)jeuCanvas.ActualHeight - 50);

            Canvas.SetLeft(fruit1, X);
            Canvas.SetTop(fruit1, Y);

            if (ingredientfruit1 == 0)
            {
                jeuCanvas.Children.Add(fruit1);
                InitTimersFruit1();
                InitTimerMaj();

            }


        }

        private void EntrerDansCarte4()
        {
            //initialisation de l'objet
            fruit = new Image
            {
                Width = tailleObj,
                Height = tailleObj,
                Source = new BitmapImage(new Uri("pack://application:,,,/img/Fruit2.png"))
            };

            int X = random.Next(0, (int)jeuCanvas.ActualWidth - 50);
            int Y = random.Next(0, (int)jeuCanvas.ActualHeight - 50);

            Canvas.SetLeft(fruit, X);
            Canvas.SetTop(fruit, Y);

            if (ingredientfruit == 0)
            {
                jeuCanvas.Children.Add(fruit);
                InitTimersFruit2();
                InitTimerMaj();
            }



        }
        private void InitMouvementFruit()
        {
            directionFeu = random.Next(0, 4);
        }

        private void InitMouvementFruit2()
        {
            directionFruit = random.Next(0, 4);
        }

        private void InitTimersFruit1()
        {
            mouvementTimerFeu = new DispatcherTimer();
            mouvementTimerFeu.Interval = TimeSpan.FromMilliseconds(1);
            mouvementTimerFeu.Tick += DeplacementFruit1;
            mouvementTimerFeu.Start();


            changementDirectionTimerFeu = new DispatcherTimer();
            changementDirectionTimerFeu.Interval = TimeSpan.FromMilliseconds(400);
            changementDirectionTimerFeu.Tick += (s, e) => InitMouvementFruit();
            changementDirectionTimerFeu.Start();
        }

        private void InitTimersFruit2()
        {
            mouvementTimerFruit = new DispatcherTimer();
            mouvementTimerFruit.Interval = TimeSpan.FromMilliseconds(1);
            mouvementTimerFruit.Tick += DeplacementFruit2;
            mouvementTimerFruit.Start();


            changementDirectionTimerFruit = new DispatcherTimer();
            changementDirectionTimerFruit.Interval = TimeSpan.FromMilliseconds(400);
            changementDirectionTimerFruit.Tick += (s, e) => InitMouvementFruit2();
            changementDirectionTimerFruit.Start();
        }

        public void InitTimerMaj()
        {

            updateTimer = new DispatcherTimer();
            updateTimer.Interval = TimeSpan.FromMilliseconds(16);
            updateTimer.Tick += (sender, e) => Actu();
            updateTimer.Start();
        }

        private void DeplacementFruit1(object sender, EventArgs e)
        {
            if (fruit1 != null)
            {
                int x = (int)Canvas.GetLeft(fruit1);
                int y = (int)Canvas.GetTop(fruit1);

                switch (directionFeu)
                {
                    case 0:
                        y -= PAS_OBJET;
                        break; // Haut
                    case 1:
                        y += PAS_OBJET;
                        break; // Bas
                    case 2:
                        x -= PAS_OBJET;
                        break; // Gauche
                    case 3:
                        x += PAS_OBJET;
                        break; // Droite
                }

                if (CollisionAvecBords(x, y, fruit1))
                {
                    switch (directionFeu)
                    {
                        case 0:
                            directionFeu = 1;
                            break;
                        case 1:
                            directionFeu = 0;
                            break;
                        case 2:
                            directionFeu = 3;
                            break;
                        case 3:
                            directionFeu = 2;
                            break;
                    }
                }
                else
                {
                    Canvas.SetLeft(fruit1, x);
                    Canvas.SetTop(fruit1, y);
                }
            }
        }

        private void DeplacementFruit2(object sender, EventArgs e)
        {
            if (fruit != null)
            {
                int x = (int)Canvas.GetLeft(fruit);
                int y = (int)Canvas.GetTop(fruit);

                switch (directionFruit)
                {
                    case 0:
                        y -= PAS_OBJET;
                        break; // Haut
                    case 1:
                        y += PAS_OBJET;
                        break; // Bas
                    case 2:
                        x -= PAS_OBJET;
                        break; // Gauche
                    case 3:
                        x += PAS_OBJET;
                        break; // Droite
                }

                if (CollisionAvecBords(x, y, fruit))
                {
                    switch (directionFruit)
                    {
                        case 0:
                            directionFruit = 1;
                            break;
                        case 1:
                            directionFruit = 0;
                            break;
                        case 2:
                            directionFruit = 3;
                            break;
                        case 3:
                            directionFruit = 2;
                            break;
                    }
                }
                else
                {
                    Canvas.SetLeft(fruit, x);
                    Canvas.SetTop(fruit, y);
                }
            }
        }

        private bool CollisionAvecBords(int x, int y, Image objet)
        {
            double canvasLargeur = jeuCanvas.ActualWidth;
            double canvasHauteur = jeuCanvas.ActualHeight;

            double objetLargeur = objet.Width;
            double objetHauteur = objet.Height;

            if (x < 0 || x + objetLargeur > canvasLargeur || y < 20 || y + objetHauteur + 5 > canvasHauteur)
            {
                return true;
            }

            return false;
        }



        private void Actu()
        {
            VerifierCollisions();
        }

        private void VerifierCollisions()
        {
            if (fruit1 != null && DetecterCollision(banane, fruit1))
            {
                jeuCanvas.Children.Remove(fruit1);
                mouvementTimerFeu.Stop();
                changementDirectionTimerFeu.Stop();
                updateTimer.Stop();
                ingredientfruit1++;
                compteurObjet++;
                ActuLabelObjet();
                //MessageBox.Show("tu as récuperer l'objet de feu, BIEN JOUER");
            }


            if (fruit != null && DetecterCollision(banane, fruit))
            {
                jeuCanvas.Children.Remove(fruit);
                mouvementTimerFruit.Stop();
                changementDirectionTimerFruit.Stop();
                updateTimer.Stop();
                ingredientfruit++;
                compteurObjet++;
                ActuLabelObjet();
                //MessageBox.Show("tu as récuperer l'objet couronné, BIEN JOUER");
            }
        }
        private void ActuLabelObjet()
        {
            if (compteurObjet < 2)
            {
                labelObjet.Content = $"Objet = {compteurObjet}";
            }
            else
            {
                // Supprimer le label "Objet"
                jeuCanvas.Children.Remove(labelObjet);
                labelObjet = null;

                // Ajouter le label "Aller combattre le boss"
                if (labelBoss == null)
                {
                    labelBoss = new Label
                    {
                        Content = "Aller combattre le boss",
                        FontSize = tailleEcriture,
                        Foreground = new SolidColorBrush(Colors.Red),
                        Background = new SolidColorBrush(Colors.Black)
                    };
                    Canvas.SetLeft(labelBoss, 10);
                    Canvas.SetTop(labelBoss, 50);
                    jeuCanvas.Children.Add(labelBoss);
                }
            }
        }

        private bool DetecterCollision(Image image, Image autreimage)
        {
            bool test = false;
            double x1 = Canvas.GetLeft(image);
            double y1 = Canvas.GetTop(image);
            double w1 = image.ActualWidth;
            double h1 = image.ActualHeight;


            double x2 = Canvas.GetLeft(autreimage);
            double y2 = Canvas.GetTop(autreimage);
            double w2 = autreimage.ActualWidth;
            double h2 = autreimage.ActualHeight;
            if (x1 < x2 + w2 && x1 + w1 > x2 && y1 < y2 + h2 && y1 + h1 > y2)
                test = true;
            return test;
        }





        private void ChangerCarte()
        {
            bool transition = true;

            //lance une animation et initialise la map
                DoubleAnimation fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
                fadeOut.Completed += (s, e) =>
                {
                    sonPorte.Play(); // Joue un son d'ouverture de porte

                if (Nbimage == 2)
                {
                    ImageBrush newBackground = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/img/Map2.PNG")));
                    jeuCanvas.Background = newBackground;
                    Canvas.SetLeft(banane, 960);
                    Canvas.SetTop(banane, 710);
                }
                if (Nbimage == 1)
                {
                    ImageBrush newBackground = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/img/map1.png")));
                    jeuCanvas.Background = newBackground;
                    Canvas.SetLeft(banane, 900);
                    Canvas.SetTop(banane, 200);
                }
                if (Nbimage == 3)
                {
                    ImageBrush newBackground = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/img/mapvide2.PNG")));
                    jeuCanvas.Background = newBackground;
                    Canvas.SetLeft(banane, 150);
                    Canvas.SetTop(banane, 650);
                }
                if (Nbimage == 4)
                {
                    ImageBrush newBackground = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/img/mapvide1.png")));
                    jeuCanvas.Background = newBackground;
                    Canvas.SetLeft(banane, 1300);
                    Canvas.SetTop(banane, 710);
                }
                if (Nbimage == 5)
                {
                    ImageBrush newBackground = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/img/Map3.png")));
                    jeuCanvas.Background = newBackground;
                    Canvas.SetLeft(banane, 960);
                    Canvas.SetTop(banane, 710);
                }
                if (Nbimage == 6)
                {
                    ImageBrush nouveauFond = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/img/chateau.png")));
                    jeuCanvas.Background = nouveauFond;
                    Canvas.SetLeft(banane, 960);
                    Canvas.SetTop(banane, 710);
                }



                DoubleAnimation fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1));
                jeuCanvas.BeginAnimation(OpacityProperty, fadeIn);
                transition = false;
            };

            jeuCanvas.BeginAnimation(OpacityProperty, fadeOut);
        }
        private void InitJeu()
        {
            //animation pour le boss
            imgP = new BitmapImage[4];
            for (int i = 0; i < imgP.Length; i++)
            {
                imgP[i] = new BitmapImage(new Uri($"pack://application:,,,/potiron/potiron000{i + 1}.png"))
                {
                    CreateOptions = BitmapCreateOptions.IgnoreColorProfile
                };
            }

            //initialisation du boss et des projectiles

            boss = new Image
            {
                Width = tailleBoss,
                Height = tailleBoss,
                Source = imgP[0]
            };

            Canvas.SetLeft(boss, 700);
            Canvas.SetTop(boss, 50);
            jeuCanvas.Children.Add(boss);

            bossAnimationStarterTimer = new DispatcherTimer();
            bossAnimationStarterTimer.Interval = TimeSpan.FromSeconds(2);
            bossAnimationStarterTimer.Tick += DebutBossAnimation;
            bossAnimationStarterTimer.Start();

            lesProjectiles = new Image[nbProjectiles];
            positionsProjectiles = new int[nbProjectiles];
            projectilesActive = new bool[nbProjectiles];

            projectileSpawnTimer = new DispatcherTimer();
            projectileSpawnTimer.Interval = TimeSpan.FromMilliseconds(50);
            projectileSpawnTimer.Tick += SpawnProjectile;
            projectileSpawnTimer.Start();

            projectileMoveTimer = new DispatcherTimer();
            projectileMoveTimer.Interval = TimeSpan.FromMilliseconds(16);
            projectileMoveTimer.Tick += BougerProjectiles;
            projectileMoveTimer.Start();

            bossMoveTimer = new DispatcherTimer();
            bossMoveTimer.Interval = TimeSpan.FromMilliseconds(100);
            bossMoveTimer.Tick += BougerBoss;
            bossMoveTimer.Start();

            //barre de vie du boss et du perso

            BarreDeVie(VieBoss, 300, 20, "Banane");
            BarreDeVie(bananeVie, 20, 20, "Boss");
        }

        private void VieBanane()
        {
            if (DetecterCollision(banane, boss))
            {
                bananeVie -= degatProjectiles;
                sonAie.Play();
                ActuBarreDeVie(bananeVie, "Banane");

                if (bananeVie <= 0)
                {
                    MessageBox.Show("La banane a perdu !");
                    RelancerJeu();
                }
            }
        }

        private void BarreDeVie(int health, double x, double y, string type)
        {

            Rectangle healthBar = null;

            if (type == "Banane")
            {

                healthBar = new Rectangle
                {
                    Width = health * 2,
                    Height = hauteurBarre,
                    Fill = new SolidColorBrush(Color.FromRgb(255, 255, 0))
                };
            }
            else if (type == "Boss")
            {
                // Barre orange pour le boss
                healthBar = new Rectangle
                {
                    Width = health * 2,
                    Height = hauteurBarre,
                    Fill = new SolidColorBrush(Color.FromRgb(255, 165, 0))
                };
            }

            Canvas.SetLeft(healthBar, x);
            Canvas.SetTop(healthBar, y);
            jeuCanvas.Children.Add(healthBar);


            if (type == "Banane")
                bananeBarreDeVie = healthBar;
            else if (type == "Boss")
                bossBarreDeVie = healthBar;
        }

        private void ActuBarreDeVie(int health, string type)
        {
            Rectangle healthBar = null;

            if (type == "Banane")
                healthBar = bananeBarreDeVie;
            else if (type == "Boss")
                healthBar = bossBarreDeVie;


            if (healthBar != null)
            {
                healthBar.Width = Math.Max(0, health * 2);

                if (health <= 0)
                {
                    jeuCanvas.Children.Remove(healthBar);
                }
            }
        }

        private void BougerBoss(object sender, EventArgs e)
        {
            double bossX = Canvas.GetLeft(boss);
            double bossY = Canvas.GetTop(boss);

            int directionHorizontale = random.Next(0, 2) == 0 ? -1 : 1;

            double step = random.Next(30, 50) * directionHorizontale;

            if (bossX + step < 0)
            {
                step = -bossX;
            }
            else if (bossX + step > jeuCanvas.Width - boss.Width)
            {
                step = jeuCanvas.Width - boss.Width - bossX;
            }

            bossX += step;

            Canvas.SetLeft(boss, bossX);
        }

        private void balle()
        {
            Image balle = new Image
            {
                Width = tailleBalle,
                Height = tailleBalle,
                Source = new BitmapImage(new Uri("pack://application:,,,/img/balle.png")),
                Tag = "BalleBanane"
            };

            double balleX = Canvas.GetLeft(banane) + (banane.Width / 2) - (balle.Width / 2);
            double balleY = Canvas.GetTop(banane);

            Canvas.SetLeft(balle, balleX);
            Canvas.SetTop(balle, balleY);
            jeuCanvas.Children.Add(balle);

            DispatcherTimer balleTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(20)
            };

            balleTimer.Tick += (s, e) =>
            {

                balleY -= VITESSE_PROJECTILE_BASE;
                Canvas.SetTop(balle, balleY);

                if (balleY < 0)
                {
                    jeuCanvas.Children.Remove(balle);
                    balleTimer.Stop();
                    return;
                }

                for (int i = 0; i < nbProjectiles; i++)
                {
                    if (projectilesActive[i] && DetecterCollision(balle, lesProjectiles[i]))
                    {
                        jeuCanvas.Children.Remove(balle);
                        jeuCanvas.Children.Remove(lesProjectiles[i]);
                        projectilesActive[i] = false;
                        balleTimer.Stop();
                        return;
                    }
                }

                if (DetecterCollision(balle, boss) && balle.Tag?.ToString() == "BalleBanane")
                {
                    jeuCanvas.Children.Remove(balle);
                    balleTimer.Stop();

                    VieBoss -= degatsBalle;
                    ActuBarreDeVie(VieBoss, "Boss"); // Mettre à jour la barre de vie du boss

                    if (VieBoss <= 0)
                    {
                        sonBoss.Play();
                        MessageBox.Show("Félicitations, vous avez vaincu le boss et terminer le jeu !");
                        finWindow fin = new finWindow();
                        bool? result = fin.ShowDialog();
                        this.Close();
                    }
                }
            };

            balleTimer.Start();
        }


        private void DebutBossAnimation(object sender, EventArgs e)
        {
            if (animationTimer != null)
            {
                animationTimer.Stop();
            }
            animationTimer = new DispatcherTimer();
            animationTimer.Interval = TimeSpan.FromMilliseconds(500);
            animationTimer.Tick += AnimationBoss;
            animationTimer.Start();
        }

        private void AnimationBoss(object sender, EventArgs e)
        {

            imgP[imgP.Length - 1] = imgP[0];


            for (int i = 0; i < imgP.Length - 1; i++)
            {
                imgP[i] = imgP[i + 1];
            }

            boss.Source = imgP[0];
        }


        private void SpawnProjectile(object sender, EventArgs e)
        {
            bool projectileSpawned = false;

            for (int i = 0; i < nbProjectiles; i++)
            {
                if (!projectilesActive[i] && !projectileSpawned)
                {

                    lesProjectiles[i] = new Image
                    {
                        Width = tailleProjectiles,
                        Height = tailleProjectiles,
                        Source = new BitmapImage(new Uri("pack://application:,,,/img/projectilepotiron.png"))
                    };
                    jeuCanvas.Children.Add(lesProjectiles[i]);


                    positionsProjectiles[i] = random.Next(100, (int)jeuCanvas.Width - 100);
                    Canvas.SetLeft(lesProjectiles[i], positionsProjectiles[i]);
                    Canvas.SetTop(lesProjectiles[i], 0);
                    projectilesActive[i] = true;

                    projectileSpawned = true;
                }
            }
        }


        private void BougerProjectiles(object sender, EventArgs e)
        {
            for (int i = 0; i < nbProjectiles; i++)
            {
                if (!projectilesActive[i])
                    continue;

                double newPosition = Canvas.GetTop(lesProjectiles[i]) + VITESSE_PROJECTILE_BASE;
                Canvas.SetTop(lesProjectiles[i], newPosition);

                if (DetecterCollision(lesProjectiles[i], banane))
                {
                    jeuCanvas.Children.Remove(lesProjectiles[i]);
                    projectilesActive[i] = false;

                    // Réduire la vie de la banane
                    bananeVie -= degatProjectiles;
                    ActuBarreDeVie(bananeVie, "Banane");

                    if (bananeVie <= 0)
                    {
                        MessageBox.Show("Game Over! Vous avez perdu.");
                        RelancerJeu();
                    }
                    continue;
                }

                if (newPosition > jeuCanvas.Height)
                {
                    jeuCanvas.Children.Remove(lesProjectiles[i]);
                    projectilesActive[i] = false;
                }
            }
        }

        private void RelancerJeu()
        {
            projectileSpawnTimer.Stop();
            projectileMoveTimer.Stop();
            bossMoveTimer.Stop();

            bananeVie = bananeVie;
            VieBoss = VieBoss;
            jeuCanvas.Children.Clear();

            Canvas.SetLeft(banane, 900);
            Canvas.SetTop(banane, 750);

            InitJeu();
            jeuCanvas.Children.Add(banane);
        }

        private void InitBitimage()
        {
            
            imgD = new BitmapImage[5];
            for (int i = 0; i < imgD.Length; i++)
            {
                imgD[i] = new BitmapImage(new Uri($"pack://application:,,,/anime_banane/banane0{i + 1}.png"))
                {
                    CreateOptions = BitmapCreateOptions.IgnoreColorProfile
                };
            }

            imgG = new BitmapImage[5];
            for (int i = 0; i < imgG.Length; i++)
            {
                imgG[i] = new BitmapImage(new Uri($"pack://application:,,,/anime_banane/bananeG0{i + 1}.png"))
                {
                    CreateOptions = BitmapCreateOptions.IgnoreColorProfile
                };
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double X = ActualWidth / canvaHauteur;
            double Y = ActualHeight / canvaLargeur;

            PAS_BANANE *= Y;

            jeuCanvas.Width = canvaHauteur * X;
            jeuCanvas.Height = canvaLargeur * Y;
            Canvas.SetLeft(jeuCanvas, 0);
            Canvas.SetTop(jeuCanvas, 0);

            banane.Width = tailleBanane * X;
            banane.Height = tailleBanane * Y;

            double XX = Canvas.GetLeft(banane);
            double YY = Canvas.GetTop(banane);

            double relX = XX;
            double relY = YY;

            if (relX < 0)
                relX = 0;
            else if (relX > jeuCanvas.Width - banane.Width)
                relX = jeuCanvas.Width - banane.Width;

            if (relY < 0)
                relY = 0;
            else if (relY > jeuCanvas.Height - banane.Height)
                relY = jeuCanvas.Height - banane.Height;

            Canvas.SetLeft(banane, relX);
            Canvas.SetTop(banane, relY);
        }

    }
}
