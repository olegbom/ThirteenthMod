using System;
using System.Reactive;
using ReactiveUI;

namespace ThirteenthMod.ViewModels
{
    public class ModResultViewModel : ViewModelBase
    {
        public int Value { get; set; }

        #region ClickCommand
            
        private ReactiveCommand<Unit, Unit> _clickCommand;
        public ReactiveCommand<Unit, Unit> ClickCommand => _clickCommand ??= ReactiveCommand.Create(Click);

        private void Click()
        {
            Choice?.Invoke(this, Value);
        }


        #endregion

        public event EventHandler<int> Choice;

    }
}