using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CodeElements.UpdateSystem.Core.Internal
{
    internal class ProcessCollection : IProcessColumn
    {
        private readonly List<IProcessColumn> _columns;
        private double _weight;

        public ProcessCollection()
        {
            _columns = new List<IProcessColumn>();
            Weight = 1;
        }

        public ProcessCollection(double weight)
        {
            Weight = weight;
        }

        public double Current { get; private set; }
        public double Maximum { get; } = 1;

        public double Weight
        {
            get => _weight;
            set
            {
                if (_weight != value)
                {
                    _weight = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ProcessColumn AddColumn(double maximum, double weight)
        {
            var column = new ProcessColumn(maximum, weight);
            AddColumn(column);
            return column;
        }

        public ProcessCollection AddColumnCollection(double weight)
        {
            var column = new ProcessCollection(weight);
            Add(column);
            Recompute();
            return column;
        }

        public void AddColumn(IProcessColumn column)
        {
            Add(column);
            Recompute();
        }

        private void Recompute()
        {
            var totalWeight = 0d;
            var currentProcess = 0d;
            foreach (var processColumn in _columns)
            {
                totalWeight += processColumn.Weight;
                currentProcess += processColumn.Current / processColumn.Maximum * processColumn.Weight;
            }

            Current = currentProcess / totalWeight;
            OnPropertyChanged(nameof(Current));
        }

        public void AddColumns(IEnumerable<IProcessColumn> columns)
        {
            foreach (var processColumn in columns)
                Add(processColumn);

            Recompute();
        }

        private void Add(IProcessColumn column)
        {
            _columns.Add(column);
            column.PropertyChanged += ColumnOnPropertyChanged;
        }

        private void ColumnOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            Recompute();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    internal interface IProcessColumn : INotifyPropertyChanged
    {
        double Current { get; }
        double Maximum { get; }
        double Weight { get; }
    }

    internal class ProcessColumn : IProcessColumn
    {
        private double _current;
        private double _maximum;
        private double _weight;

        public ProcessColumn(double maximum = 1, double weight = 1)
        {
            _maximum = maximum;
            Weight = weight;
        }

        public double Maximum
        {
            get => _maximum;
            set
            {
                if (Maximum != value)
                {
                    _maximum = value;
                    OnPropertyChanged();
                }
            }
        }

        public double Current
        {
            get => _current;
            set
            {
                if (Current != value)
                {
                    _current = value;
                    OnPropertyChanged();
                }
            }
        }

        public double Weight
        {
            get => _weight;
            set
            {
                if (_weight != value)
                {
                    _weight = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Increment(double value = 1)
        {
            Current += value;
        }

        public void Complete()
        {
            Current = Maximum;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}