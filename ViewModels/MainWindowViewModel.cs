using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Xml.Linq;
using Avalonia.Threading;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ThirteenthMod.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private Random _r = new Random();

        public ObservableCollection<ModResultViewModel> ModResults { get; } = new ObservableCollection<ModResultViewModel>();
        public ObservableCollection<AnsweredTask> AnsweredTasks { get; } = new ObservableCollection<AnsweredTask>();

        [Reactive] public int BigNumber { get; set; }
        [Reactive] public double ElapsedTime { get; private set; }
        [Reactive] public bool IsFinish { get; private set; }

        [Reactive] public int NumberOfCorrectAnswers { get; private set; }

        private Stopwatch sw = new Stopwatch();

        public MainWindowViewModel()
        {
            for (int i = 0; i < 13; i++)
            {
                var modResult = new ModResultViewModel() {Value = i};
                modResult.Choice += ModResultOnChoice;
                ModResults.Add(modResult);
            }

            RerollNumber();

            DispatcherTimer timer = new DispatcherTimer(new TimeSpan(0,0,0,0,20), DispatcherPriority.Normal, SecondTimerCallback);
            timer.Start();
            sw.Start();
        }

        private void SecondTimerCallback(object sender, EventArgs e)
        {
            ElapsedTime = sw.Elapsed.TotalSeconds;
        }

        private double _lastTime = 0;
        private void ModResultOnChoice(object sender, int e)
        {
            double totalTime = sw.Elapsed.TotalSeconds;
            AnsweredTasks.Add(new AnsweredTask(BigNumber, e, totalTime - _lastTime));
            _lastTime = totalTime;
            if (AnsweredTasks.Count == 10)
            {
                NumberOfCorrectAnswers = AnsweredTasks.Count(x => x.IsAnswerCorrect);
                IsFinish = true;
                _lastTime = 0;
                sw.Stop();
            }
            else
            {
                RerollNumber();
            }
        }


        #region RestartCommand

        private ReactiveCommand<Unit, Unit> _restartCommand;
        public ReactiveCommand<Unit, Unit> RestartCommand => _restartCommand ??= ReactiveCommand.Create(Restart);

        private void Restart()
        {
            AnsweredTasks.Clear();
            NumberOfCorrectAnswers = 0;
            IsFinish = false;
            RerollNumber();
            sw.Restart();
        }


        #endregion

        public void RerollNumber()
        {
            BigNumber = _r.Next(1000, 10000);
        }
    }

    public class AnsweredTask : ViewModelBase
    {
        public int BigNumber { get; }
        public int Answer { get; }
        public int TrueAnswer => BigNumber % 13;

        public double Time { get; }
        public bool IsAnswerCorrect => Answer == TrueAnswer;

        public AnsweredTask(int bigNumber, int answer, double time)
        {
            BigNumber = bigNumber;
            Answer = answer;
            Time = time;
        }
    }


}
