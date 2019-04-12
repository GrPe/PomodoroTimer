using System;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PomodoroTimer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly int workTime = 60 * 25; //25 minutes
        private readonly int breakTime = 60 * 5; // 5 minutes
        private int time = 60*25;
        CancellationTokenSource cancelToken = new CancellationTokenSource();
        Task task;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            ResetTimer(workTime);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            cancelToken.Cancel();
            time = 60 * 25;
            SetTime(time);
        }

        private void BreakButton_Click(object sender, RoutedEventArgs e)
        {
            ResetTimer(breakTime);
        }

        private void TimerUpdater()
        {
            try
            {
                while (time > 0)
                {
                    Thread.Sleep(1000);
                    time -= 1;

                    if (cancelToken.IsCancellationRequested)
                    {
                        break;
                    }

                    this.Dispatcher.Invoke(delegate
                    {
                        SetTime(time);
                    });
                }
                if(time <= 0)
                {
                    SystemSounds.Beep.Play();
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void SetTime(int time)
        {
            string minutes = (time / 60 > 9) ? (time / 60).ToString() : "0" + (time / 60).ToString();
            string seconds = (time % 60 > 9) ? (time % 60).ToString() : "0" + (time % 60).ToString();
            TimerLabel.Content = String.Format($"{minutes}:{seconds}");
        }

        private void ResetTimer(int time)
        {
            if (task != null && !cancelToken.IsCancellationRequested)
            {
                cancelToken.Cancel();
                task.Wait();
            }
            cancelToken.Dispose();
            cancelToken = new CancellationTokenSource();
            this.time = time;

            task = Task.Factory.StartNew(() => TimerUpdater(), cancelToken.Token);
        }
    }
}
