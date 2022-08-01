using Calculator_WinUI3_MVVMToolkit7.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static Calculator_WinUI3_MVVMToolkit7.Models.Calculator;

namespace Calculator_WinUI3_MVVMToolkit7
{
    internal class ViewModel : ObservableObject
    {
        public ViewModel()
        {
            AddTokenCommand = new RelayCommand<string>(AddToken);
        }

        private Calculator calculator = new();

        public string Monitor { get => GetMonitor(calculator); }
        public string SubMonitor { get => GetSubMonitor(calculator); }
        public IEnumerable<decimal> Stack { get=>calculator.EnumerateStack(); }

        public IRelayCommand<string> AddTokenCommand { get; }

        public void AddToken(string tokenStr)
        {
            if(TokenExt.TryParse(out var token, tokenStr))
            {
                calculator.AddToken(token);
                NotifyUpdate();
            }
        }

        public static string GetMonitor(Calculator calculator)
        {
            if (calculator.state == State.Result)
            {
                return calculator.PeekStack(0).ToString();
            }
            else if(calculator.state == State.InputNum)
            {
                return calculator.CurrentNumberBuilder();
            }
            else if(calculator.state == State.InputOp)
            {
                return calculator.PeekStack(0).ToString();
            }
            else
            {
                return "(error)";
            }
        }

        public static string GetSubMonitor(Calculator calculator)
        {
            if(calculator.state == State.Result)
            {
                return "";
            }
            else if(calculator.state == State.InputNum || calculator.state == State.InputOp)
            {
                return $"{calculator.PeekStack()} {calculator.Op?.PrettyString() ?? ""}";
            }
            else
            {
                return "";
            }
        }

        public void NotifyUpdate()
        {
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Monitor)));
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(SubMonitor)));
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Stack)));
        }

    }
}
