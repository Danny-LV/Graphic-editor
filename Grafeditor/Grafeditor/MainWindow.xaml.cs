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
using System.Windows.Controls.Primitives;
using Microsoft.Win32;
using System.IO;


namespace Grafeditor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Random rnd = new Random();
        enum Draw_state
        {
            Pencil,
            Circle,
            Rectangle,
            Line,
            No_state
        }
        Draw_state state = Draw_state.No_state;
        double mousex, mousey;// текущие координаты мыши
        double omousex, omousey;// старые координаты мыши
        Color Drawcolor = Color.FromArgb(0, 0, 0, 0);
        bool Figurestart = false; // рисуем ли мы ее
        object figure = null;
        double f_x, f_y;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton b = sender as ToggleButton;
            if (b != null)
            {
                clearSelect();
                b.IsChecked = true;
               if (b == Toggle_Pencil)
               {
                    state = Draw_state.Pencil;
                    
               }
               if (b == Toggle_Rectangle)
               {
                    state = Draw_state.Rectangle;
                }
                if (b == Toggle_Circle)
                {
                    state = Draw_state.Circle;
                }
                if (b == Toggle_Line)
                {
                    state = Draw_state.Line;
                }
              
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            for (int c = 0; c < 23; c++) // create 23 buttons
            {
                Button b = new Button();
                b.Content = ""; // нет надписи на кнопке
                var br = new SolidColorBrush(Color.FromArgb((byte)rnd.Next(0, 255), 
                    (byte)rnd.Next(0, 255), 
                    (byte)rnd.Next(0, 255), 
                    (byte)rnd.Next(0, 255))); // random color
                b.Background = br; // фон кнопки
                b.Height = 15; // размер кнопки
                b.Width = 30;
                b.Click += Color_Click; // обработчик событий
                colbar.Items.Add(b); // добавляем кнопку в Toolbar
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string filename = openFileDialog.FileName; 
                Image image = new Image(); // Создаем обьект Image
                image.Source = new BitmapImage(new Uri(filename, UriKind.Absolute));
                // Источник - т.е. файл картинки существует по следующему адресу:
                Can.Children.Add(image);
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                string savenam = saveFileDialog.FileName;
                Rect rect = new Rect(); // Создается прямоугольник
                rect.X = 0;
                rect.Y = 0;
                rect.Width = Can.ActualWidth; // Реальная ширина холста
                rect.Height = Can.ActualHeight; // Реальная высота холста
                RenderTargetBitmap rtb = new RenderTargetBitmap((int)rect.Width, 
                (int)rect.Height, 96d, 96d, System.Windows.Media.PixelFormats.Default);
                // Создается обьект RenderTargetBitmap 
                // Векторная графика - растризуется
                rtb.Render(Can); // Преобразуем canvas в картинку
                BitmapEncoder pngEncoder = new PngBitmapEncoder(); // Кодировщик картинки (png)
                pngEncoder.Frames.Add(BitmapFrame.Create(rtb)); // Добавляем нашу картинку в кодировщик
                try
                {
                    FileStream fs = File.Open(savenam, FileMode.Create); // Создаем файловый поток куда писать данные
                    pngEncoder.Save(fs); // записываем Данные
                    fs.Close(); // Закрываем файловый поток
                }catch
                {
                    MessageBox.Show("Ошибка записи в файл");
                }

            }
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            //if (Clipboard.ContainsImage())
            //{
            //    Image img = new Image();
            //    img.Source = Clipboard.GetImage();
            //    img.Width = 100;
            //    img.Height = 100;
            //    Can.Children.Add(img);
            //}
            //else
            //{

            //}

        }


        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            Window1 winhelp = new Window1();
            winhelp.ShowDialog();
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            while (Can.Children.Count > 0)
            {
                Can.Children.Clear();
            }
        }

        private void Color_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                SolidColorBrush sb = (SolidColorBrush)b.Background; // Берем цвет с фона кнопки
                Drawcolor = sb.Color; // устанавливаем цвет
            }
        }

        void clearSelect()
        {
            Toggle_Rectangle.IsChecked = false;
            Toggle_Pencil.IsChecked = false;
            Toggle_Circle.IsChecked = false;
            Toggle_Line.IsChecked = false;
        }

        private void Can_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(Can); // получили точку мышки
            omousex = mousex;
            omousey = mousey;
            mousex = p.X;
            mousey = p.Y;

            if (e.LeftButton == MouseButtonState.Pressed) // проверка нажатия левой кнопки
            {
                switch (state)
                {
                    case Draw_state.No_state:
                        {
                            break;
                        }
                    case Draw_state.Pencil:
                        {
                            Line line = new Line();
                            line.Visibility = System.Windows.Visibility.Visible;
                            line.StrokeThickness = 4;
                            line.Stroke = new SolidColorBrush(Drawcolor);
                            line.X1 = omousex;
                            line.Y1 = omousey;
                            line.X2 = mousex;
                            line.Y2 = mousey;
                            Can.Children.Add(line);
                            break;

                        }
                    case Draw_state.Rectangle:
                        {
                            if (!Figurestart) // Если мы еще не начали рисовать фигуру
                            {
                                Figurestart = true; // начали и запомнили координаты начала
                                f_x = mousex;
                                f_y = mousey;
                                Rectangle rectangle = new Rectangle() // Создаем обьект Rectangle
                                {
                                    Stroke = new SolidColorBrush(Drawcolor), // Обводка
                                    Fill = new SolidColorBrush(Drawcolor), // Заливка
                                    Width = Math.Abs(Convert.ToDouble(f_x - omousex)), // Ширина 
                                    Height = Math.Abs(Convert.ToDouble(f_y - omousey)), // высота
                                    HorizontalAlignment = HorizontalAlignment.Left,
                                    VerticalAlignment = VerticalAlignment.Top
                                };

                                figure = rectangle;
                                Can.Children.Add(rectangle);
                                Canvas.SetTop(rectangle, mousey); // Устанавливаем координаты
                                Canvas.SetLeft(rectangle, mousex);
                            }
                            else
                            { // фигура существует
                                Rectangle rectangle = figure as Rectangle;
                                if (rectangle != null)
                                {
                                    rectangle.Width = Math.Abs(Convert.ToDouble(mousex - f_x));
                                    rectangle.Height = Math.Abs(Convert.ToDouble(mousey - f_y));
                                }
                            }
                            break;
                        }
                    case Draw_state.Circle:
                        {
                            if (!Figurestart) // Если мы еще не начали рисовать фигуру
                            {
                                Figurestart = true; // начали и запомнили координаты начала
                                f_x = mousex;
                                f_y = mousey;
                                Ellipse ellipse = new Ellipse() // Создаем обьект Ellipse
                                {
                                    Stroke = new SolidColorBrush(Drawcolor), // Обводка
                                    Fill = new SolidColorBrush(Drawcolor), // Заливка
                                    Width = Math.Abs(Convert.ToDouble(f_x - omousex)), // Ширина 
                                    Height = Math.Abs(Convert.ToDouble(f_y - omousey)), // высота
                                    HorizontalAlignment = HorizontalAlignment.Left,
                                    VerticalAlignment = VerticalAlignment.Top
                                };

                                figure = ellipse;
                                Can.Children.Add(ellipse);
                                Canvas.SetTop(ellipse, mousey); // Устанавливаем координаты
                                Canvas.SetLeft(ellipse, mousex);
                            }
                            else
                            { // фигура существует
                                Ellipse ellipse = figure as Ellipse; // получаем из figure
                                if (ellipse != null)
                                {
                                    ellipse.Width = Math.Abs(Convert.ToDouble(mousex - f_x));
                                    ellipse.Height = Math.Abs(Convert.ToDouble(mousey - f_y));
                                }
                            }
                            break;
                        }

                    case Draw_state.Line:
                        {
                            if (!Figurestart) // Если мы еще не начали рисовать фигуру
                            {
                                Figurestart = true; // начали и запомнили координаты начала
                                f_x = mousex;
                                f_y = mousey;
                                Line line = new Line() // Создаем обьект Line
                                {
                                    Stroke = new SolidColorBrush(Drawcolor), // Обводка
                                    Fill = new SolidColorBrush(Drawcolor), // Заливка
                                    // Ширина 
                                    // высота
                                    HorizontalAlignment = HorizontalAlignment.Left,
                                    VerticalAlignment = VerticalAlignment.Top,
                                    X1 = mousex,
                                    Y1= mousey,
                                    X2 = mousex,
                                    Y2 = mousey
                                };

                                figure = line;
                                Can.Children.Add(line);
                                
                            }
                            else
                            { // фигура существует
                                Line line = figure as Line; // получаем из figure
                                if (line != null)
                                {
                                    line.X2 = Math.Abs(Convert.ToDouble(mousex));
                                    line.Y2 = Math.Abs(Convert.ToDouble(mousey));
                                }
                            }
                            break;
                        }

                }

            }
            else
            {
               Figurestart = false;
            }
        }
    }
}

