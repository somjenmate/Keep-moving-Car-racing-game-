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
using System.Windows.Threading;
using System.IO;

namespace Car_Racing_Game
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        Random r = new Random();
        ImageBrush playerimg = new ImageBrush();
        ImageBrush carimg1 = new ImageBrush();
        ImageBrush carimg2 = new ImageBrush();
        ImageBrush carimg3 = new ImageBrush();
        ImageBrush carimg4 = new ImageBrush();
        Rect playerHitBox;
        int speed, barnum;
        double score;
        bool moveLeft, moveRight, gameOver;
        MediaPlayer soundplayer=new MediaPlayer();
        public MainWindow()
        {
            InitializeComponent();
            soundplayer.Open(new Uri("../../sound/island.mp3",UriKind.RelativeOrAbsolute));
            soundplayer.Play();
            soundplayer.MediaEnded += new EventHandler(Sound_end);
            Opentop();
        }
        private void Sound_end(object sender, EventArgs e)
        {
            soundplayer.Position = TimeSpan.Zero;
            soundplayer.Play();
        }

        private void GameLoop(object sender, EventArgs e)
        {
            score += .05;
            time.Content = "Survived: "+ score.ToString("#.#")+" seconds";
            speedlabel.Content = "Speed: " + speed;
            playerHitBox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
            
            if(moveLeft==true&&Canvas.GetLeft(player)>0)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) - 15);
            }
            
            if (moveRight == true && Canvas.GetLeft(player)+90<Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) + 15);
            }

            foreach (var x in road.Children.OfType<Rectangle>())
            {
                if((string)x.Tag=="line")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + speed);
                    if(Canvas.GetTop(x)>510)
                    {
                        Canvas.SetTop(x, -152);
                    }
                }

                if((string)x.Tag=="barrier")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + speed);
                    if(Canvas.GetTop(x)>500)
                    {
                        Canvas.SetTop(x, (r.Next(100, 400) * -1));
                        Canvas.SetLeft(x, r.Next(0, 390));
                        Changebarrier(x);
                    }

                    Rect barrierHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    if(playerHitBox.IntersectsWith(barrierHitBox)&&x.Visibility!=Visibility.Hidden)
                    {
                        timer.Stop();
                        endlabel.Visibility = Visibility.Visible;
                        endlabel.Content = "Good job " + playername.Text +" !";
                        endlabel2.Visibility = Visibility.Visible;
                        gameOver = true;
                        Savetop();
                    }
                }
            }
            if(score>=10 &&score<20)
            {
                speed = 12;
            }
            if(score>=20&&score<30)
            {
                speed = 14;
            }
            if (score >= 30 && score < 40)
            {
                speed = 16;
            }
            if (score >= 40 && score < 50)
            {
                speed = 18;
            }
            if (score >= 50 && score < 80)
            {
                speed = 20;
            }

        }

        private void Playbutton(object sender, RoutedEventArgs e)
        {
            bool check= false,check2=false;

            foreach (var x in road.Children.OfType<RadioButton>())
            {
                if(x.IsChecked==true&&(string)x.Content=="Beginner")
                {
                    check = true;
                    int j = 0;
                    foreach (var y in road.Children.OfType<Rectangle>())
                    {
                        if ((string)y.Tag == "barrier")
                        {
                            j++;
                            if (j >= 3)
                            {
                                y.Visibility = Visibility.Hidden;
                            }
                        }
                    }
                }
                if (x.IsChecked == true && (string)x.Content == "Intermediate")
                {
                    check = true;
                    int j = 0;
                    foreach (var y in road.Children.OfType<Rectangle>())
                    {
                        if ((string)y.Tag == "barrier")
                        {
                            j++;
                            if (j == 4)
                            {
                                y.Visibility = Visibility.Hidden;
                            }
                        }
                    }
                }
                if (x.IsChecked == true && (string)x.Content == "Advanced")
                {
                    check = true;
                    foreach (var y in road.Children.OfType<Rectangle>())
                    {
                        if ((string)y.Tag == "barrier")
                        {
                            y.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
            if((carw.IsChecked==true||carr.IsChecked == true ||carg.IsChecked == true || cargr.IsChecked == true)&&playername.Text!="")
            {
                check2 = true;
            }
            if(check==true&&check2==true)
            {
                startscreen.Visibility = Visibility.Hidden;
                sound.Visibility = Visibility.Hidden;
                topplayertext.Visibility = Visibility.Hidden;
                topplayer.Visibility = Visibility.Hidden;
                cartext.Visibility = Visibility.Hidden;
                playernametext.Visibility = Visibility.Hidden;
                playername.Visibility = Visibility.Hidden;
                begb.Visibility = Visibility.Hidden;
                interb.Visibility = Visibility.Hidden;
                advb.Visibility = Visibility.Hidden;
                carselect.Visibility = Visibility.Hidden;
                road.Focus();
                timer.Tick += GameLoop;
                timer.Interval = TimeSpan.FromMilliseconds(20);
                Start();
            }
            else
            {
                MessageBox.Show("You have to choose a level and a car! You also have to give yourself a player name!");
            }
            
        }

        private void Playsound(object sender, RoutedEventArgs e)
        {
            if(sound.Content.ToString()=="Sound Off")
            {
                sound.Content="Sound On";
                soundplayer.Pause();
            }
            else
            {
                if(sound.Content.ToString()=="Sound On")
                {
                    sound.Content="Sound Off";
                    soundplayer.Play();
                }  
            }  
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Left||e.Key==Key.A)
            {
                moveLeft = true;
            }
            if(e.Key==Key.Right||e.Key==Key.D)
            {
                moveRight = true;
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left || e.Key == Key.A)
            {
                moveLeft = false;
            }
            if (e.Key == Key.Right || e.Key == Key.D)
            {
                moveRight = false;
            }

            if(e.Key==Key.Enter&&gameOver==true)
            {
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
        }

        private void Showcars(object sender, RoutedEventArgs e)
        {
            carimg1.ImageSource = new BitmapImage(new Uri("pack://application:,,,/img/auto1.png"));
            car1.Fill = carimg1;
            carimg2.ImageSource = new BitmapImage(new Uri("pack://application:,,,/img/auto2.png"));
            car2.Fill = carimg2;
            carimg3.ImageSource = new BitmapImage(new Uri("pack://application:,,,/img/auto3.png"));
            car3.Fill = carimg3;
            carimg4.ImageSource = new BitmapImage(new Uri("pack://application:,,,/img/burger.png"));
            car4.Fill = carimg4;
            cars.Visibility = Visibility.Visible;
            car1.Visibility = Visibility.Visible;
            car2.Visibility = Visibility.Visible;
            car3.Visibility = Visibility.Visible;
            car4.Visibility = Visibility.Visible;
            carw.Visibility = Visibility.Visible;
            carr.Visibility = Visibility.Visible;
            carg.Visibility = Visibility.Visible;
            cargr.Visibility = Visibility.Visible;
            closecar.Visibility = Visibility.Visible;
        }

        public void Closecars(object sender, RoutedEventArgs e)
        {
            cars.Visibility = Visibility.Hidden;
            car1.Visibility = Visibility.Hidden;
            car2.Visibility = Visibility.Hidden;
            car3.Visibility = Visibility.Hidden;
            car4.Visibility = Visibility.Hidden;
            carw.Visibility = Visibility.Hidden;
            carr.Visibility = Visibility.Hidden;
            carg.Visibility = Visibility.Hidden;
            cargr.Visibility = Visibility.Hidden;
            closecar.Visibility = Visibility.Hidden;
        }

        private void Start()
        {
            speed = 10;
            timer.Start();
            moveLeft = false;
            moveRight = false;
            gameOver = false;
            score=0;
            time.Content = "Survived: 0 seconds";
            if(carw.IsChecked==true)
            {
                playerimg.ImageSource = new BitmapImage(new Uri("pack://application:,,,/img/auto1.png"));
            }
            if(carr.IsChecked == true)
            {
                playerimg.ImageSource = new BitmapImage(new Uri("pack://application:,,,/img/auto2.png"));
            }
            if (carg.IsChecked == true)
            {
                playerimg.ImageSource = new BitmapImage(new Uri("pack://application:,,,/img/auto3.png"));
            }
            if (cargr.IsChecked == true)
            {
                playerimg.ImageSource = new BitmapImage(new Uri("pack://application:,,,/img/burger.png"));
            }

            player.Fill = playerimg;
            road.Background = Brushes.DarkGray;
            foreach(var x in road.Children.OfType<Rectangle>())
            {
                if((string)x.Tag=="barrier")
                {
                    Rect beforeActualHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    Canvas.SetTop(x, (r.Next(100, 400) * -1));
                    Canvas.SetLeft(x, (r.Next(0, 390)));
                    Rect actualHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    while(beforeActualHitBox.IntersectsWith(actualHitBox))
                    {
                        Canvas.SetTop(x, (r.Next(100, 400) * -1));
                        Canvas.SetLeft(x, (r.Next(0, 390)));
                        actualHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    }
                    Changebarrier(x);
                }
            }
        }

        private void Changebarrier(Rectangle barrier)
        {
            barnum = r.Next(1, 4);
            ImageBrush barrierimg = new ImageBrush();
            switch(barnum)
            {
                case 1:
                    {
                        barrierimg.ImageSource = new BitmapImage(new Uri("pack://application:,,,/img/boja.png"));
                        break;
                    }
                case 2:
                    {
                        barrierimg.ImageSource = new BitmapImage(new Uri("pack://application:,,,/img/faronk.png"));
                        break;
                    }
                case 3:
                    {
                        barrierimg.ImageSource = new BitmapImage(new Uri("pack://application:,,,/img/sorompo.png"));
                        break;
                    }
            }
            barrier.Fill = barrierimg;

        }

        private void Savetop()
        {
            FileStream f=new FileStream("top.txt", FileMode.Open);
            StreamReader sr=new StreamReader(f);
            string[]t=sr.ReadLine().Split(';');
            sr.Close();
            f.Close();
            if(Convert.ToDouble(t[1])<score)
            {
                FileStream f2=new FileStream("top.txt",FileMode.Create);
                StreamWriter sw=new StreamWriter(f2);
                sw.WriteLine(playername.Text+";"+Math.Round(score,1));
                sw.Flush();
                sw.Close();
                f2.Close();
            }
        }

        private void Opentop()
        {
            FileStream f=new FileStream("top.txt", FileMode.Open);
            StreamReader sr=new StreamReader(f); 
            string[]t=sr.ReadLine().Split(';');
            topplayer.Content=t[0]+"-"+t[1]+" s";
            sr.Close();
            f.Close();
        }
    }
}
