using Syncfusion.Maui.DataGrid;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ContextMenuSample
{
    public class OrderInfoRepository : INotifyPropertyChanged
    {
        private ObservableCollection<OrderInfo>? orderInfo;

        public ObservableCollection<OrderInfo> OrderInfoCollection
        {
            get => orderInfo!;
            set
            {
                if (orderInfo != value)
                {
                    orderInfo = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand SortAscendingCommand { get; }
        public ICommand SortDescendingCommand { get; }
        public ICommand ClearSortingCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand UndoDeleteCommand { get; }
        private OrderInfo? _lastDeletedItem;
        private int _lastDeletedIndex = -1;
        public ICommand ExpandGroupCommand { get; }
        public ICommand CollapseGroupCommand { get; }
        public ICommand ShowTotalOrdersCommand { get; }
        public ICommand ShowTotalGroupsCommand { get; }

        public OrderInfoRepository()
        {
            OrderInfoCollection = new ObservableCollection<OrderInfo>();
            GenerateOrders();
            SortAscendingCommand = new Command<HeaderContextInfo>(OnSortAscending);
            SortDescendingCommand = new Command<HeaderContextInfo>(OnSortDescending);
            ClearSortingCommand = new Command<HeaderContextInfo>(OnClearSorting);
            DeleteCommand = new Command<RowContextMenuInfo>(OnDelete);
            UndoDeleteCommand = new Command(OnUndoDelete, CanUndoDelete);
            ExpandGroupCommand = new Command<GroupCaptionContextInfo>(OnExpandGroup);
            CollapseGroupCommand = new Command<GroupCaptionContextInfo>(OnCollapseGroup);
            ShowTotalOrdersCommand = new Command<TableSummaryContextInfo>(OnShowTotalOrders);
            ShowTotalGroupsCommand = new Command<TableSummaryContextInfo>(OnShowTotalGroups);
        }

        private void OnShowTotalOrders(TableSummaryContextInfo contextInfo)
        {
            if (contextInfo?.DataGrid == null) return;
            var grid = contextInfo.DataGrid;

            int totalOrders = grid.View?.Records?.Count
                              ?? (grid.ItemsSource as System.Collections.ICollection)?.Count
                              ?? 0;

            var row = contextInfo.SummaryRow ?? grid.TableSummaryRows.FirstOrDefault();
            if (row == null) return;

            row.ShowSummaryInRow = true;
            row.Title = $"Total Orders: {totalOrders}";

            grid.View?.Refresh();
        }

        private void OnShowTotalGroups(TableSummaryContextInfo contextInfo)
        {
            if (contextInfo?.DataGrid == null) return;
            var grid = contextInfo.DataGrid;

            int totalGroups = grid.View?.TopLevelGroup?.Groups?.Count ?? 0;

            var row = contextInfo.SummaryRow ?? grid.TableSummaryRows.FirstOrDefault();
            if (row == null) return;

            row.ShowSummaryInRow = true;
            row.Title = $"Total Groups: {totalGroups}";

            grid.View?.Refresh();
        }

        private void OnExpandGroup(GroupCaptionContextInfo context)
        {
            if (context?.DataGrid == null || context.Group == null) return;
            context.DataGrid.ExpandGroup(context.Group);
        }

        private void OnCollapseGroup(GroupCaptionContextInfo context)
        {
            if (context?.DataGrid == null || context.Group == null) return;
            context.DataGrid.CollapseGroup(context.Group);
        }


        private void OnDelete(RowContextMenuInfo context)
        {
            if (context?.RowData is not OrderInfo row)
            {
                return;
            }

            _lastDeletedIndex = OrderInfoCollection.IndexOf(row);
            _lastDeletedItem = row;
            OrderInfoCollection.Remove(row);
            (UndoDeleteCommand as Command)?.ChangeCanExecute();
        }

        private void OnUndoDelete()
        {
            if (_lastDeletedItem == null) return;

            var insertIndex = _lastDeletedIndex;
            if (insertIndex < 0 || insertIndex > OrderInfoCollection.Count)
            {
                insertIndex = OrderInfoCollection.Count;
            }

            OrderInfoCollection.Insert(insertIndex, _lastDeletedItem);
            _lastDeletedItem = null;
            _lastDeletedIndex = -1;
            (UndoDeleteCommand as Command)?.ChangeCanExecute();
        }

        private bool CanUndoDelete()
        {
            return _lastDeletedItem != null;
        }

        private void OnSortAscending(HeaderContextInfo context)
        {
            if (context?.DataGrid == null || context.Column == null) return;

            context.DataGrid.SortColumnDescriptions.Clear();
            context.DataGrid.SortColumnDescriptions.Add(new SortColumnDescription
            {
                ColumnName = context.Column.MappingName,
                SortDirection = System.ComponentModel.ListSortDirection.Ascending
            });
        }

        private void OnSortDescending(HeaderContextInfo context)
        {
            if (context?.DataGrid == null || context.Column == null) return;

            context.DataGrid.SortColumnDescriptions.Clear();
            context.DataGrid.SortColumnDescriptions.Add(new SortColumnDescription
            {
                ColumnName = context.Column.MappingName,
                SortDirection = System.ComponentModel.ListSortDirection.Descending
            });
        }

        private void OnClearSorting(HeaderContextInfo context)
        {
            context?.DataGrid?.SortColumnDescriptions.Clear();
        }

        public void GenerateOrders()
        {
            orderInfo!.Add(new OrderInfo("1001", "MA0024", "Maria Anders", "Germany", "Berlin"));
            orderInfo.Add(new OrderInfo("1002", "AT0165", "Ana Trujillo", "Mexico", "Mexico D.F."));
            orderInfo.Add(new OrderInfo("1003", "AF0006", "Ant Fuller", "Mexico", "Mexico D.F."));
            orderInfo.Add(new OrderInfo("1004", "TH0280", "Thomas Hardy", "UK", "London"));
            orderInfo.Add(new OrderInfo("1005", "TA0058", "Tim Adams", "Sweden", "Gothenburg"));
            orderInfo.Add(new OrderInfo("1006", "HM0012", "Hanna Moos", "Germany", "Mannheim"));
            orderInfo.Add(new OrderInfo("1007", "AF0037", "Andrew Fuller", "France", "Strasbourg"));
            orderInfo.Add(new OrderInfo("1008", "MK0138", "Martin King", "Spain", "Madrid"));
            orderInfo.Add(new OrderInfo("1009", "LL0015", "Lenny Lin", "France", "Marseille"));
            orderInfo.Add(new OrderInfo("1010", "JC0126", "John Carter", "Canada", "Ottawa"));
            orderInfo.Add(new OrderInfo("1011", "LK0045", "Laura King", "UK", "London"));
            orderInfo.Add(new OrderInfo("1012", "AW0134", "Anne Wilson", "Germany", "Mannheim"));
            orderInfo.Add(new OrderInfo("1013", "MK0055", "Martin King", "France", "Strasbourg"));
            orderInfo.Add(new OrderInfo("1014", "GI0092", "Gina Irene", "UK", "London"));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
