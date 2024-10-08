using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace CS
{
    class AppViewModel : INotifyPropertyChanged
    {
        AppModel Model = new();

        public String Code
        {
            get => Model.Code;
            set
            {
                if (Model.Code != value)
                {
                    Model.Code = value;
                    OnPropertyChanged(nameof(Code));
                }
            }
        }

        public String Conclusion
        {
            get => Model.Conclusion;
            set
            {
                if (Model.Conclusion != value)
                {
                    Model.Conclusion = value;
                    OnPropertyChanged(nameof(Conclusion));
                }
            }
        }

        public ICommand OpenCodeFileCommand
        {
            get
            {
                return new UniversalCommand(
                    async obj =>
                    {
                        OpenFileDialog openFileDialog = new OpenFileDialog();
                        if (openFileDialog.ShowDialog() == true)
                        {
                            FileStream CodeFileStream = new(openFileDialog.FileName, FileMode.Open);
                            byte[] buffer = new byte[CodeFileStream.Length];
                            await CodeFileStream.ReadAsync(buffer, 0, buffer.Length);
                            Code = new string(Encoding.Default.GetString(buffer));
                        }
                    });
            }
        }

        public ICommand ClearCodeText
        {
            get
            {
                return new UniversalCommand(
                    obj =>
                    {
                        Code = "";
                    },
                    obj =>
                    {
                        return !String.IsNullOrEmpty(Model.Code);
                    });
            }
        }

        public ICommand MakeConclusion
        {
            get
            {
                return new UniversalCommand(
                    obj =>
                    {
                        // Множества для хранения уникальных операторов и операндов
                        HashSet<string> operators = new HashSet<string>();
                        HashSet<string> operands = new HashSet<string>();

                        // Счетчики операторов и операндов
                        int totalOperators = 0;
                        int totalOperands = 0;

                        // Регулярные выражения для поиска операторов и операндов
                        string operatorPattern = @"[+\-*/=<>!&|]";
                        string operandPattern = @"\b[a-zA-Z_][a-zA-Z0-9_]*|\b\d+\b";

                        // Поиск операторов
                        foreach (Match match in Regex.Matches(Model.Code, operatorPattern))
                        {
                            operators.Add(match.Value);
                            totalOperators++;
                        }

                        // Поиск операндов
                        foreach (Match match in Regex.Matches(Model.Code, operandPattern))
                        {
                            operands.Add(match.Value);
                            totalOperands++;
                        }

                        // Метрики Холстеда
                        int n1 = operators.Count;   // Количество уникальных операторов
                        int n2 = operands.Count;    // Количество уникальных операндов
                        int N1 = totalOperators;    // Общее количество операторов
                        int N2 = totalOperands;     // Общее количество операндов

                        // Объем программы (V)
                        double V = (N1 + N2) * Math.Log2(n1 + n2);

                        // Трудоемкость (E)
                        double eta = 1;  // Коэффициент уровня программы (предположим 1)
                        double E = V / eta;

                        // Прогнозируемое количество ошибок (B)
                        double B = Math.Pow(E, 2.0 / 3.0) / 3000;

                        // Возврат метрик в виде словаря
                        Conclusion =
                        "n1 (уникальные операторы)\t\t:\t" + n1.ToString() +
                        "\nn2 (уникальные операнды)\t\t:\t" + n2.ToString() +
                        "\nN1 (общее количество операторов)\t:\t" + N1.ToString() +
                        "\nN2 (общее количество операндов)\t:\t" + N2.ToString() +
                        "\nV (объем программы)\t\t\t:\t" + V.ToString() +
                        "\nE (трудоемкость)\t\t\t\t:\t" + E.ToString() +
                        "\nB (прогнозируемое количество ошибок)\t:\t" + B;
                    },
                    obj =>
                    {
                        return !String.IsNullOrEmpty(Model.Code);
                    });
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string PropertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }
}
